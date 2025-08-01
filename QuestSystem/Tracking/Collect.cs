using HarmonyLib;

namespace CreatureQuests.QuestSystem.Tracking;

public static class Collect
{
    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    private static class Player_Awake_Patch
    {
        private static void Postfix(Player __instance)
        {
            if (!__instance != Player.m_localPlayer) return;
            __instance.GetInventory().m_onChanged += () =>
            {
                if (QuestManager.m_currentQuest is not {Data.Type: QuestManager.Quest.QuestData.QuestType.Collect or QuestManager.Quest.QuestData.QuestType.CollectItems} quest) return;
                if (!quest.HasRequirements()) return;
                switch (quest.Data.Type)
                {
                    case QuestManager.Quest.QuestData.QuestType.Collect:
                        int count = __instance.GetInventory().CountItems(QuestManager.m_currentQuest.Data.SharedName);
                        quest.SetProgress(count);
                        break;
                    case QuestManager.Quest.QuestData.QuestType.CollectItems:
                        int itemCount = 0;
                        foreach (var name in quest.Data.SharedNames)
                        {
                            if (__instance.IsKnownMaterial(name)) ++itemCount;
                        }
                        quest.SetProgress(itemCount);
                        break;
                }
            };
        }
    }
}