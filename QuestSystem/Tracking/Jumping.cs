using HarmonyLib;

namespace CreatureQuests.QuestSystem.Tracking;

public static class Jumping
{
    [HarmonyPatch(typeof(Player), nameof(Player.OnJump))]
    private static class Player_OnJump_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (__instance != Player.m_localPlayer) return;
            if (QuestManager.m_currentQuest == null) return;
            if (!QuestManager.m_currentQuest.IsCorrectType(QuestManager.Quest.QuestData.QuestType.Jump)) return;
            if (!QuestManager.m_currentQuest.HasRequirements()) return;
            QuestManager.m_currentQuest.IncreaseProgress();
        }
    }
}