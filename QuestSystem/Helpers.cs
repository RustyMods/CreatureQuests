using System.Text.RegularExpressions;

namespace Shapeshift.QuestSystem;

public static class Helpers
{
    public static string GetNormalizedName(string name) => Regex.Replace(name, @"\s*\(.*?\)", "").Trim();
    
    public static void ParseParentheses(string input, out string outside, out string inside)
    {
        int start = input.IndexOf('(');
        int end = input.LastIndexOf(')');

        if (start > 0 && end > start)
        {
            outside = input.Substring(0, start).Trim();
            inside = input.Substring(start + 1, end - start - 1).Trim();
        }
        else
        {
            outside = string.Empty;
            inside = string.Empty;
        }
    }
    
    public static string RemoveParentheses(string input)
    {
        var result = new System.Text.StringBuilder();
        int depth = 0;

        foreach (char c in input)
        {
            if (c == '(')
            {
                depth++;
            }
            else if (c == ')')
            {
                if (depth > 0) depth--;
            }
            else if (depth == 0)
            {
                result.Append(c);
            }
        }

        return result.ToString().Trim();
    }

}