using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using HarmonyLib;
using ServerSync;
using YamlDotNet.Serialization;

namespace CreatureQuests.QuestSystem;

public static class QuestMaker
{
    private static readonly string RootFolder = Paths.ConfigPath + Path.DirectorySeparatorChar + "CreatureQuests";
    private static readonly string FolderName = "Quests";
    private static readonly string FolderPath = Path.Combine(RootFolder, FolderName);
    private static readonly string DefaultFileName = "Defaults.yml";
    private static readonly string DefaultFilePath = Path.Combine(FolderPath, DefaultFileName);

    private static readonly CustomSyncedValue<string> ServerData = new(CreatureQuestsPlugin.ConfigSync, "ServerRavenQuests", "");
    private static List<List<string>> QuestBlocks = new();

    public static void Setup()
    {
        WriteDefaults();
        Read();
        SetupServerWatch();
        SetupFileWatch();
    }

    private static void SetupFileWatch()
    {
        FileSystemWatcher watcher = new FileSystemWatcher(FolderPath, "*.yml");
        watcher.EnableRaisingEvents = true;
        watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        watcher.IncludeSubdirectories = true;
        watcher.Created += OnFileChange;
        watcher.Changed += OnFileChange;
        watcher.Deleted += OnFileChange;
    }

    private static void OnFileChange(object sender, FileSystemEventArgs args)
    {
        if (!ZNet.instance || !ZNet.instance.IsServer()) return;
        CreatureQuestsPlugin.CreatureQuestsLogger.LogDebug("Reloading raven quests...");
        Read();
    }

    private static void SetupServerWatch()
    {
        ServerData.ValueChanged += () =>
        {
            if (!ZNet.instance || !ZNet.instance.IsServer()) return;
            if (ServerData.Value.IsNullOrWhiteSpace()) return;
            try
            {
                var deserializer = new DeserializerBuilder().Build();
                var data = deserializer.Deserialize<List<List<string>>>(ServerData.Value);
                QuestBlocks = data;
                ParseData();
                CreatureQuestsPlugin.CreatureQuestsLogger.LogDebug("Loaded server quests");
            }
            catch
            {
                CreatureQuestsPlugin.CreatureQuestsLogger.LogDebug("Failed to parse server quest data");
            }
        };
    }

    private static void Read()
    {
        if (!Directory.Exists(RootFolder)) Directory.CreateDirectory(RootFolder);
        if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
        foreach (string file in Directory.GetFiles(FolderPath, "*.yml")) SplitQuestsIntoBlocks(file);
        ParseData();
        UpdateServer();
    }

