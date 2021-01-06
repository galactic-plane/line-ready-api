using LineReadyApi.Data;
using LineReadyApi.Infrastructure;
using LineReadyApi.Models;
using LineReadyApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace LineReadyApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class FishController : ControllerBase
    {
        private readonly PagingOptions _defaultPagingOptions;
        private readonly FishData _fishData;
        private readonly IFishService _fishService;

        public FishController(IFishService fishService,
            IOptions<PagingOptions> defaultPagingOptionsWrapper,
            IOptions<FishData> fishDataWrapper)
        {
            _fishService = fishService;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
            _fishData = fishDataWrapper.Value;
        }

        /// <summary>
        /// Gets a collection of fish.
        /// </summary>
        [HttpGet(Name = nameof(GetFish))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<FishData>>> GetFish(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<FishData, FishEntity> sortOptions,
            [FromQuery] SearchOptions<FishData, FishEntity> searchOptions)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            PagedResults<FishData> fish = await _fishService.GetFishAsync(
                pagingOptions, sortOptions, searchOptions);

            FishResponse collection = PagedCollection<FishData>.Create<FishResponse>(
                Link.ToCollection(nameof(GetFish)),
                fish.Items.ToArray(),
                fish.TotalSize,
                pagingOptions);

            collection.FishQuery = FormMetadata.FromResource<FishData>(
                Link.ToForm(
                    nameof(GetFish),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));

            foreach (FishData link in collection.Value)
            {
                link.Href = Url.Link(nameof(GetFishById), new { link.Id });
            }

            return collection;
        }

        /// <summary>
        /// Gets a fish by its id.
        /// </summary>
        [HttpGet("{id}", Name = nameof(GetFishById))]
        [ProducesResponseType(404)]
        [ProducesResponseType(304)]
        [ProducesResponseType(200)]
        [ResponseCache(CacheProfileName = "Static")]
        [Etag]
        public async Task<ActionResult<FishData>> GetFishById(int id)
        {
            if (!Request.GetEtagHandler().NoneMatch(_fishData))
            {
                return StatusCode(304, _fishData);
            }

            FishData data = await _fishService.GetFishByIdAsync(id);

            data.Href = Url.Link(nameof(GetFishById), new { data.Id });

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }
    }
}