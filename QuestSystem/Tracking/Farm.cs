using CreatureQuests.QuestSystem;
using HarmonyLib;

namespace Shapeshift.QuestSystem.Tracking;

public static class Farm
{
    [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
    private static class Player_PlacePiece_Patch
    {
        private static void Postfix(Player __instance, Piece piece)
        {
            if (__instance != Player.m_localPlayer) return;
            if (QuestManager.m_currentQuest == null) return;
            if (!QuestManager.m_currentQuest.IsCorrectType(QuestManager.Quest.QuestData.QuestType.Farm)) return;
            if (!QuestManager.m_currentQuest.IsMatchingPrefab(piece.name)) return;
            if (!QuestManager.m_currentQuest.HasRequirements()) return;
            QuestManager.m_currentQuest.IncreaseProgress();

        }
    }
    
    
    // [HarmonyPatch(typeof(Plant), nameof(Plant.Awake))]
    // private static class Plant_Awake_Patch
    // {
    //     private static void Postfix(Plant __instance)
    //     {
    //         if (!__instance || !__instance.m_nview.IsValid()) return;
    //         __instance.m_nview.Register<string, long>(nameof(RPC_CheckQuest),RPC_CheckQuest);
    //     }
    // }
    //
    // private static void RPC_CheckQuest(long sender, string prefabName, long uid)
    // {
    //     if (QuestManager.m_currentQuest == null) return;
    //     if (!QuestManager.m_currentQuest.IsCorrectType(QuestManager.Quest.QuestData.QuestType.Farm)) return;
    //     if (!QuestManager.m_currentQuest.IsMatchingPrefab(prefabName)) return;
    //     if (uid != Player.m_localPlayer.GetPlayerID()) return;
    //     if (!QuestManager.m_currentQuest.IsCorrectCreature()) return;
    //     QuestManager.m_currentQuest.IncreaseProgress();
    // }
    //
    // [HarmonyPatch(typeof(Plant), nameof(Plant.Grow))]
    // private static class Plant_Grow_Patch
    // {
    //     private static void Prefix(Plant __instance)
    //     {
    //         // If not the owner of the object, do nothing
    //         if (!__instance.m_nview.IsOwner()) return;
    //         // Tell everyone to check quest
    //         string name = __instance.name.Replace("(Clone)", string.Empty);
    //         long creator = __instance.m_nview.GetZDO().GetLong(ZDOVars.s_creator);
    //         __instance.m_nview.InvokeRPC(nameof(RPC_CheckQuest), name, creator);
    //         // // Tell yourself to check quest
    //         // RPC_CheckQuest(0L, name, creator);
    //     }
    // }
    
}