using System.Linq;
using HarmonyLib;
using ShapeShiftManager;


namespace CreatureQuests.Source;

public class SE_ValkyrieSet : SE_Stats
{
    public override string GetTooltipString()
    {
        var tooltip = base.GetTooltipString();
        tooltip += "<color=orange>$tooltip_wyrdform_master</color>\n";
        tooltip += "$tooltip_input_chat: <color=orange>/wyrdform</color> $tooltip_to_revert\n";
        tooltip += "$tooltip_input_chat: <color=orange>/wyrdform</color> $tooltip_creature_name";
        return tooltip;
    }

    [HarmonyPatch(typeof(Chat), nameof(Chat.InputText))]
    private static class Chat_InputText_Patch
    {
        private static bool Prefix(Chat __instance)
        {
            string text = __instance.m_input.text;
            if (text.Length == 0) return true;
            if (!Player.m_localPlayer.GetSEMan().HaveStatusEffect("SE_ValkyrieSet".GetStableHashCode())) return true;
            string[] words = text.Split(' ');
            if (words[0].ToLower() != "/wyrdform") return true;

            if (words.Length < 2)
            {
                CreatureFormManager.Revert(Player.m_localPlayer);
            }
            else
            {
                string name = string.Join(" ", words.Skip(1));
                name = FindName(name);
                if (string.IsNullOrWhiteSpace(name)) return false;
                if (!Transformation.m_sharedNameToCreature.TryGetValue(name, out CreatureFormManager.CreatureForm data)) return false;
                CreatureFormManager.TriggerTransformation(Player.m_localPlayer, data.SourcePrefabName, 0f);
            }

            return false;
        }
    }
    

    private static string FindName(string input)
    {
        input = input.ToLower().Trim();

        string? bestMatch = null;
        int bestScore = int.MinValue;

        foreach (var name in Transformation.m_sharedNameToCreature.Keys)
        {
            string lowerName = name.ToLower();

            if (lowerName == input)
            {
                // Exact match — no need to search further
                return name;
            }

            if (lowerName.Contains(input))
            {
                int score = 1000 - (lowerName.Length - input.Length);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = name;
                }
            }
            else if (input.Contains(lowerName))
            {
                int score = 500 - (input.Length - lowerName.Length);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = name;
                }
            }
        }

        return bestMatch ?? "";
    }
}