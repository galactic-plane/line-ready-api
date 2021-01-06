using LineReadyApi.Data;
using LineReadyApi.Models;
using System.Threading.Tasks;

namespace LineReadyApi.Services
{
    public interface IFishService
    {
        Task<PagedResults<FishData>> GetFishAsync(PagingOptions pagingOptions, SortOptions<FishData, FishEntity> sortOptions, SearchOptions<FishData, FishEntity> searchOptions);

        Task<FishData> GetFishByIdAsync(int id);
    }
}