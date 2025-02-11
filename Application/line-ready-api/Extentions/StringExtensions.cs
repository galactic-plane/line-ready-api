namespace LineReadyApi.Extentions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string first = input.Substring(0, 1).ToLower();
            if (input.Length == 1) return first;

            return $"{first}{input.Substring(1)}";
        }
    }
}