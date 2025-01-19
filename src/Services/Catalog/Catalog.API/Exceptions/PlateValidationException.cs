namespace Catalog.API.Exceptions
{
    public class PlateValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public PlateValidationException(IReadOnlyDictionary<string, string[]> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }
}
