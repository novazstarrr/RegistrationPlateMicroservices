namespace Catalog.Domain.Helpers
{
    public static class PlateMatchingHelper
    {
        private static readonly Dictionary<char, string[]> _equivalences = new()
        {
            { 'A', new[] { "A", "4" } },
            { '4', new[] { "A", "4" } },
            { 'E', new[] { "E", "3" } },
            { '3', new[] { "E", "3" } },
            { 'G', new[] { "G", "6" } },
            { '6', new[] { "G", "6" } },
            { 'I', new[] { "I", "1" } },
            { '1', new[] { "I", "1" } },
            { 'O', new[] { "O", "0" } },
            { '0', new[] { "O", "0" } },
            { 'S', new[] { "S", "5" } },
            { '5', new[] { "S", "5" } },
            { 'T', new[] { "T", "7" } },
            { '7', new[] { "T", "7" } },
            { 'Z', new[] { "Z", "2" } },
            { '2', new[] { "Z", "2" } }
        };

        public static string BuildSqlLikePattern(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return "%";

            searchTerm = searchTerm.ToUpper().Replace(" ", "");

            var pattern = new System.Text.StringBuilder();
            pattern.Append('%');

            foreach (char c in searchTerm)
            {
                if (_equivalences.TryGetValue(c, out var equivalents))
                {
                    pattern.Append('[')
                           .Append(string.Join("", equivalents))
                           .Append(']')
                           .Append('%');
                }
                else
                {
                    pattern.Append(c)
                           .Append('%');
                }
            }

            return pattern.ToString();
        }
    }
}