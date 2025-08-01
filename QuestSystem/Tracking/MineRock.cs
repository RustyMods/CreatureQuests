using CreatureQuests.QuestSystem;
using HarmonyLib;
using Shapeshift.Source;

namespace Shapeshift.QuestSystem.Tracking;

public static class MineRock
{
    [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.Damage))]
    private static class MineRock5_Damage_Patch
    {
        private static void Postfix(MineRock5 __instance, HitData? hit)
        {
            if (QuestManager.m_currentQuest is not { Data.Type: QuestManager.Quest.QuestData.QuestType.Destructible } quest || hit is null) return;
            if (hit.GetAttacker() is not { } attack || attack != Player.m_localPlayer) return;
            if (!quest.IsMatchingPrefab(__instance.m_name, true)) return;
            if (!QuestManager.m_currentQuest.HasRequirements()) return;
            quest.IncreaseProgress();
        }
    }
}