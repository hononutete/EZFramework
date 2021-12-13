namespace EZFramework
{
    public static class StringExtensions
    {
        public static string ReplaceNewLineCode(this string text)
        {
            if (text.Contains("\\n"))
                text = text.Replace("\\n", System.Environment.NewLine);

            return text;
        }
    }
}
