using System.ComponentModel;
using System.Text.Json.Serialization;

namespace LineReadyApi.Models
{
    public class Link
    {
        public const string GetMethod = "GET";
        public const string PostMethod = "POST";

        public string Href { get; set; }

        [DefaultValue(GetMethod)]
        public string Method { get; set; }

        [JsonPropertyName("rel")]
        public string[] Relations { get; set; }

        // Stores the route name before being rewritten by the LinkRewritingFilter
        [JsonIgnore]
        public string RouteName { get; set; }

        // Stores the route parameters before being rewritten by the LinkRewritingFilter
        [JsonIgnore]
        public object RouteValues { get; set; }

        public static Link To(string routeName, object routeValues = null)
        {
            return new Link
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = null
            };
        }

        public static Link ToCollection(string routeName, object routeValues = null)
        {
            return new Link
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = GetMethod,
                Relations = new[] { "collection" }
            };
        }

        public static Link ToForm(
            string routeName,
            object routeValues = null,
            string method = PostMethod,
            params string[] relations)
        {
            return new Link
            {
                RouteName = routeName,
                RouteValues = routeValues,
                Method = method,
                Relations = relations
            };
        }
    }
}