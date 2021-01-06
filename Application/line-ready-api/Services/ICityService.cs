using LineReadyApi.Data;
using LineReadyApi.Models;
using System.Threading.Tasks;

namespace LineReadyApi.Services
{
    public interface ICityService
    {
        Task<PagedResults<CityData>> GetCitiesAsync(
            PagingOptions pagingOptions,
            SortOptions<CityData, CityEntity> sortOptions,
            SearchOptions<CityData, CityEntity> searchOptions);

        Task<CityData> GetCityByIdAsync(int id);
    }
}