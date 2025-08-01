namespace Shapeshift.Source;

public static class Helpers
{
    public static string GetLastWord(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;
    
        // Trim whitespace and split by spaces
        string[] words = input.Trim().Split(' ');
    
        // Return the last word, or empty string if no words found
        return words.Length > 0 ? words[words.Length - 1] : string.Empty;
    }
    
    public static string SplitCamelCase(string camelCase)
    {
        if (string.IsNullOrEmpty(camelCase))
            return camelCase;
    
        var result = new System.Text.StringBuilder();
        bool isNewWord = true;
    
        for (int i = 0; i < camelCase.Length; i++)
        {
            char c = camelCase[i];
        
            // Replace underscores with spaces and flag as new word
            if (c == '_')
            {
                result.Append(' ');
                isNewWord = true;
                continue;
            }
        
            // Add space before uppercase letters (except first character)
            if (i > 0 && char.IsUpper(c))
            {
                result.Append(' ');
                isNewWord = true;
            }
        
            // Capitalize first letter of each word if it's not already uppercase
            if (isNewWord && char.IsLetter(c))
            {
                result.Append(char.ToUpper(c));
                isNewWord = false;
            }
            else
            {
                result.Append(c);
            }
        }
    
        return result.ToString();
    }
}