using LineReadyApi.Data;
using LineReadyApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LineReadyApi.Services
{
    public class CityService : ICityService
    {
        private readonly LineReadyApiDbContext _context;

        public CityService(LineReadyApiDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<CityData>> GetCitiesAsync(
            PagingOptions pagingOptions,
            SortOptions<CityData, CityEntity> sortOptions,
            SearchOptions<CityData, CityEntity> searchOptions)
        {
            IQueryable<CityEntity> query = _context.Cities.Include(x => x.Coord);
            query = searchOptions.Apply(query);
            query = sortOptions.Apply(query);

            int size = await query.CountAsync();

            CityEntity[] items = await query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ToArrayAsync();

            return new PagedResults<CityData>
            {
                Items = items.Select(x => new CityData()
                {
                    Id = x.Id,
                    Name = x.Name,
                    State = x.State,
                    Country = x.Country,
                    Coord = new Coord()
                    {
                        Lon = (x.Coord == null) ? 0 : x.Coord.Lon,
                        Lat = (x.Coord == null) ? 0 : x.Coord.Lat
                    }
                }),
                TotalSize = size
            };
        }

        public async Task<CityData> GetCityByIdAsync(int id)
        {
            CityEntity x = await _context.Cities.Include(x => x.Coord).SingleOrDefaultAsync(x => x.Id == id);

            if (x == null)
            {
                return new CityData()
                {
                    Id = id,
                    Name = "Not Found"
                };
            }

            return new CityData()
            {
                Id = x.Id,
                Name = x.Name,
                State = x.State,
                Country = x.Country,
                Coord = new Coord()
                {
                    Lon = (x.Coord == null) ? 0 : x.Coord.Lon,
                    Lat = (x.Coord == null) ? 0 : x.Coord.Lat
                }
            };
        }
    }
}