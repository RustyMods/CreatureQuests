using System.Collections.Generic;
using System.Text;
using BepInEx;
using Shapeshift.QuestSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CreatureQuests.QuestSystem.Behaviors;

public class QuestRaven : MonoBehaviour, Hoverable, Interactable, IDestructible
{
    public static QuestManager.Quest? m_questData;
    public static QuestRaven? m_instance;
    public bool m_active;
    
    public string m_name = "Översyn";
    public GameObject m_visual = null!;
    public GameObject m_exclamation = null!;
    public float m_idleEffectIntervalMin = 30f;
    public float m_idleEffectIntervalMax = 60f;
    public float m_spawnDistance = 20f;
    public float m_enemyCheckDistance = 4f;
    public float m_rotateSpeed = 70f;
    public float m_minRotationAngle = 30f;
    public float m_longDialogVisibleTime = 60f;
    public float m_textOffset = 1.4f;
    public float m_textCullDistance = 20f;
    public EffectList m_idleEffect = new();
    public EffectList m_deSpawnEffect = new();
    public Animator m_animator = null!;
    private QuestManager.Quest.Dialogue? m_currentDialogue;

    private static readonly List<string> m_randomTalk = new()
    {
        "Kaw! Got any snacks?",
        "Odin told me to keep an eye on you... literally!",
        "Ever seen a bird this stylish? Kaw!",
        "Flying around is exhausting. Got a place to sit?",
        "Kaw! I saw you trip back there. Didn’t think I noticed, huh?",
        "Rumor has it you’re a hero. Don’t let it go to your head!",
        "Need some advice? Kaw! Too bad, I got none.",
        "Kaw! Kaw! Does this echo? …Anyone?",
        "I could fly all day... but I’d rather watch you.",
        "You’d be lost without me. Admit it!",
        "Ever think about taking a bird break? Kaw!",
        "Odin says he’s watching you. So am I... closer!",
        "Kaw! This is my good side. No, wait, this one!",
        "Ever wonder why the sky is blue? I don’t. Kaw!",
        "Kaw! Do you talk to animals often, or am I special?",
        "You call that a sword? Kaw!",
        "Kaw! Wanna trade? I got... feathers.",
        "Hope you don’t mind the mess. Odin’s messy too!",
        "Kaw! I saw what you did... Odin’s impressed!"
        
    };

    private static readonly List<string> m_wrongBiomeTalk = new()
    {
        "Kaw! The wind carries no omens here—seek the",
        "This ground holds no truth for you. Fly to the",
        "Kraaah... Odin watches elsewhere. Begin in the",
        "Your fate waits beyond these trees—in the",
        "Foolish hatchling, your trial starts not here, but in the",
        "Even the roots whisper: this is not the place. Go to the",
        "Do not waste your talons on this soil. The answer lies in the",
        "The Allfather’s eye turns toward the",
        "This land is silent to your call. The challenge breathes in the",
        "You flap aimlessly—fly to where your story begins: the",
        "A shadow without purpose. Find your path in the",
        "Kaw! Kaw! Wrong sky, wrong stone—begin in the",
        "Your spirit stirs, but this realm is still. Seek the",
        "You peck at barren land. The harvest grows in the",
        "Kreee... I tire of watching you flounder. Go to the",
    };

    private static readonly List<string> m_goodbyeTalk = new()
    {
        "Kaw! Till next time!",
        "Winds guide you!",
        "Feathers to the sky!",
        "The Allfather watches.",
        "Gone, like shadow on snow.",
        "May the storm favor you!",
        "Eyes in the sky, always.",
        "Krrk! Vanishing winds!",
        "Kaw! Fate calls us onward!",
        "We fly where tales are born.",
        "Our wings remember.",
        "Back to the branches!",
        "Skies beckon us!"
    };

    private static readonly List<string> m_needDefeatedKeyTalk = new()
    {
        "Come back when the blood of  ",
        "Return when the shadows have passed over ",
        "Only then will the way be clear beyond ",
        "The path lies hidden until the fate of  ",
        "Your journey is not complete without crossing ",
        "When silence falls over , seek me again",
        "The truth waits beyond the fall of ",
        "Speak again after the winds have carried away ",
        "The veil will lift once the echoes of  ",
        "No passage until the darkness yields to "
    };


