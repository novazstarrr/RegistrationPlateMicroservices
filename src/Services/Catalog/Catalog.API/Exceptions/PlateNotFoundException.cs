namespace Catalog.API.Exceptions
{
    public class PlateNotFoundException : Exception
    {
        public PlateNotFoundException(Guid id)
            : base($"Plate with ID {id} was not found.")
        {
        }
    }
}
