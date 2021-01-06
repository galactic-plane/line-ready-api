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
    public class CitiesController : ControllerBase
    {
        private readonly CityData _cityData;
        private readonly ICityService _cityService;
        private readonly PagingOptions _defaultPagingOptions;

        public CitiesController(ICityService cityService,
            IOptions<PagingOptions> defaultPagingOptionsWrapper,
            IOptions<CityData> cityDataWrapper)
        {
            _cityService = cityService;
            _defaultPagingOptions = defaultPagingOptionsWrapper.Value;
            _cityData = cityDataWrapper.Value;
        }

        /// <summary>
        /// Gets a collection of US Cities.
        /// </summary>
        [HttpGet(Name = nameof(GetCities))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Collection<CityData>>> GetCities(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<CityData, CityEntity> sortOptions,
            [FromQuery] SearchOptions<CityData, CityEntity> searchOptions)
        {
            pagingOptions.Offset = pagingOptions.Offset ?? _defaultPagingOptions.Offset;
            pagingOptions.Limit = pagingOptions.Limit ?? _defaultPagingOptions.Limit;

            PagedResults<CityData> cities = await _cityService.GetCitiesAsync(
                pagingOptions, sortOptions, searchOptions);

            CityResponse collection = PagedCollection<CityData>.Create<CityResponse>(
                Link.ToCollection(nameof(GetCities)),
                cities.Items.ToArray(),
                cities.TotalSize,
                pagingOptions);

            collection.CityQuery = FormMetadata.FromResource<CityData>(
                Link.ToForm(
                    nameof(GetCities),
                    null,
                    Link.GetMethod,
                    Form.QueryRelation));

            foreach (CityData link in collection.Value)
            {
                link.Href = Url.Link(nameof(GetCityById), new { link.Id });
            }

            return collection;
        }

        /// <summary>
        /// Gets a US City by its id.
        /// </summary>
        [HttpGet("{id}", Name = nameof(GetCityById))]
        [ProducesResponseType(404)]
        [ProducesResponseType(304)]
        [ProducesResponseType(200)]
        [ResponseCache(CacheProfileName = "Static")]
        [Etag]
        public async Task<ActionResult<CityData>> GetCityById(int id)
        {
            if (!Request.GetEtagHandler().NoneMatch(_cityData))
            {
                return StatusCode(304, _cityData);
            }

            CityData data = await _cityService.GetCityByIdAsync(id);

            data.Href = Url.Link(nameof(GetCityById), new { data.Id });

            if (data == null)
            {
                return NotFound();
            }

            return data;
        }
    }
}