using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CreatureQuests.QuestSystem;

public class EventSystem : MonoBehaviour
{
    public Player m_player = null!;
    public int m_maxSpawns = 5;
    public float m_updateInterval = 30f;
    public float m_pinRadius = 100f;
    
    public bool m_active;
    public float m_timer;
    public List<GameObject> m_spawnedCreatures = new();
    public Minimap.PinData? m_eventPin;
    public void Awake()
    {
        m_player = GetComponent<Player>();
    }

    public void Start()
    {
        InvokeRepeating(nameof(UpdateEvent), 1f, m_updateInterval);
    }

    public void OnDestroy()
    {
        RemovePin();
        foreach (var spawn in m_spawnedCreatures)
        {
            if (spawn == null) continue;
            if (!spawn.TryGetComponent(out ZNetView znv)) continue;
            znv.ClaimOwnership();
            znv.Destroy();
        }
    }

    public void Update()
    {
        if (m_player.IsDead() || QuestManager.m_currentQuest is not { m_activated: true } quest)
        {
            StopEvent();
            return;
        }
        if (m_active)
        {
            UpdateEventPin();
            return;
        }
        if (quest.Data.EventData.Count <= 0) return;
        if (quest.Data.EventDuration <= 0f) return;
        
        TriggerEvent();
    }

    public void UpdateEvent()
    {
        if (!m_active) return;
        m_timer += Time.deltaTime;
        if (QuestManager.m_currentQuest is not { } quest || m_timer > quest.Data.EventDuration || quest.IsCompleted())
        {
            RemovePin();
            StopEvent();
            m_timer = 0.0f;
            return;
        }
        Spawn(quest);
        AddPin();
    }

    public void Spawn(QuestManager.Quest data)
    {
        if (m_spawnedCreatures.Count >= m_maxSpawns) return;
        var creature = data.Data.EventData[Random.Range(0, data.Data.EventData.Count)];
        if (ZNetScene.instance.GetPrefab(creature.PrefabName) is not { } prefab) return;
        var random = Random.insideUnitCircle * 10f;
        var pos = transform.position + new Vector3(random.x, 0f, random.y);
        ZoneSystem.instance.FindFloor(pos, out float height);
        pos.y = height + 0.1f;
        var spawn = Instantiate(prefab, pos, Quaternion.identity);
        if (!spawn.TryGetComponent(out ZNetView znv)) return;
        if (!spawn.TryGetComponent(out Character character)) return;
        if (!spawn.TryGetComponent(out BaseAI baseAI)) return;
        baseAI.SetHuntPlayer(true);
        baseAI.SetAlerted(true);
        baseAI.SetTargetInfo(m_player.GetZDOID());
        if (baseAI is MonsterAI monsterAI) monsterAI.SetEventCreature(true);
        character.SetLevel(Random.Range(creature.MinLevel, creature.MaxLevel));
        character.m_onDeath += () => m_spawnedCreatures.Remove(spawn);
        znv.GetZDO().Set(ZDOVars.s_spawnTime, ZNet.instance.GetTime().Ticks);
        m_spawnedCreatures.Add(spawn);
    }
    public void TriggerEvent() => m_active = true;
    public void StopEvent() => m_active = false;

    public void UpdateEventPin()
    {
        if (m_eventPin == null) return;
        m_eventPin.m_pos = transform.position;
    }

    public void AddPin()
    {
        if (m_eventPin != null) return;
        m_eventPin = Minimap.instance.AddPin(transform.position, Minimap.PinType.EventArea, "", false, false);
        m_eventPin.m_worldSize = m_pinRadius;
    }

    public void RemovePin()
    {
        if (m_eventPin == null) return;
        Minimap.instance.RemovePin(m_eventPin);
    }
}