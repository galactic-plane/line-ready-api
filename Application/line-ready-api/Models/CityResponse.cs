namespace LineReadyApi.Models
{
    public class CityResponse : PagedCollection<CityData>
    {
        public Form CityQuery { get; set; }
    }
}