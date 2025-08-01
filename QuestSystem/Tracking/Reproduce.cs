using CreatureQuests.QuestSystem;
using HarmonyLib;
using Shapeshift.Source;
using UnityEngine;

namespace Shapeshift.QuestSystem.Tracking;

public static class Reproduce
{
    [HarmonyPatch(typeof(Procreation), nameof(Procreation.Procreate))]
    private static class Procreation_Procreate_Patch
    {
        private static void Postfix(Procreation __instance)
        {
            if (QuestManager.m_currentQuest is null) return;
            if (__instance.IsPregnant()) return;
            if (!QuestManager.m_currentQuest.IsCorrectType(QuestManager.Quest.QuestData.QuestType.Procreate)) return;
            if (!QuestManager.m_currentQuest.IsMatchingPrefab(__instance.name) || QuestManager.m_currentQuest.Data.Level > __instance.m_character.m_level) return;
            if (!QuestManager.m_currentQuest.HasRequirements()) return;

            if (Random.value <= __instance.m_pregnancyChance || __instance.m_baseAI.IsAlerted() ||
                __instance.m_tameable.IsHungry() ||
                SpawnSystem.GetNrOfInstances(__instance.m_myPrefab, __instance.transform.position,
                    __instance.m_totalCheckRange) >= __instance.m_maxCreatures) return;

            var closestPlayer = Player.GetClosestPlayer(__instance.transform.position, 10f);
            if (closestPlayer == null) return;

            if (closestPlayer != Player.m_localPlayer) return;

            __instance.m_loveEffects.Create(__instance.transform.position, __instance.transform.rotation);
            int num = __instance.m_nview.GetZDO().GetInt(ZDOVars.s_lovePoints) + 1;
            __instance.m_nview.GetZDO().Set(ZDOVars.s_lovePoints, num, false);
            if (num < __instance.m_requiredLovePoints) return;
            
            __instance.m_nview.GetZDO().Set(ZDOVars.s_lovePoints, 0, false);
            __instance.MakePregnant();
            
            QuestManager.m_currentQuest.IncreaseProgress();
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "You've impregnated a " + __instance.m_character.m_name);
        }
    }
}