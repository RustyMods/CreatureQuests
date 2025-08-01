using HarmonyLib;
using UnityEngine;

namespace CreatureQuests.QuestSystem.Tracking;

public static class Flying
{
    [HarmonyPatch(typeof(Character), nameof(Character.UpdateMotion))]
    private static class Character_UpdateMotion_Patch
    {
        private static Vector3? s_lastPosition = null;
        private static float m_timer;

        private static void Postfix(Character __instance)
        {
            if (__instance == null || __instance != Player.m_localPlayer) return;
            if (QuestManager.m_currentQuest == null) return;
            if (!QuestManager.m_currentQuest.IsCorrectType(QuestManager.Quest.QuestData.QuestType.Fly)) return;
            if (!QuestManager.m_currentQuest.HasRequirements()) return;
            m_timer += Time.deltaTime;
            if (m_timer < 1f) return;
            m_timer = 0.0f;
            if (!__instance.IsFlying()) return;
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