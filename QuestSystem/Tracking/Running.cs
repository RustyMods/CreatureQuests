using HarmonyLib;
using UnityEngine;

namespace CreatureQuests.QuestSystem.Tracking;

public static class Running
{
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    private static class Player_Update_Patch
    {
        private static Vector3? s_lastPosition = null;
        private static float m_timer;

        private static void Postfix(Player __instance)
        {
            if (__instance == null || __instance != Player.m_localPlayer) return;
            if (QuestManager.m_currentQuest == null) return;
            if (!QuestManager.m_currentQuest.IsCorrectType(QuestManager.Quest.QuestData.QuestType.Run)) return;
            if (!QuestManager.m_currentQuest.HasRequirements()) return;
            m_timer += Time.deltaTime;
            if (m_timer < 1f) return;
            m_timer = 0.0f;
            if (!__instance.IsRunning()) return;
            Vector3 currentPosition = __instance.transform.position;

            if (s_lastPosition.HasValue)
            {
                float distanceThisFrame = Vector3.Distance(s_lastPosition.Value, currentPosition);
                QuestManager.m_currentQuest.IncreaseProgress((int)distanceThisFrame);
            }

            s_lastPosition = currentPosition;
        }
    }

}