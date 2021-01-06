using LineReadyApi.Data;
using LineReadyApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineReadyApi.Services
{
    public class FishService : IFishService
    {
        private readonly LineReadyApiDbContext _context;

        public FishService(LineReadyApiDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<FishData>> GetFishAsync(
           PagingOptions pagingOptions,
           SortOptions<FishData, FishEntity> sortOptions,
           SearchOptions<FishData, FishEntity> searchOptions)
        {
            IQueryable<FishEntity> query = _context.Fish.Include(x => x.SpeciesIllustrationPhoto);
            query = searchOptions.Apply(query);
            query = sortOptions.Apply(query);

            int size = await query.CountAsync();

            FishEntity[] items = await query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ToArrayAsync();

            return new PagedResults<FishData>
            {
                Items = items.Select(x => new FishData()
                {
                    Id = x.Id,
                    FisheryManagement = x.FisheryManagement,
                    Habitat = x.Habitat,
                    HabitatImpacts = x.HabitatImpacts,
                    Location = x.Location,
                    Population = x.Population,
                    PopulationStatus = x.PopulationStatus,
                    ScientificName = x.ScientificName,
                    SpeciesAliases = x.SpeciesAliases,
                    SpeciesIllustrationPhoto = new SpeciesIllustrationPhoto()
                    {
                        Src = (x.SpeciesIllustrationPhoto == null) ? string.Empty : (x.SpeciesIllustrationPhoto.Src ?? string.Empty),
                        Alt = (x.SpeciesIllustrationPhoto == null) ? string.Empty : (x.SpeciesIllustrationPhoto.Alt ?? string.Empty),
                        Title = (x.SpeciesIllustrationPhoto == null) ? string.Empty : (x.SpeciesIllustrationPhoto.Title ?? string.Empty)
                    },
                    ImageGallery = FillGallery(x.ImageGallery),
                    SpeciesName = x.SpeciesName,
                    Availability = x.Availability,
                    Biology = x.Biology,
                    Bycatch = x.Bycatch,
                    Calories = x.Calories,
                    Cholesterol = x.Cholesterol,
                    Color = x.Color,
                    FatTotal = x.FatTotal,
                    FishingRate = x.FishingRate,
                    Harvest = x.Harvest,
                    HealthBenefits = x.HealthBenefits,
                    PhysicalDescription = x.PhysicalDescription,
                    Protein = x.Protein,
                    Quote = x.Quote,
                    QuoteBackgroundColor = x.QuoteBackgroundColor,
                    SaturatedFattyAcidsTotal = x.SaturatedFattyAcidsTotal,
                    Selenium = x.Selenium,
                    Servings = x.Servings,
                    Sodium = x.Sodium,
                    Source = x.Source,
                    Taste = x.Taste,
                    Texture = x.Texture,
                    Path = x.Path,
                    LastUpdate = x.LastUpdate
                }),
                TotalSize = size
            };
        }

        public async Task<FishData> GetFishByIdAsync(int id)
        {
            FishEntity x = await _context.Fish.Include(x => x.SpeciesIllustrationPhoto).SingleOrDefaultAsync(x => x.Id == id);

            if (x == null)
            {
                return new FishData()
                {
                    Id = id,
                    ScientificName = "Not Found",
                    SpeciesName = "Not Found"
                };
            }

            return new FishData()
            {
                Id = x.Id,
                FisheryManagement = x.FisheryManagement,
                Habitat = x.Habitat,
                HabitatImpacts = x.HabitatImpacts,
                Location = x.Location,
                Population = x.Population,
                PopulationStatus = x.PopulationStatus,
                ScientificName = x.ScientificName,
                SpeciesAliases = x.SpeciesAliases,
                SpeciesIllustrationPhoto = new SpeciesIllustrationPhoto()
                {
                    Src = (x.SpeciesIllustrationPhoto == null) ? string.Empty : (x.SpeciesIllustrationPhoto.Src ?? string.Empty),
                    Alt = (x.SpeciesIllustrationPhoto == null) ? string.Empty : (x.SpeciesIllustrationPhoto.Alt ?? string.Empty),
                    Title = (x.SpeciesIllustrationPhoto == null) ? string.Empty : (x.SpeciesIllustrationPhoto.Title ?? string.Empty)
                },
                ImageGallery = FillGallery(x.ImageGallery),
                SpeciesName = x.SpeciesName,
                Availability = x.Availability,
                Biology = x.Biology,
                Bycatch = x.Bycatch,
                Calories = x.Calories,
                Cholesterol = x.Cholesterol,
                Color = x.Color,
                FatTotal = x.FatTotal,
                FishingRate = x.FishingRate,
                Harvest = x.Harvest,
                HealthBenefits = x.HealthBenefits,
                PhysicalDescription = x.PhysicalDescription,
                Protein = x.Protein,
                Quote = x.Quote,
                QuoteBackgroundColor = x.QuoteBackgroundColor,
                SaturatedFattyAcidsTotal = x.SaturatedFattyAcidsTotal,
                Selenium = x.Selenium,
                Servings = x.Servings,
                Sodium = x.Sodium,
                Source = x.Source,
                Taste = x.Taste,
                Texture = x.Texture,
                Path = x.Path,
                LastUpdate = x.LastUpdate
            };
        }

        private List<ImageGallery> FillGallery(List<ImageGalleryEntity> imageGallery)
        {
            if ((imageGallery == null) || (imageGallery.Count == 0))
            {
                return null;
            }

            return imageGallery.Select(x => new ImageGallery()
            {
                Alt = x.Alt,
                Src = x.Src,
                Title = x.Title
            }).ToList();
        }
    }
}