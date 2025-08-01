using CreatureQuests.QuestSystem;
using HarmonyLib;
using Shapeshift.Source;

namespace Shapeshift.QuestSystem.Tracking;

public static class Tame
{
    [HarmonyPatch(typeof(Tameable), nameof(Tameable.Tame))]
    private static class Tameable_Tame_Patch
    {
        private static void Postfix(Tameable __instance)
        {
            if (QuestManager.m_currentQuest is null) return;
            if (!QuestManager.m_currentQuest.IsCorrectType(QuestManager.Quest.QuestData.QuestType.Tame)) return;
            if (!QuestManager.m_currentQuest.IsMatchingPrefab(__instance.name) || QuestManager.m_currentQuest.Data.Level > __instance.m_character.m_level) return;
            if (!QuestManager.m_currentQuest.HasRequirements()) return;
            var closestPlayer = Player.GetClosestPlayer(__instance.transform.position, 10f);
            if (closestPlayer == null) return;
            QuestManager.m_currentQuest.IncreaseProgress();
        }
    }
}