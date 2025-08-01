using CreatureQuests.QuestSystem;
using HarmonyLib;
using Shapeshift.Source;

namespace Shapeshift.QuestSystem.Tracking;

public static class Kills
{
    [HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
    private static class Character_OnDeath_Patch
    {
        private static void Postfix(Character __instance)
        {
            if (__instance.IsPlayer()) return;
            if (!__instance.m_localPlayerHasHit) return;
            if (QuestManager.m_currentQuest is null) return;
            if (!QuestManager.m_currentQuest.IsCorrectType(QuestManager.Quest.QuestData.QuestType.Kill)) return;
            if (!QuestManager.m_currentQuest.IsMatchingPrefab(__instance.name) || QuestManager.m_currentQuest.Data.Level > __instance.m_level) return;
            if (!QuestManager.m_currentQuest.HasRequirements()) return;
            QuestManager.m_currentQuest.IncreaseProgress();
        }
    }
}