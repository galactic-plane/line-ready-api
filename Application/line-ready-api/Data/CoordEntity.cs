using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LineReadyApi.Data
{
    public class CoordEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public double Lat { get; set; }
        public double Lon { get; set; }

        [ForeignKey(nameof(CityEntityFK))]
        public int CityEntityFK { get; set; }
    }
}