using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace LineReadyApi.Infrastructure
{
    public class EtagHandlerFeature : IEtagHandlerFeature
    {
        private readonly IHeaderDictionary _headers;

        public EtagHandlerFeature(IHeaderDictionary headers)
        {
            _headers = headers;
        }

        public bool NoneMatch(IEtaggable entity)
        {
            if (!_headers.TryGetValue("If-None-Match", out StringValues etags)) return true;

            string entityEtag = entity.GetEtag();
            if (string.IsNullOrEmpty(entityEtag)) return true;

            if (!entityEtag.Contains('"'))
            {
                entityEtag = $"\"{entityEtag}\"";
            }

            return !etags.Contains(entityEtag);
        }
    }
}