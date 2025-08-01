using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using HarmonyLib;
using Shapeshift.Source;
using ShapeShiftManager;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CreatureQuests.Source;

public static class Commands
{
    [HarmonyPatch(typeof(Terminal), nameof(Terminal.Awake))]
    private static class Terminal_Awake_Patch
    {
        private static void Postfix()
        {
            Terminal.ConsoleCommand main = new Terminal.ConsoleCommand(CommandData.m_startCommand,
                "Use help to find commands",
                args =>
                {
                    if (args.Length < 2) return false;
                    if (!CommandData.m_commands.TryGetValue(args[1], out CommandData data)) return false;
                    return data.Run(args);
                }, optionsFetcher: CommandData.m_commands.Where(x => !x.Value.IsSecret()).Select(x => x.Key).ToList);

            CommandData setModel = new CommandData("set", "Set model", args =>
            {
                if (args.Length < 3) return false;
                var value = args[2];
                if (CreatureFormManager.GetCreature(value) is not { } creature) return false;
                CreatureFormManager.m_currentModel = creature.PrefabName;
                if (!ZNetScene.instance || !ZNetScene.instance.enabled)
                {
                    if (!FejdStartup.instance) return false;
                    FejdStartup.instance.SetupCharacterPreview(null);
                }
                else
                {
                    if (!Player.m_localPlayer) return false;
                    Player.m_localPlayer.GetSEMan().AddStatusEffect(creature.StatusEffect);
                }

                return true;
            }, adminOnly: true , optionsFetcher: () => CreatureFormManager.GetSourceToCreatureFormDict().Keys.ToList());

            CommandData reset = new CommandData("reset", "reset to player model", args =>
            {
                if (!ZNetScene.instance || !ZNetScene.instance.enabled)
                {
                    if (!FejdStartup.instance) return false;
                    CreatureFormManager.m_currentModel = "";
                    var profile = FejdStartup.instance.m_profiles[FejdStartup.instance.m_profileIndex];
                    FejdStartup.instance.SetupCharacterPreview(profile);
                }
                else
                {
                    if (!Player.m_localPlayer) return false;
                    SE_Shapeshift? status = null;
                    foreach (var effect in Player.m_localPlayer.GetSEMan().GetStatusEffects())
                    {
                        if (effect is not SE_Shapeshift shapeshift) continue;
                        status = shapeshift;
                    }

                    if (status != null)
                    {
                        Player.m_localPlayer.GetSEMan().RemoveStatusEffect(status);
                    }
                    else
                    {
                        CreatureFormManager.CreatureForm.Revert();
                    }
                }

                return true;
            }, adminOnly:true);

            CommandData export = new CommandData("export", "export relevant creature data for custom creature fields",
                args =>
                {
                    if (!ZNetScene.instance) return false;
                    if (args.Length < 3) return false;
                    if (!Transformation.Export(args[2]))
                    {
                        CreatureQuestsPlugin.CreatureQuestsLogger.LogWarning("Failed to export: " + args[2]);
                    }
                    return true;
                }, adminOnly:true);
        }
    }
}