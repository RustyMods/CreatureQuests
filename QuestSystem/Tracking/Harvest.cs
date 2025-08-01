using HarmonyLib;

namespace CreatureQuests.QuestSystem.Tracking;

public static class Harvest
{
    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Interact))]
    private static class Pickable_Interact_Patch
    {
        private static void Postfix(Pickable __instance, ref bool __result)
        {
            if (!__result) return;
            if (QuestManager.m_currentQuest is null) return;
            if (!QuestManager.m_currentQuest.HasRequirements()) return;
            if (!QuestManager.m_currentQuest.IsCorrectType(QuestManager.Quest.QuestData.QuestType.Harvest)) return;
            if (!QuestManager.m_currentQuest.IsMatchingPrefab(__instance.name)) return;
            QuestManager.m_currentQuest.IncreaseProgress();
        }
    }
}