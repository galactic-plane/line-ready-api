using LineReadyApi.Infrastructure;
using System.Text.Json;

namespace LineReadyApi.Models
{
    public class CityData : Resource, IEtaggable
    {

        public string GetEtag()
        {
            string serialized = JsonSerializer.Serialize(this);
            return Md5Hash.ForString(serialized);
        }

        public int Id { get; set; }

        [Sortable]
        [Searchable]
        public string Name { get; set; }

        [Sortable]
        [Searchable]
        public string State { get; set; }

        [Sortable]
        [Searchable]
        public string Country { get; set; }

        public Coord Coord { get; set; }
    }

    public class Coord
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}