    private static void ParseData()
    {
        if (QuestBlocks.Count <= 0) return;
        QuestManager.m_quests.Clear();
        foreach (List<string> block in QuestBlocks) CreateQuest(block);
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Awake))]
    private static class ZNet_Awake_Patch
    {
        private static void Postfix(ZNet __instance)
        {
            if (!__instance || !__instance.IsServer()) return;
            UpdateServer();
        }
    }

    private static void UpdateServer()
    {
        if (QuestBlocks.Count <= 0) return;
        ISerializer serializer = new SerializerBuilder().Build();
        ServerData.Value = serializer.Serialize(QuestBlocks);
    }
    
    private static void SplitQuestsIntoBlocks(string filePath)
    {
        if (!File.Exists(filePath)) return;
        string name = Path.GetFileNameWithoutExtension(filePath);
        if (name == "Defaults") return;
        List<string> data = File.ReadAllLines(filePath).ToList();

        // Find indices of lines with quest block headers (lines like "[QuestName]")
        List<int> blockIndices = new List<int>();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].StartsWith("[") && data[i].EndsWith("]"))
            {
                blockIndices.Add(i);
            }
        }

        // Use indices to create sub-lists for each quest block
        for (int i = 0; i < blockIndices.Count; i++)
        {
            int start = blockIndices[i];
            int end = i + 1 < blockIndices.Count ? blockIndices[i + 1] : data.Count;

            // Get lines from the start of the block to just before the next block or end of file
            List<string> block = data.GetRange(start, end - start);
            QuestBlocks.Add(block);
        }
    }

    private static void WriteDefaults()
    {
        if (!Directory.Exists(RootFolder)) Directory.CreateDirectory(RootFolder);
        if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
        List<string> data = new();
        foreach (QuestManager.Quest quest in QuestManager.m_quests.Values)
        {
            data.AddRange(FormatQuests(quest));
            data.Add("");
        }
        File.WriteAllLines(DefaultFilePath, data);
    }

    private static void CreateQuest(IReadOnlyList<string> block)
    {
        try
        {
            string title = block[0].Replace("[", string.Empty).Replace("]", string.Empty);
            var titleParts = title.Split('.');
            var questIndex = int.Parse(titleParts[0]);
            string name = titleParts[1];
            QuestManager.Quest quest = new QuestManager.Quest(name, questIndex);
            foreach (string line in block)
            {
                string[] parts = line.Split(':');
                string type = parts[0].ToLower();
                switch (type)
                {
                    case "description":
                        quest.Description = parts[1].Trim();
                        break;
                    case "creatureform":
                        quest.RequiredShapeshiftForm = parts[1].Trim();
                        break;
                    case "requiredkey":
                        quest.RequiredDefeatKey = parts[1].Trim();
                        break;
                    case "duration":
                        quest.Duration = float.Parse(parts[1]);
                        break;
                    case "questtype":
                        Helpers.ParseParentheses(parts[1].Trim(), out string questType, out string questValues);
                        if (!Enum.TryParse(questType, true, out QuestManager.Quest.QuestData.QuestType QuestType)) break;
                        quest.Data.Type = QuestType;
                        switch (quest.Data.Type)
                        {
                            case QuestManager.Quest.QuestData.QuestType.Run or QuestManager.Quest.QuestData.QuestType.Jump or QuestManager.Quest.QuestData.QuestType.Fly or QuestManager.Quest.QuestData.QuestType.Attach or QuestManager.Quest.QuestData.QuestType.Travel:
                                quest.Data.Amount = int.TryParse(questValues.Trim(), out int questAmount)
                                    ? questAmount
                                    : 1;
                                break;
                            default:
                                var questParts = questValues.Split(',');
                                var prefab = questParts[0].Trim();
                                if (!int.TryParse(questParts[1].Trim(), out int amount)) break;
                                if (!int.TryParse(questParts[2].Trim(), out int level)) break;
                                quest.Data.PrefabName = prefab;
                                quest.Data.Amount = amount;
                                quest.Data.Level = level;
                                break;
                        }

                        break;
                    case "reward":
                        var rewards = parts[1].Split('|');
                        foreach (var reward in rewards)
                        {
                            Helpers.ParseParentheses(reward.Trim(), out string rewardType, out string rewardValue);
                            if (!Enum.TryParse(rewardType, true, out QuestManager.Quest.RewardData.RewardType RewardType)) break;
                            switch (RewardType)
                            {
                                case QuestManager.Quest.RewardData.RewardType.Item:
                                    var itemValues = rewardValue.Split(',');
                                    var itemName = itemValues[0].Trim();
                                    if (!int.TryParse(itemValues[1].Trim(), out int itemAmount)) break;
                                    if (!int.TryParse(itemValues[2].Trim(), out int itemQuality)) break;
                                    int itemVariant = 0;
                                    if (itemValues.Length > 3)
                                    {
                                        if (!int.TryParse(itemValues[3].Trim(), out itemVariant)) break;
                                    }
                                    quest.Rewards.Set(itemName, itemAmount, itemQuality, itemVariant);
                                    break;
                                case QuestManager.Quest.RewardData.RewardType.Skill:
                                    var skillValues = rewardValue.Split(',');
                                    var skill = skillValues[0].Trim();
                                    if (!float.TryParse(skillValues[1], out float skillAmount)) break;
                                    if (!Enum.TryParse(skill, true, out Skills.SkillType SkillType)) break;
                                    quest.Rewards.Set(SkillType, skillAmount);
                                    break;
                                case QuestManager.Quest.RewardData.RewardType.Spawn:
                                    
                                    break;
                            }
                        }
                        break;
                    case "start" or "cancel" or "completed":
                        var dialogueParts = parts[1].Split('|');
                        string acceptText = dialogueParts[0].Trim();
                        string cancelText = "";
                        if (dialogueParts.Length > 1) cancelText = dialogueParts[2].Trim();
                        if (cancelText.IsNullOrWhiteSpace())
                        {
                            if (type == "start") quest.AddStartText(acceptText);
                            else if(type == "cancel") quest.AddCancelText(acceptText);
                            else if(type == "completed") quest.AddCompletedText(acceptText);
                        }
                        else
                        {
                            if (type == "start") quest.AddStartAction(acceptText, cancelText);
                            else if (type == "cancel") quest.AddCancelAction(acceptText, cancelText);
                            else if (type == "completed") quest.AddCompletedAction(acceptText,cancelText);
                        }
                        break;
                    case "event":
                        var eventParts = parts[1].Split('|');
                        foreach (var part in eventParts)
                        {
                            var creatureParts = part.Split(',');
                            var eventPrefab = creatureParts[0].Trim();
                            int min = int.TryParse(creatureParts[1].Trim(), out int eventPrefabMinLevel) ? eventPrefabMinLevel : 1;
                            int max = int.TryParse(creatureParts[2].Trim(), out int eventPrefabMaxLevel)
                                ? eventPrefabMaxLevel
                                : 3;
                            quest.Data.AddEventSpawn(eventPrefab, min, max);
                        }
                        break;
                    case "eventduration":
                        quest.Data.EventDuration = float.TryParse(parts[1].Trim(), out float dur) ? dur : 100f;
                        break;
                }
            }
        }
        catch
        {
            CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning("Failed to parse quest: " + block[0]);
        }
    }

    private static List<string> FormatQuests(QuestManager.Quest info)
    {
        List<string> formattedData = new();
        formattedData.Add($"[{info.Index}.{info.Name}]");
        formattedData.Add($"Description: {info.Description}");
        formattedData.Add($"CreatureForm: {info.RequiredShapeshiftForm}");
        formattedData.Add($"Duration: {info.Duration}");
        if (!info.RequiredDefeatKey.IsNullOrWhiteSpace())
        {
            formattedData.Add($"RequiredKey: {info.RequiredDefeatKey}");
        }
        if (info.Data.EventData.Count > 0)
        {
            string eventString = "Event: ";
            for (var index = 0; index < info.Data.EventData.Count; index++)
            {
                var creature = info.Data.EventData[index];
                eventString += $"{creature.PrefabName}, {creature.MinLevel}, {creature.MaxLevel}";
                if (index < info.Data.EventData.Count - 1)
                {
                    eventString += " | ";
                }
            }
            formattedData.Add(eventString);
            formattedData.Add($"EventDuration: {info.Data.EventDuration}");
        }
        switch (info.Data.Type)
        {
            case QuestManager.Quest.QuestData.QuestType.Bounty:
                formattedData.Add($"QuestType: {info.Data.Type}({info.Data.PrefabName}, {info.Data.BountyData.m_name}, {info.Data.BountyData.m_level}, {info.Data.BountyData.m_health}, {info.Data.BountyData.m_scale})");
                break;
            case QuestManager.Quest.QuestData.QuestType.Run or QuestManager.Quest.QuestData.QuestType.Jump or QuestManager.Quest.QuestData.QuestType.Fly or QuestManager.Quest.QuestData.QuestType.Attach or QuestManager.Quest.QuestData.QuestType.Travel:
                formattedData.Add($"QuestType: {info.Data.Type}({info.Data.Amount})");
                break;
            default:
                formattedData.Add($"QuestType: {info.Data.Type}({info.Data.PrefabName}, {info.Data.Amount}, {info.Data.Level})");
                break;
        }

        foreach (var reward in info.Rewards.List)
        {
            switch (reward.Type)
            {
                case QuestManager.Quest.RewardData.RewardType.Item:
                    formattedData.Add($"Reward: {reward.Type}({reward.PrefabName}, {reward.Amount}, {reward.Quality}, {reward.Variant})");
                    break;
                case QuestManager.Quest.RewardData.RewardType.Skill:
                    formattedData.Add($"Reward: {reward.Type}({reward.SkillType}, {reward.ExperienceAmount})");
                    break;
            }
        }
        foreach (var text in info.m_startQueue)
        {
            if (text.m_action is not null)
            {
                string dialogue = $"Start: {text.m_acceptText} | {text.m_cancelText} | Command(Accept)" ;
                formattedData.Add(dialogue);
            }
            else
            {
                string dialogue = $"Start: {text.m_acceptText}";
                formattedData.Add(dialogue);
            }
        }

        foreach (var text in info.m_cancelQueue)
        {
            if (text.m_action is not null)
            {
                string dialogue = $"Cancel: {text.m_acceptText} | {text.m_cancelText} | Command(Cancel)" ;
                formattedData.Add(dialogue);
            }
            else
            {
                string dialogue = $"Cancel: {text.m_acceptText}";
                formattedData.Add(dialogue);
            }
        }

        foreach (var text in info.m_completedQueue)
        {
            if (text.m_action is not null)
            {
                string dialogue = $"Completed: {text.m_acceptText} | {text.m_cancelText} | Command(Collect)" ;
                formattedData.Add(dialogue);
            }
            else
            {
                string dialogue = $"Completed: {text.m_acceptText}";
                formattedData.Add(dialogue);
            }
        }
        return formattedData;
    }
    
    // [20.Moonburn]
    // Description: Cast out from the firelit caves, you prowl the peaks with frozen fury. As Fenring, hunt the cultists who turned fang against fang and scorched the old ways.
    // CreatureForm: Fenring
    // Duration: 1000
    // RequiredKey: defeated_eikthyr
    // Event: Fenring_Cultist, 1, 3
    // EventDuration: 1000
    // QuestType: Kill(Fenring_Cultist, 10, 1)
    // Reward: Item(Shapeshift_Fenring_item, 2, 1, 0)
    // Start: They called you feral. You called them kin.
    // Start: Now their fire burns where the moon once ruled. Take back the night.
    // Start: They’ll bleed for every howl they silenced. | Not yet. The hunt waits. | Command(Accept)
    // Cancel: Their flames still crackle in your place. The caves remain cursed.
    // Cancel: Leave vengeance buried? | Even wolves can fear fire. | Command(Cancel)
    // Completed: The cultists lie broken. The cold returns to your caves.
    // Completed: No more fire chants. No more exile. The mountain howls with you again.
    // Completed: Take this, moonborn avenger. | Return when old scars reopen. | Command(Collect)
}