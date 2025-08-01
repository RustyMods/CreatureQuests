using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Shapeshift.QuestSystem.Behaviors;

namespace CreatureQuests.QuestSystem;

public static class QuestCommands
{
    [HarmonyPatch(typeof(Terminal), nameof(Terminal.Awake))]
    private static class Terminal_Awake_Patch
    {
        private static void Postfix()
        {
            Terminal.ConsoleCommand command = new("ravenquest", "use help to discover available commands",
                (Terminal.ConsoleEventFailable)(args =>
                {
                    if (!Player.m_localPlayer || !ZNetScene.instance || !ZNetScene.instance.enabled)
                    {
                        CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning("Quest System not ready");
                        return false;
                    }

                    if (args.Length < 2)
                    {
                        CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning("Missing command parts");
                        return false;
                    }
                    switch (args[1])
                    {
                        case "help":
                            List<string> helpInfo = new()
                            {
                                "reset - clears player profile of quest progress",
                                "info - prints to console quest progress",
                                "list - prints to console registered quests"
                            };
                            foreach(string info in helpInfo) CreatureQuestsPlugin.CreatureQuestsLogger.LogInfo(info);
                            break;
                        case "reset":
                            Player.m_localPlayer.m_customData.Remove("RavenQuestData");
                            foreach (var quest in QuestManager.m_quests.Values)
                            {
                                quest.Reset();
                            }
                            QuestManager.m_currentQuest = null;
                            if (Bounty.m_instance != null) Bounty.m_instance.m_character.SetHealth(0f);
                            CreatureQuestsPlugin.CreatureQuestsLogger.LogDebug("Cleared raven quest data from player profile");
                            break;
                        case "list":
                            foreach (var quest in QuestManager.m_quests.Values)
                            {
                                CreatureQuestsPlugin.CreatureQuestsLogger.LogInfo($"[{quest.Index}.{quest.Name}]");
                                CreatureQuestsPlugin.CreatureQuestsLogger.LogInfo($"Completed: {quest.m_completed}");
                                CreatureQuestsPlugin.CreatureQuestsLogger.LogInfo("");
                            }
                            break;
                        case "start" or "info":
                            if (args.Length < 3)
                            {
                                CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning("Missing quest index");
                                return true;
                            }
                            var id = args[2];
                            if (!int.TryParse(id, out int index))
                            {
                                CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning("Invalid index");
                                return true;
                            }
                            var quests = QuestManager.m_quests.Values.ToList();
                            if (index < 0 || index >= quests.Count)
                            {
                                CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning("Failed to find quest");
                                return true;
                            }
                            var option = quests[index];
                            if (!option.m_isValid)
                            {
                                CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning("Invalid Quest: " + option.Name);
                                return true;
                            }

                            if (args[1] == "start")
                            {
                                option.Reset();
                                option.AcceptQuest();
                            }
                            else
                            {
                                switch (option.Data.Type)
                                {
                                    default:
                                        CreatureQuestsPlugin.CreatureQuestsLogger.LogInfo($"[{option.Index}.{option.Name}]");
                                        CreatureQuestsPlugin.CreatureQuestsLogger.LogInfo("Progress: " + option.Data.Progress);
                                        CreatureQuestsPlugin.CreatureQuestsLogger.LogInfo("Completed: " + option.m_completed);
                                        break;
                                }
                            }
                            break;
                    }
                    return true;
                }), onlyAdmin: true , optionsFetcher:()=>new List<string> {"help", "reset", "info", "list", "start"});
        }
    }
}