using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LineReadyApi.Data
{
    public class SpeciesIllustrationPhotoEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Src { get; set; }
        public string Alt { get; set; }
        public string Title { get; set; }

        [ForeignKey(nameof(SpeciesIllustrationPhotoEntityFK))]
        public int SpeciesIllustrationPhotoEntityFK { get; set; }
    }
}