    public void Awake()
    {
        transform.position = new Vector3(0.0f, 100000f, 0.0f);
        m_instance = this;
        m_animator = m_visual.GetComponentInChildren<Animator>();
        InvokeRepeating(nameof(IdleEffect),
            Random.Range(m_idleEffectIntervalMin, m_idleEffectIntervalMax),
            Random.Range(m_idleEffectIntervalMin, m_idleEffectIntervalMax));
        InvokeRepeating(nameof(CheckSpawn), 1f, 1f);
    }

    public void OnDestroy()
    {
        if (m_instance != this) return;
        m_instance = null;
    }

    public string GetHoverText()
    {
        if (!IsSpawned()) return "";
        if (m_questData is null && QuestManager.m_currentQuest is null) return Localization.instance.Localize(m_name + "\n[<color=yellow><b>$KEY_Use</b></color>] $raven_interact");
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(m_name);
        if (QuestManager.m_currentQuest != null)
        {
            if (QuestManager.m_currentQuest.IsCompleted())
            {
                if (QuestManager.m_currentQuest.m_completedQueue.Count == 0) return Localization.instance.Localize(m_name + "\n[<color=yellow><b>$KEY_Use</b></color>] $raven_interact");
                var dialogue = QuestManager.m_currentQuest.m_completedQueue.Peek();
                if (dialogue.m_action != null)
                {
                    stringBuilder.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $raven_collect");
                    stringBuilder.Append("\n[<color=yellow><b>L.Shift + $KEY_Use</b></color>] $raven_refuse");
                }
                else
                {
                    stringBuilder.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $raven_continue");
                }
            }
            else
            {
                if (QuestManager.m_currentQuest.m_cancelQueue.Count == 0) return Localization.instance.Localize(m_name + "\n[<color=yellow><b>$KEY_Use</b></color>] $raven_interact");
                var dialogue = QuestManager.m_currentQuest.m_cancelQueue.Peek();
                if (dialogue.m_action != null)
                {
                    stringBuilder.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $raven_keep");
                    stringBuilder.Append("\n[<color=yellow><b>L.Shift + $KEY_Use</b></color>] $raven_cancel");
                }
                else
                {
                    stringBuilder.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $raven_continue");
                }
            }
        }
        else
        {
            if (m_questData is null) return Localization.instance.Localize(m_name + "\n[<color=yellow><b>$KEY_Use</b></color>] $raven_interact");
            if (m_questData.m_startQueue.Count == 0) return Localization.instance.Localize(m_name + "\n[<color=yellow><b>$KEY_Use</b></color>] $raven_interact");
            var dialogue = m_questData.m_startQueue.Peek();
            if (dialogue?.m_action != null)
            {
                stringBuilder.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $raven_accept");
                stringBuilder.Append("\n[<color=yellow><b>L.Shift + $KEY_Use</b></color>] $raven_decline");
            }
            else
            {
                stringBuilder.Append("\n[<color=yellow><b>$KEY_Use</b></color>] $raven_continue");
            }
        }

        return Localization.instance.Localize(stringBuilder.ToString());
    }

    public string GetHoverName() => Localization.instance.Localize(m_name);

    public bool Interact(Humanoid character, bool hold, bool alt)
    {
        if (hold) return false;
        if (Chat.instance.IsDialogVisible(gameObject)) Chat.instance.ClearNpcText(gameObject);
        Talk(alt);
        return false;
    }

