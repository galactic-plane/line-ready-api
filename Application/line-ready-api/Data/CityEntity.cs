using System.ComponentModel.DataAnnotations.Schema;

namespace LineReadyApi.Data
{
    public class CityEntity
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        [ForeignKey("CityEntityFK")]
        public virtual CoordEntity Coord { get; set; }

    }
}