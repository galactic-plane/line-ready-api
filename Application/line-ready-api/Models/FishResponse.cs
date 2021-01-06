namespace LineReadyApi.Models
{
    public class FishResponse : PagedCollection<FishData>
    {
        public Form FishQuery { get; set; }
    }
}