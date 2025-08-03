using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlmanacClasses.API;
using BepInEx;
using CreatureQuests.QuestSystem.Behaviors;
using HarmonyLib;
using Shapeshift.QuestSystem.Behaviors;
using ShapeShiftManager;
using UnityEngine;
using YamlDotNet.Serialization;

namespace CreatureQuests.QuestSystem;

public static class QuestManager
{
    private const string PlayerCustomDataKey = "RavenQuestData";
    public static Quest? m_currentQuest;
    public static readonly Dictionary<int, Quest> m_questQueue = new();
    public static readonly Dictionary<string, Quest> m_quests = new();

    private static readonly Dictionary<string, Character> m_defeatKeyToCharacter = new();

    public static bool FirstQuestTriggered;
    public static bool HasAvailableQuests() => m_questQueue.Values.Any(quest => !quest.m_completed && quest.m_isValid);
    
    public static Quest? GetQuest()
    {
        foreach (var quest in m_questQueue.OrderBy(q => q.Value.Index).Select(q => q.Value))
        {
            if (!quest.IsCompleted() && quest.m_isValid)
                return quest;
        }
        return null;
    }
    
    private static void SaveProgress()
    {
        if (!Player.m_localPlayer ) return;
        List<string> m_completedQuests = new();
        foreach (var kvp in m_quests)
        {
            if (!kvp.Value.m_completed) continue;
            m_completedQuests.Add(kvp.Key);
        }
        ISerializer serializer = new SerializerBuilder().Build();
        string data = serializer.Serialize(m_completedQuests);
        Player.m_localPlayer.m_customData[PlayerCustomDataKey] = data;
    }
    
