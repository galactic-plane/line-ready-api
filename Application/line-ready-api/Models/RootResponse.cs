namespace LineReadyApi.Models
{
    public class RootResponse : Resource
    {
        public Link Info { get; set; }

        public Link Rooms { get; set; }
    }
}