    public void Talk(bool alt)
    {
        if (!Player.m_localPlayer) return;
        if (QuestManager.m_currentQuest == null)
        {
            m_questData ??= QuestManager.GetQuest();
            if (m_questData == null)
            {
                RandomTalk(m_randomTalk);
                return;
            }

            if (m_questData.RequiredBiome != Heightmap.Biome.None && m_questData.RequiredBiome != Player.m_localPlayer.GetCurrentBiome())
            { 
                RandomTalk(m_wrongBiomeTalk, m_questData.RequiredBiome);
                return;
            }

            if (!m_questData.RequiredDefeatKey.IsNullOrWhiteSpace() && !m_questData.HasRequiredKey())
            {
                RandomTalk(m_needDefeatedKeyTalk, m_questData.RequiredDefeatCharacterSharedName);
                return;
            }
            if (m_questData.m_startQueue.Count == 0) return;
            QuestManager.Quest.Dialogue dialogue = m_questData.m_startQueue.Dequeue();
            Say(dialogue, alt);
            m_questData.m_startQueue.Enqueue(dialogue);
            dialogue.m_action?.Invoke(alt);
        }
        else
        {
            if (QuestManager.m_currentQuest.IsCompleted())
            {
                if (QuestManager.m_currentQuest.m_completedQueue.Count == 0) return;
                var dialogue = QuestManager.m_currentQuest.m_completedQueue.Dequeue();
                Say(dialogue);
                QuestManager.m_currentQuest.m_completedQueue.Enqueue(dialogue);
                if (!alt) dialogue.m_action?.Invoke(alt);
            }
            else
            {
                if (QuestManager.m_currentQuest.m_cancelQueue.Count == 0) return;
                var dialogue = QuestManager.m_currentQuest.m_cancelQueue.Dequeue();
                Say(dialogue);
                QuestManager.m_currentQuest.m_cancelQueue.Enqueue(dialogue);
                dialogue.m_action?.Invoke(alt);
            }
        }
    }
    
    public void RandomTalk(List<string> texts)
    {
        string text = texts[Random.Range(0, texts.Count)];
        Say(new QuestManager.Quest.Dialogue(text));
    }

    public void RandomTalk(List<string> texts, Heightmap.Biome biome)
    {
        string text = texts[Random.Range(0, texts.Count)];
        if (biome is not Heightmap.Biome.None)
        {
            text += $" {biome.ToString().ToLower()}";
        }
        Say(new QuestManager.Quest.Dialogue(text));
    }
    
    public void RandomTalk(List<string> texts, string defeatedCharacterSharedName)
    {
        string text = texts[Random.Range(0, texts.Count)] + Localization.instance.Localize(defeatedCharacterSharedName);
        Say(new QuestManager.Quest.Dialogue(text));
    }

    public void Say(QuestManager.Quest.Dialogue dialogue, bool alt = false)
    {
        Chat.instance.SetNpcText(gameObject, Vector3.up * m_textOffset, m_textCullDistance, m_longDialogVisibleTime, m_name, alt ? dialogue.m_cancelText : dialogue.m_acceptText, true);
        m_animator.SetTrigger("talk");
        m_currentDialogue = dialogue;
    }
    
    public bool UseItem(Humanoid user, ItemDrop.ItemData item) => false;

    public void IdleEffect()
    {
        if (!IsSpawned()) return;
        Transform transform1 = transform;
        m_idleEffect.Create(transform1.position, transform1.rotation);
        CancelInvoke(nameof(IdleEffect));
        InvokeRepeating(nameof(IdleEffect),
            Random.Range(m_idleEffectIntervalMin, m_idleEffectIntervalMax),
            Random.Range(m_idleEffectIntervalMin, m_idleEffectIntervalMax));
    }
    public void Update()
    {
        if (!IsAway() && !IsFlying() && Player.m_localPlayer)
        {
            UpdateLookTowardsPlayer();
        }

        if (IsSpawned())
        {
            m_exclamation.SetActive(QuestManager.m_currentQuest is null or { m_completed: true } && QuestManager.HasAvailableQuests());
        }
        else
        {
            m_exclamation.SetActive(false);
        }
    }

    private void UpdateLookTowardsPlayer()
    {
        var vector3 = (Player.m_localPlayer.transform.position - transform.position) with
        {
            y = 0.0f
        };
        vector3.Normalize();
        var f = Vector3.SignedAngle(transform.forward, vector3, Vector3.up);
        if (Mathf.Abs(f) > (double)m_minRotationAngle)
        {
            m_animator.SetFloat("anglevel", m_rotateSpeed * Mathf.Sign(f), 0.4f, Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(vector3), Time.deltaTime * m_rotateSpeed);
        }
        else
        {
            m_animator.SetFloat("anglevel", 0.0f, 0.4f, Time.deltaTime);
        }
    }

