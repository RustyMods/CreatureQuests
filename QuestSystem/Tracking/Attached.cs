using HarmonyLib;

namespace CreatureQuests.QuestSystem.Tracking;

public static class Attached
{
    [HarmonyPatch(typeof(Attack), nameof(Attack.UpdateAttach))]
    private static class Attack_UpdateAttach_Patch
    {
        private static float m_timer;
        
        private static void Postfix(Attack __instance, float dt)
        {
            if (!__instance.m_isAttached) return;
            if (QuestManager.m_currentQuest == null) return;
            if (!QuestManager.m_currentQuest.IsCorrectType(QuestManager.Quest.QuestData.QuestType.Attach)) return;
            if (!QuestManager.m_currentQuest.HasRequirements()) return;
            m_timer += dt;
            if (m_timer < 1f) return;
            m_timer = 0.0f;
            QuestManager.m_currentQuest.IncreaseProgress();
        }
    }
}