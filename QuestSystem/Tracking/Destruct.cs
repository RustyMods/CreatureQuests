using CreatureQuests.QuestSystem;
using HarmonyLib;

namespace Shapeshift.QuestSystem.Tracking;

public static class Destruct
{
    [HarmonyPatch(typeof(Destructible), nameof(Destructible.Destroy))]
    private static class Destructible_Destroy_Patch
    {
        private static void Prefix(Destructible __instance, HitData? hit)
        {
            if (QuestManager.m_currentQuest is not { Data.Type: QuestManager.Quest.QuestData.QuestType.Destructible } quest || hit is null) return;
            if (hit.GetAttacker() is not { } attacker || attacker != Player.m_localPlayer) return;
            if (!quest.IsMatchingPrefab(__instance.name)) return;
            if (!quest.HasRequirements()) return;
            quest.IncreaseProgress();
        }
    }
}