    private static void LoadProgress(Player player)
    {
        FirstQuestTriggered = false;
        if (!player.m_customData.TryGetValue(PlayerCustomDataKey, out string serializedData)) return;
        try
        {
            IDeserializer deserializer = new DeserializerBuilder().Build();
            var data = deserializer.Deserialize<List<string>>(serializedData);
            foreach (var quest in m_quests.Values)
            {
                if (!data.Contains(quest.Name)) continue;
                quest.SetCompleted();
                FirstQuestTriggered = true;
            }
        }
        catch
        {
            player.m_customData.Remove(PlayerCustomDataKey);
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Load))]
    private static class Player_Load_Patch
    {
        private static void Postfix(Player __instance) => LoadProgress(__instance);
    }

    [HarmonyPatch(typeof(Game), nameof(Game.SavePlayerProfile))]
    private static class Game_SavePlayerProfile_Patch
    {
        private static void Prefix() => SaveProgress();
    }

    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    private static class ObjectDB_Awake_Patch
    {
        private static void Postfix(ObjectDB __instance)
        {
            if (!__instance || !ZNetScene.instance) return;
            foreach (var prefab in ZNetScene.instance.m_prefabs)
            {
                if (!prefab.TryGetComponent(out Character character)) continue;
                prefab.AddComponent<DropRelic>();
                if (character.m_defeatSetGlobalKey.IsNullOrWhiteSpace()) continue;
                m_defeatKeyToCharacter[character.m_defeatSetGlobalKey] = character;
            }
            foreach (Quest quest in m_quests.Values)
            {
                quest.Validate();
            }
        }
    }

    public class Quest
    {
        public readonly string Name;
        public string Description;
        public readonly int Index;
        public string RequiredShapeshiftForm = "";
        public string RequiredDefeatKey = "";
        public string RequiredDefeatCharacterSharedName = "";
        public float Duration = 500f;
        public Heightmap.Biome RequiredBiome = Heightmap.Biome.None;
        public readonly Reward Rewards;
        public readonly QuestData Data;
        public Bounty? m_currentBounty;
        public readonly Queue<Dialogue> m_startQueue = new();
        public readonly Queue<Dialogue> m_cancelQueue = new();
        public readonly Queue<Dialogue> m_completedQueue = new();
        public Action? OnStartQuest;
        public Action? OnCancelQuest;
        public Action? OnCollectReward;
        public bool m_completed;
        public bool m_isValid = true;
        public bool m_activated;

        public Quest(string name, string description, int index, QuestData.QuestType type)
        {
            Name = name;
            Index = index;
            Description = description;
            Data = new QuestData(type, this);
            Rewards = new Reward(this);
            m_quests[name] = this;
            m_questQueue[index] = this;
        }

        public Quest(string name, int index) : this(name, "", index, QuestData.QuestType.None){}
        
        public Quest(string name, string description, QuestData.QuestType type)
            : this(name, description, GetNextIndex(), type) { }

        private static int GetNextIndex()
        {
            int i = 0;
            while (m_questQueue.ContainsKey(i))
            {
                i++;
            }
            return i;
        }

        public bool HasRequiredKey()
        {
            if (RequiredDefeatKey.IsNullOrWhiteSpace()) return true;
            if (!Player.m_localPlayer || !ZoneSystem.instance) return false;
            bool playerHasKey = Player.m_localPlayer.HaveUniqueKey(RequiredDefeatKey);
            bool worldHasKey = Enum.TryParse(RequiredDefeatKey, true, out GlobalKeys globalKey)
                ? ZoneSystem.instance.GetGlobalKey(globalKey)
                : ZoneSystem.instance.GetGlobalKey(RequiredDefeatKey);
            return playerHasKey || worldHasKey;
        }

        public bool HasRequirements()
         {
             bool isCorrectForm = RequiredShapeshiftForm.IsNullOrWhiteSpace() || RequiredShapeshiftForm == CreatureFormManager.m_currentCreature;
             bool hasKey = HasRequiredKey();
             return isCorrectForm && hasKey;
         }

        public void Validate()
        {
            if (!CreatureFormManager.IsValidPrefabName(RequiredShapeshiftForm))
            {
                CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Index}.{Name}] Invalid creature: {RequiredShapeshiftForm}");
                m_isValid = false;
                return;
            }

            if (!RequiredDefeatKey.IsNullOrWhiteSpace())
            {
                if (!m_defeatKeyToCharacter.TryGetValue(RequiredDefeatKey, out Character requiredDefeatedCharacter))
                {
                    CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Index}.{Name}] Invalid required key: {RequiredDefeatKey}");
                    m_isValid = false;
                    return;
                }

                RequiredDefeatCharacterSharedName = requiredDefeatedCharacter.m_name;
            }
            ValidateData();
            ValidateRewardData();
            if (Data.Type is QuestData.QuestType.Bounty) ValidateBounty();
        }

        public bool IsCorrectType(QuestData.QuestType type) => Data.Type == type;
        
        public bool IsMatchingPrefab(string name, bool sharedName = false)
        {
            name = Helpers.RemoveParentheses(name);
            return sharedName ? Data.SharedName == name : name == Data.PrefabName;
        }

        private void ValidateData()
        {
            if (!Data.Validate()) m_isValid = false;
        }

        private void ValidateRewardData()
        {
            foreach (var reward in Rewards.List)
            {
                reward.Parent = this;
                if (!reward.Validate()) m_isValid = false;
            }
        }

        public void AcceptQuest()
        {
            OnStartQuest?.Invoke();
            m_currentQuest = this;
            QuestRaven.m_questData = null;
            Shapeshift();
            if (!CreatureQuestsPlugin.plugin.IsInvoking()) CreatureQuestsPlugin.plugin.Invoke(nameof(CreatureQuestsPlugin.ToggleFly), 5f);
            StartBountySpawn();
            UI.Show(Name, Localization.instance.Localize(GetTooltip()), GetProgressText());
            m_activated = true;
        }

        private string GetTooltip()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Description);
            builder.Append("\n<color=orange>$raven_reward</color>:\n");
            foreach (var reward in Rewards.List)
            {
                switch (reward.Type)
                {
                    case RewardData.RewardType.Item:
                        var localizedName = Localization.instance.Localize(reward.SharedName);
                        if (reward.Quality > 1)
                        {
                            builder.AppendFormat("{0}, {1}, {2}\n", localizedName, reward.Amount, reward.Quality);
                        }
                        else
                        {
                            builder.AppendFormat("{0}, {1}\n", localizedName, reward.Amount);
                        }
                        break;
                    case RewardData.RewardType.Skill:
                        var skill = $"$skill_{reward.SkillType.ToString().ToLower()}";
                        builder.Append($"{skill} +{reward.ExperienceAmount}\n");
                        break;
                    case RewardData.RewardType.Spawn:
                        break;
                    case RewardData.RewardType.AlmanacEXP:
                        break;
                }
            }

            return builder.ToString();
        }

        private void Shapeshift(bool revert = false)
        {
            if (Player.m_localPlayer == null) return;
            if (revert)
            {
                CreatureFormManager.Revert(Player.m_localPlayer);
            }
            else
            {
                if (CreatureFormManager.TriggerTransformation(Player.m_localPlayer, RequiredShapeshiftForm, Duration)) return;
                var data = CreatureFormManager.GetCreature(RequiredShapeshiftForm);
                if (data == null) return;
                var hash = data.StatusEffect.name.GetStableHashCode();
                if (Player.m_localPlayer.GetSEMan().GetStatusEffect(hash) is { } effect)
                {
                    effect.m_ttl = Duration;
                }
            }
        }

        private void ValidateBounty()
        {
            GameObject prefab = ZNetScene.instance.GetPrefab(Data.PrefabName);
            if (!prefab)
            {
                CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Index}.{Name}] Invalid bounty prefab: {Data.PrefabName}");
                m_isValid = false;
                return;
            }
            Data.BountyData.m_critter = prefab;
        }

        private void StartBountySpawn()
        {
            if (Data.Type is not QuestData.QuestType.Bounty) return;
            if (!Data.BountyData.m_critter)
            {
                CancelQuest();
                return;
            }
            
            if (!Bounty.FindSpawnPoint(out Vector3 pos))
            {
                CancelQuest();
                return;
            }

            Bounty.PreSpawnEffectList.Create(pos, Quaternion.identity);
            Data.BountyData.m_pos = pos;
            CreatureQuestsPlugin.plugin.Invoke(nameof(CreatureQuestsPlugin.SpawnBounty), 10f);
        }

        private void StartRewardSpawn()
        {
            foreach (var reward in Rewards.List)
            {
                if (reward.Type is not RewardData.RewardType.Spawn) return;
                if (!Data.PrefabName.IsNullOrWhiteSpace())
                {
                    CancelQuest();
                    return;
                }

                Bounty.PreSpawnEffectList.Create(Player.m_localPlayer.transform.position, Quaternion.identity);
                if (!CreatureQuestsPlugin.plugin.IsInvoking()) CreatureQuestsPlugin.plugin.Invoke(nameof(CreatureQuestsPlugin.Spawn), 10f);
            }
        }
        
        public void CancelQuest()
        {
            OnCancelQuest?.Invoke();
            Shapeshift(true);
            Data.Progress = 0;
            m_currentQuest = null;
            QuestRaven.m_questData = null;
            if (!CreatureQuestsPlugin.plugin.IsInvoking()) CreatureQuestsPlugin.plugin.Invoke(nameof(CreatureQuestsPlugin.ToggleFly), 5f);
            UI.Hide();
            m_activated = false;
        }

        public void OnDeath()
        {
            Data.Progress = 0;
            m_currentQuest = null;
            QuestRaven.m_questData = null;
            UI.Hide();
            m_activated = false;
        }

        public void Reset()
        {
            SetProgress(0);
            Data.PrefabNames.AddRange(Data.FoundPrefabs);
            Data.FoundPrefabs.Clear();
            m_completed = false;
        }

        public void SetCompleted()
        {
            if (Data.Progress < Data.Amount) Data.Progress = Data.Amount;
            m_completed = true;
        }
        
        public bool IsCompleted() => Data.Progress >= Data.Amount;

        public void IncreaseProgress(int amount = 1)
        {
            Data.Progress += amount;
            UI.UpdateProgress(GetProgressText());
        }

        public void IncreaseFoundPrefabs(string name)
        {
            Data.FoundPrefabs.Add(name);
            Data.PrefabNames.Remove(name);
            IncreaseProgress();
        }

        public void SetProgress(int amount)
        {
            Data.Progress = amount;
        }

        private string GetProgressText()
        {
            switch (Data.Type)
            {
                default:
                    return Localization.instance.Localize(
                        $"$quest_type_{Data.Type.ToString().ToLower()} <color=orange>{Data.Progress}</color>/{Data.Amount} {Data.SharedName}");
            }
        }

        private void CollectReward()
        {
            if (!Player.m_localPlayer) return;
            OnCollectReward?.Invoke();
            Shapeshift(true);
            if (m_completed)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Already collected reward");
            }
            else
            {
                foreach (var reward in Rewards.List)
                {
                    switch (reward.Type)
                    {
                        case RewardData.RewardType.Item:
                            GameObject? item = ObjectDB.instance.GetItemPrefab(reward.PrefabName);
                            if (!item  || !item.TryGetComponent(out ItemDrop component)) return;
                            ItemDrop.ItemData data = component.m_itemData.Clone();
                            data.m_stack = reward.Amount;
                            data.m_dropPrefab = item;
                            data.m_crafterName = Player.m_localPlayer.GetHoverName();
                            data.m_crafterID = Player.m_localPlayer.GetPlayerID();
                            data.m_variant = reward.Variant;
                            data.m_quality = reward.Quality;
                            Player.m_localPlayer.GetInventory().AddItem(data);
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"Received {reward.Amount} x {reward.SharedName}");
                            break;
                        case RewardData.RewardType.Skill:
                            Player.m_localPlayer.GetSkills().RaiseSkill(reward.SkillType, reward.ExperienceAmount);
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"Received {reward.ExperienceAmount} $skill_{reward.SkillType.ToString().ToLower()} experience");
                            break;
                        case RewardData.RewardType.AlmanacEXP:
                            ClassesAPI.AddEXP((int)reward.ExperienceAmount);
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"Received {reward.ExperienceAmount} Almanac Class Experience");
                            break;
                        case RewardData.RewardType.Spawn:
                            StartRewardSpawn();
                            break;
                    }
                }
                SetCompleted();
            }
            m_currentQuest = null;
            UI.Hide();
            QuestRaven.ToggleFly(false);
        }

        public void AddCancelText(string text) => m_cancelQueue.Enqueue(new Dialogue(text));

        public void AddCancelAction(string acceptText, string cancelText)
        {
            Dialogue dialogue = new Dialogue(acceptText, cancelText, alt =>
            {
                if (alt) CancelQuest();
                else
                {
                    if (!CreatureQuestsPlugin.plugin.IsInvoking()) CreatureQuestsPlugin.plugin.Invoke(nameof(CreatureQuestsPlugin.ToggleFly), 5f);
                }
            });
            m_cancelQueue.Enqueue(dialogue);
        }

        public void AddStartText(string text) => m_startQueue.Enqueue(new Dialogue(text));

        public void AddStartAction(string acceptText, string cancelText)
        {
            Dialogue dialogue = new Dialogue(acceptText, cancelText, alt =>
            {
                if (!alt) AcceptQuest();
                else
                {
                    QuestRaven.m_questData = null;
                    if (!CreatureQuestsPlugin.plugin.IsInvoking()) CreatureQuestsPlugin.plugin.Invoke(nameof(CreatureQuestsPlugin.ToggleFly), 5f);
                }
            });
            m_startQueue.Enqueue(dialogue);
        }

        public void AddCompletedText(string text) => m_completedQueue.Enqueue(new Dialogue(text));

        public void AddCompletedAction(string acceptText, string cancelText)
        {
            Dialogue dialogue = new Dialogue(acceptText, cancelText, alt =>
            {
                if (!alt) CollectReward();
                else
                {
                    if (!CreatureQuestsPlugin.plugin.IsInvoking()) CreatureQuestsPlugin.plugin.Invoke(nameof(CreatureQuestsPlugin.ToggleFly), 5f);
                }
            });
            m_completedQueue.Enqueue(dialogue);
        }
        
        public class Dialogue
        {
            public readonly string m_acceptText;
            public readonly string m_cancelText;
            public readonly Action<bool>? m_action;

            public Dialogue(string acceptText, string cancelText = "")
            {
                m_acceptText = acceptText;
                m_cancelText = cancelText;
            }

            public Dialogue(string acceptText, string cancelText, Action<bool>? action)
            {
                m_acceptText = acceptText;
                m_cancelText = cancelText;
                m_action = action;
            }
        }
        public class QuestData
        {
            public readonly Quest Parent;
            public QuestType Type;
            public string PrefabName = "";
            public List<string> PrefabNames = new();
            public readonly List<string> SharedNames = new();
            public readonly Dictionary<string, string> Names = new();
            public readonly List<SpawnData> EventData = new();
            public float EventDuration = 100f;
            public int PrefabCount;
            public List<string> FoundPrefabs = new();
            public string? SharedName;
            public int Amount;
            public int Level;
            public int Progress;
            public readonly Bounty.BountyModifiers BountyData = new();
            
            public void AddEventSpawn(string prefabName, int minLevel = 1, int maxLevel = 3)
            {
                EventData.Add(new SpawnData(prefabName, minLevel, maxLevel));
            }

            public void Set(List<string> prefabNames, string key)
            {
                PrefabNames = prefabNames;
                Amount = prefabNames.Count;
                PrefabCount = prefabNames.Count;
                PrefabName = key;
            }
            public void Set(string prefabName, int amount = 1, int level = 1)
            {
                PrefabName = prefabName;
                Amount = amount;
                Level = level;
                if (Type is QuestType.Bounty) Amount = 1;
            }

            public void Set(int amount)
            {
                Amount = amount;
            }

            public void Set(string prefabName, string name)
            {
                PrefabName = prefabName;
                BountyData.m_name = name;
                Amount = 1;
                Type = QuestType.Bounty;
            }

            public bool Validate()
            {
                if (Type is QuestType.Run or QuestType.Jump or QuestType.Fly or QuestType.Attach or QuestType.Travel) return true;
                if (PrefabNames.Count > 0 && Type is QuestType.CollectItems)
                {
                    foreach (var name in PrefabNames)
                    {
                        if (ZNetScene.instance.GetPrefab(name) is { } itemPrefab && itemPrefab.TryGetComponent(out ItemDrop itemComponent))
                        {
                            SharedNames.Add(itemComponent.m_itemData.m_shared.m_name);
                            Names[name] = itemComponent.m_itemData.m_shared.m_name;
                        }
                        else
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid collect item: {name}");
                            return false;
                        }
                    }
                }

                if (PrefabName.IsNullOrWhiteSpace())
                {
                    CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] quest data prefab is null");
                    return false;
                }
                if (ZNetScene.instance.GetPrefab(PrefabName) is not { } prefab)
                {
                    CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid data prefab: {PrefabName}");
                    return false;
                }
                switch (Type)
                {
                    case QuestType.Kill:
                        if (!prefab.TryGetComponent(out Character character))
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid kill prefab: {PrefabName}");
                            return false;
                        }
                        SharedName =character.m_name;
                        break;
                    case QuestType.Tame:
                        if (!prefab.TryGetComponent(out Character tame) || !prefab.GetComponent<Tameable>())
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid tame prefab: {PrefabName}");
                            return false;
                        }
                        SharedName = tame.m_name;
                        break;
                    case QuestType.Procreate:
                        if (!prefab.TryGetComponent(out Character mate) || !prefab.GetComponent<Procreation>())
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid procreate prefab: {PrefabName}");
                            return false;
                        }
                        SharedName = mate.m_name;
                        break;
                    case QuestType.Harvest:
                        if (!prefab.TryGetComponent(out Pickable pickable))
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid pickable prefab: {PrefabName}");
                            return false;
                        }
                        SharedName = pickable.GetHoverName();
                        break;
                    case QuestType.Collect:
                        if (!prefab.TryGetComponent(out ItemDrop itemDrop))
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid craft prefab: {PrefabName}");
                            return false;
                        }
                        SharedName = itemDrop.m_itemData.m_shared.m_name;
                        break;
                    case QuestType.Farm:
                        if (!prefab.TryGetComponent(out Piece piece))
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid farm prefab: {PrefabName}");
                            return false;
                        }
                        SharedName = piece.m_name;
                        break;
                    case QuestType.Destructible:
                        if (!prefab.TryGetComponent(out Destructible destructible))
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid destructible prefab: {PrefabName}");
                            return false;
                        }

                        if (!prefab.TryGetComponent(out HoverText hoverText))
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid destructible prefab, missing hover text: {PrefabName}");
                            return false;
                        }
                        SharedName = hoverText.m_text;
                        break;
                    case QuestType.Bounty:
                        SharedName = BountyData.m_name;
                        break;
                }

                return true;
            }

            public enum QuestType
            {
                None,
                Kill,
                Harvest,
                Collect,
                CollectItems,
                Bounty,
                Farm,
                Destructible,
                Tame,
                Procreate,
                Run,
                Jump,
                Fly,
                Attach,
                Travel
            }

            public QuestData(QuestType questType, Quest parent)
            {
                Type = questType;
                Parent = parent;
            }
        }

        public class Reward
        {
            private readonly Quest Parent;
            public readonly List<RewardData> List = new();

            public Reward(Quest parent)
            {
                Parent = parent;
            }
            
            public void Set(Skills.SkillType skillType, float amount)
            {
                var reward = new RewardData(Parent);
                reward.Set(skillType, amount);
                List.Add(reward);
            }

            public void Set(string creatureName, int amount, float delay)
            {
                var reward = new RewardData(Parent);
                reward.Set(creatureName, amount, delay);
                List.Add(reward);
            }

            public void Set(string prefab, int amount = 1, int quality = 1, int variant = 0)
            {
                var reward = new RewardData(Parent);
                reward.Set(prefab, amount, quality, variant);
                List.Add(reward);
            }
        }

        public class RewardData
        {
            public Quest Parent;
            public RewardType Type { get; private set; }
            public string PrefabName { get; private set; } = null!;
            public string SharedName = "";
            public int Amount { get; private set; }
            public int Quality { get; private set; }
            public int Variant { get; private set; }
            public Skills.SkillType SkillType { get; private set; }
            public float ExperienceAmount { get; private set; }

            public RewardData(Quest parent)
            {
                Parent = parent;
            }

            public void Set(string itemName, int amount, int quality, int variant)
            {
                Type = RewardType.Item;
                PrefabName = itemName;
                Amount = amount;
                Quality = quality;
                Variant = variant;
            }

            public bool Validate()
            {
                switch (Type)
                {
                    case RewardType.Skill:
                        
                        break;
                    case RewardType.Spawn:
                        if (PrefabName.IsNullOrWhiteSpace())
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Reward prefab is null");
                            return false;
                        }
                        GameObject creature = ZNetScene.instance.GetPrefab(PrefabName);
                        if (!creature || !creature.TryGetComponent(out Character character))
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid reward spawn: {PrefabName}");
                            return false;
                        }
                        SharedName = character.m_name;
                        break;
                    default:
                        if (PrefabName.IsNullOrWhiteSpace())
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Reward prefab is null");
                            return false;
                        }
                        GameObject item = ObjectDB.instance.GetItemPrefab(PrefabName);
                        if (!item || !item.TryGetComponent(out ItemDrop itemDrop))
                        {
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning($"[{Parent.Index}.{Parent.Name}] Invalid reward: {PrefabName}");
                            return false;
                        }
                        SharedName = itemDrop.m_itemData.m_shared.m_name;
                        break;
                }

                return true;
            }

            public void Set(Skills.SkillType skillType, float amount)
            {
                Type = RewardType.Skill;
                SkillType = skillType;
                ExperienceAmount = amount;
            }

            public void Set(string creatureName, int amount, float delay)
            {
                PrefabName = creatureName;
                Amount = amount;
            }

            public enum RewardType
            {
                Item,
                Skill,
                AlmanacEXP,
                Spawn
            }
        }

        public class SpawnData
        {
            public readonly string PrefabName;
            public readonly int MaxLevel;
            public readonly int MinLevel;

            public SpawnData(string prefab, int min, int max)
            {
                PrefabName = prefab;
                MaxLevel = max;
                MinLevel = min;
            }
        }
    }

    [Serializable]
    public class PlayerQuestData
    {
        public string QuestName = null!;
        public int Progress;
        public bool Completed;
        public List<string> FoundPrefabs = new();
    }
}