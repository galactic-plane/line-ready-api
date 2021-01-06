using System.Collections.Generic;

namespace LineReadyApi.Data
{
    public class RootEntity
    {
        public IEnumerable<CityEntity> CityData { get; set; }
        public IEnumerable<FishEntity> FishData { get; set; }
    }
}