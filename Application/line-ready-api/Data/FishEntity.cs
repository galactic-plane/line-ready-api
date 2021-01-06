using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LineReadyApi.Data
{
    public class FishEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FisheryManagement { get; set; }
        public string Habitat { get; set; }
        public string HabitatImpacts { get; set; }

        [ForeignKey("ImageGalleryFK")]
        public List<ImageGalleryEntity> ImageGallery { get; set; }
        public string Location { get; set; }
        public string Management { get; set; }
        public string NOAAFisheriesRegion { get; set; }
        public string Population { get; set; }
        public string PopulationStatus { get; set; }
        public string ScientificName { get; set; }
        public string SpeciesAliases { get; set; }

        [ForeignKey("SpeciesIllustrationPhotoEntityFK")]
        public SpeciesIllustrationPhotoEntity SpeciesIllustrationPhoto { get; set; }

        public string SpeciesName { get; set; }
        public string AnimalHealth { get; set; }
        public string Availability { get; set; }
        public string Biology { get; set; }
        public string Bycatch { get; set; }
        public string Calories { get; set; }
        public string Carbohydrate { get; set; }
        public string Cholesterol { get; set; }
        public string Color { get; set; }
        public string DiseaseTreatmentandPrevention { get; set; }
        public string DiseasesinSalmon { get; set; }
        public string DisplayedSeafoodProfileIllustration { get; set; }
        public string EcosystemServices { get; set; }
        public string EnvironmentalConsiderations { get; set; }
        public string EnvironmentalEffects { get; set; }
        public string FarmingMethods { get; set; }
        public string FatTotal { get; set; }
        public string Feeds_ { get; set; }
        public string Feeds { get; set; }
        public string FiberTotalDietary { get; set; }
        public string FishingRate { get; set; }
        public string Harvest { get; set; }
        public string HarvestType { get; set; }
        public string HealthBenefits { get; set; }
        public string Human_Health_ { get; set; }
        public string HumanHealth { get; set; }
        public string PhysicalDescription { get; set; }
        public string Production { get; set; }
        public string Protein { get; set; }
        public string Quote { get; set; }
        public string QuoteBackgroundColor { get; set; }
        public string Research { get; set; }
        public string SaturatedFattyAcidsTotal { get; set; }
        public string Selenium { get; set; }
        public string ServingWeight { get; set; }
        public string Servings { get; set; }
        public string Sodium { get; set; }
        public string Source { get; set; }
        public string SugarsTotal { get; set; }
        public string Taste { get; set; }
        public string Texture { get; set; }
        public string Path { get; set; }
        public string LastUpdate { get; set; }
    }
}