using System;
using CreatureQuests.QuestSystem;
using HarmonyLib;
using UnityEngine;

namespace Shapeshift.QuestSystem.Behaviors;

public class Bounty : MonoBehaviour
{
    public static Bounty? m_instance;
    public static EffectList PreSpawnEffectList = null!;
    public static EffectList DoneSpawnEffectList = null!;

    public Character m_character = null!;
    public ZNetView m_nview = null!;
    public void Awake()
    {
        m_nview = GetComponent<ZNetView>();
        m_character = GetComponent<Character>();
        m_character.m_onDeath += OnDeath;
        m_nview.GetZDO().Persistent = false;
        m_nview.Register<bool>(nameof(RPC_SetBoss), RPC_SetBoss);
        m_nview.Register<float>(nameof(RPC_SetScale),RPC_SetScale);
        m_nview.Register<string>(nameof(RPC_SetFaction), RPC_SetFaction);
        m_nview.Register<string>(nameof(RPC_SetSharedName),RPC_SetSharedName);
        m_nview.Register<bool>(nameof(RPC_SetTameable),RPC_SetTameable);
        m_instance = this;
    }
    public void OnDestroy()
    {
        if (m_character.m_health > 0f)
        {
            if (QuestManager.m_currentQuest is { Data.Type: QuestManager.Quest.QuestData.QuestType.Bounty })
            {
                if (QuestManager.m_currentQuest.m_currentBounty != this) return;
                QuestManager.m_currentQuest.m_currentBounty = null;
                QuestManager.m_currentQuest.CancelQuest();
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Bounty Escaped");
            }
        }

        m_instance = null;
    }
    
    public void OnDeath()
    {
        m_instance = null;
        if (QuestManager.m_currentQuest == null) return;
        if (QuestManager.m_currentQuest.Data.Type is not QuestManager.Quest.QuestData.QuestType.Bounty) return;
        QuestManager.m_currentQuest.IncreaseProgress();
        QuestManager.m_currentQuest.m_currentBounty = null;
    }

    private void RPC_SetFaction(long sender, string faction)
    {
        if (!Enum.TryParse(faction, true, out Character.Faction newFaction)) return;
        m_character.m_faction = newFaction;
    }

    private void RPC_SetBoss(long sender, bool enable) => m_character.m_boss = enable;

    private void RPC_SetScale(long sender, float scale) => transform.localScale = new Vector3(scale, scale, scale);

    private void RPC_SetSharedName(long sender, string sharedName) => m_character.m_name = sharedName;

    private void RPC_SetTameable(long sender, bool enable)
    {
        if (!enable && m_character.TryGetComponent(out Tameable component)) Destroy(component);
    }

    public void ApplyModifiers(BountyModifiers data)
    {
        m_nview.InvokeRPC(nameof(RPC_SetBoss), true);
        m_nview.InvokeRPC(nameof(RPC_SetFaction), "Boss");
        m_nview.InvokeRPC(nameof(RPC_SetSharedName), data.m_name);
        m_nview.InvokeRPC(nameof(RPC_SetTameable), false);
        m_nview.InvokeRPC(nameof(RPC_SetScale), data.m_scale);
        
        m_character.m_faction = Character.Faction.Boss;
        m_character.m_boss = true;
        m_character.m_name = data.m_name;
        transform.localScale = new Vector3(data.m_scale, data.m_scale, data.m_scale);
        m_character.SetLevel(data.m_level);
        m_character.SetMaxHealth(data.m_health);
        m_character.m_baseAI.SetAlerted(true);
    }
    
    
    public static bool FindSpawnPoint(out Vector3 point)
    {
        var position = Player.m_localPlayer.transform.position;
        var forward = Utils.GetMainCamera().transform.forward with
        {
            y = 0.0f
        };
        forward.Normalize();
        point = new Vector3(0.0f, -999f, 0.0f);
        bool spawnPoint = false;
        for (int index = 0; index < 20; ++index)
        {
            Vector3 vector3 = Quaternion.Euler(0.0f, UnityEngine.Random.Range(-30, 30), 0.0f) * forward;
            Vector3 p = position + vector3 * UnityEngine.Random.Range(20f - 5f, 20f);
            if (ZoneSystem.instance.GetSolidHeight(p, out float height, out Vector3 normal, out GameObject? _) && height > 30.0 &&
                height > (double)point.y && height < 2000.0 && normal.y > 0.5 &&
                Mathf.Abs(height - position.y) < 2.0)
            {
                p.y = height;
                point = p;
                spawnPoint = true;
            }
        }

        return spawnPoint;
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    private static class ZNetScene_GetBountyAssets
    {
        private static void Postfix(ZNetScene __instance)
        {
            if (!__instance) return;
            GetEffects(__instance);
        }
    }

    private static void GetEffects(ZNetScene instance)
    {
        GetPreSpawnEffects(instance);
        GetSpawnEffects(instance);
    }

    private static void GetPreSpawnEffects(ZNetScene instance)
    {
        GameObject? VFX_PreSpawn = instance.GetPrefab("vfx_prespawn");
        GameObject? SFX_PreSpawn = instance.GetPrefab("sfx_prespawn");
        if (!VFX_PreSpawn || !SFX_PreSpawn) return;
        
        PreSpawnEffectList = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = VFX_PreSpawn,
                    m_enabled = true,
                    m_variant = -1
                },
                new EffectList.EffectData()
                {
                    m_prefab = SFX_PreSpawn,
                    m_enabled = true,
                    m_variant = -1,
                }
            }
        };
    }

    private static void GetSpawnEffects(ZNetScene instance)
    {
        GameObject? VFX_Spawn = instance.GetPrefab("vfx_spawn");
        GameObject? SFX_Spawn = instance.GetPrefab("sfx_spawn");
        if (!VFX_Spawn || !SFX_Spawn) return;

        DoneSpawnEffectList = new EffectList()
        {
            m_effectPrefabs = new[]
            {
                new EffectList.EffectData()
                {
                    m_prefab = VFX_Spawn,
                    m_enabled = true,
                    m_variant = -1
                },
                new EffectList.EffectData()
                {
                    m_prefab = SFX_Spawn,
                    m_enabled = true,
                    m_variant = -1
                }
            }
        };
    }

    [Serializable]
    public class BountyModifiers
    {
        public GameObject? m_critter;
        public Vector3 m_pos;
        public string m_name = "";
        public float m_health = 1f;
        public float m_scale = 1f;
        public int m_level = 1;
    }
}