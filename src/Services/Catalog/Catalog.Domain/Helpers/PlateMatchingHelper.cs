namespace Catalog.Domain.Helpers
{
    public static class PlateMatchingHelper
    {
        private static readonly Dictionary<char, string> _equivalences = new()
        {
            { 'A', "[A4]" }, { '4', "[A4]" },
            { 'E', "[E3]" }, { '3', "[E3]" },
            { 'G', "[G6]" }, { '6', "[G6]" },
            { 'I', "[I1]" }, { '1', "[I1]" },
            { 'O', "[O0]" }, { '0', "[O0]" },
            { 'S', "[S5]" }, { '5', "[S5]" },
            { 'T', "[T7]" }, { '7', "[T7]" },
            { 'Z', "[Z2]" }, { '2', "[Z2]" }
        };

        public static string BuildRegexPattern(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return string.Empty;

            searchTerm = searchTerm.ToUpper().Replace(" ", "");

            var pattern = string.Join(".*?", searchTerm.Select(c =>
                _equivalences.TryGetValue(c, out var equiv) ? equiv : c.ToString()));

            return pattern;
        }
    }
}