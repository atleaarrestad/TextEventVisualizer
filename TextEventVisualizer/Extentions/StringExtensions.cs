using System.Text.RegularExpressions;

namespace TextEventVisualizer.Extentions
{
    public static class StringExtensions
    {
        // Method to clean text before vectorizing it usign text2vec transformer.
        public static string RemoveInvalidCharacters(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Normalize whitespace: replace new lines, carriage returns, and tabs with a space
            string result = Regex.Replace(input, @"\s+", " ");

            // Replace long dashes with a single dash
            result = Regex.Replace(result, @"[—]+", "-"); 

            // Removing citations like (AP), (Reuters), etc.
            result = Regex.Replace(result, @"\([A-Za-z]+\)", "");

            result = Regex.Replace(result, @"http[^\s]+", "");

            result = result.Trim();

            return result.ToLower();

        }


    }
}
