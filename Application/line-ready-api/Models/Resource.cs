using System.Text.Json.Serialization;

namespace LineReadyApi.Models
{
    public abstract class Resource : Link
    {
        [JsonIgnore]
        public Link Self { get; set; }
    }
}