    private bool FindSpawnPoint(out Vector3 point, out GameObject? landOn)
    {
        var position = Player.m_localPlayer.transform.position;
        var forward = Utils.GetMainCamera().transform.forward with
        {
            y = 0.0f
        };
        forward.Normalize();
        point = new Vector3(0.0f, -999f, 0.0f);
        landOn = null;
        bool spawnPoint = false;
        for (int index = 0; index < 20; ++index)
        {
            Vector3 vector3 = Quaternion.Euler(0.0f, Random.Range(-30, 30), 0.0f) * forward;
            Vector3 p = position + vector3 * Random.Range(m_spawnDistance - 5f, m_spawnDistance);
            if (ZoneSystem.instance.GetSolidHeight(p, out float height, out Vector3 normal, out GameObject? go) && height > 30.0 &&
                height > (double)point.y && height < 2000.0 && normal.y > 0.5 &&
                Mathf.Abs(height - position.y) < 2.0)
            {
                p.y = height;
                point = p;
                landOn = go;
                spawnPoint = true;
            }
        }

        return spawnPoint;
    }

    private bool EnemyNearby(Vector3 point) => LootSpawner.IsMonsterInRange(point, m_enemyCheckDistance);
    private bool InState(string state)
    {
        if (!m_animator.isInitialized) return false;
        AnimatorStateInfo animatorStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsTag(state)) return true;
        animatorStateInfo = m_animator.GetNextAnimatorStateInfo(0);
        return animatorStateInfo.IsTag(state);
    }

    public void FlyAway()
    {
        Chat.instance.ClearNpcText(gameObject);
        m_animator.SetTrigger(IsUnderRoof() ? "poff" : "flyaway");
        m_currentDialogue = null;
    }

    public void CheckSpawn()
    {
        if (!Player.m_localPlayer) return;
        if (IsSpawned() && !m_active)
        {
            FlyAway();
        }
        else if (!IsSpawned() && m_active)
        {
            if (!IsAway() || EnemyNearby(transform.position) || RandEventSystem.InEvent()) return;
            Spawn(false);
        }
        else if (IsSpawned() && m_currentDialogue == null)
        {
            RandomTalk(m_randomTalk, Heightmap.Biome.None);
        }
        else if (IsSpawned() && m_active)
        {
            var distance = Vector3.Distance(Player.m_localPlayer.transform.position, transform.position);
            if (distance > 20f)
            {
                FlyAway();
            }
        }
    }

    public DestructibleType GetDestructibleType() => DestructibleType.Character;

    public void Damage(HitData hit)
    {
        if (!IsSpawned()) return;
        FlyAway();
        RestartSpawnCheck(4f);
        Game.instance.IncrementPlayerStat(PlayerStatType.RavenHits);
    }

    public void RestartSpawnCheck(float delay)
    {
        CancelInvoke(nameof(CheckSpawn));
        InvokeRepeating(nameof(CheckSpawn), delay, 1f);
    }

    public bool IsSpawned() => InState("visible");
    private bool IsAway() => InState("away");
    private bool IsFlying() => InState("flying");
    
    public void Spawn(bool forceTeleport)
    {
        if (Utils.GetMainCamera() is null) return;
        if (!FindSpawnPoint(out Vector3 point, out GameObject? _)) return;

        Transform transform1 = transform;
        transform1.position = point;
        Vector3 forward = (Player.m_localPlayer.transform.position - transform1.position) with { y = 0.0f };
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);
        if (forceTeleport) m_animator.SetTrigger("teleportin");
        else m_animator.SetTrigger(IsUnderRoof() ? "teleportin" : "flyin");
    }
    public static void ToggleFly(bool enable)
    {
        if (!CreatureQuestsPlugin.m_raven || !Player.m_localPlayer) return;
        if (!enable)
        {
            if (m_instance is null) return;
            m_instance.m_active = false;
        }
        else
        {
            if (m_instance is null)
            {
                GameObject raven = Instantiate(CreatureQuestsPlugin.m_raven, Player.m_localPlayer.transform.position, Quaternion.identity);
                if (Utils.FindChild(raven.transform, "Jaw") is { } jaw)
                {
                    GameObject thirdEye = Instantiate(CreatureQuestsPlugin.m_thirdEye, jaw);
                    thirdEye.transform.localPosition = new Vector3(0.0004f, 0.0033f, 0.0116f);
                    thirdEye.transform.localRotation = new Quaternion(-51.949f, 134.846f, 61.205f, 0f);
                    thirdEye.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                }
            }
        
            if (m_instance is null) return;
            m_instance.m_active = true;
        }
    }

    private bool IsUnderRoof() => Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.up, 20f, LayerMask.GetMask("Default", "static_solid", "piece"));
}