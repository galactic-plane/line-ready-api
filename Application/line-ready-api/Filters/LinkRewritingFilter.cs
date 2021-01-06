using LineReadyApi.Extentions;
using LineReadyApi.Infrastructure;
using LineReadyApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LineReadyApi.Filters
{
    public class LinkRewritingFilter : IAsyncResultFilter
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public LinkRewritingFilter(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        public Task OnResultExecutionAsync(
            ResultExecutingContext context, ResultExecutionDelegate next)
        {
            ObjectResult asObjectResult = context.Result as ObjectResult;
            bool shouldSkip = (asObjectResult?.StatusCode >= 400)
                || (asObjectResult?.Value == null)
                || (asObjectResult?.Value as Resource == null);

            if (shouldSkip)
            {
                return next();
            }

            LinkRewriter rewriter = new LinkRewriter(_urlHelperFactory.GetUrlHelper(context));
            RewriteAllLinks(asObjectResult.Value, rewriter);

            return next();
        }

        private static void RewriteAllLinks(object model, LinkRewriter rewriter)
        {
            if (model == null) return;

            PropertyInfo[] allProperties = model
                .GetType().GetTypeInfo()
                .GetAllProperties()
                .Where(p => p.CanRead)
                .ToArray();

            IEnumerable<PropertyInfo> linkProperties = allProperties
                .Where(p => p.CanWrite && (p.PropertyType == typeof(Link)));

            foreach (PropertyInfo linkProperty in linkProperties)
            {
                Link rewritten = rewriter.Rewrite(linkProperty.GetValue(model) as Link);
                if (rewritten == null) continue;

                linkProperty.SetValue(model, rewritten);

                // Special handling of the hidden Self property: unwrap into the root object
                if (linkProperty.Name == nameof(Resource.Self))
                {
                    allProperties
                        .SingleOrDefault(p => p.Name == nameof(Resource.Href))
                        ?.SetValue(model, rewritten.Href);

                    allProperties
                        .SingleOrDefault(p => p.Name == nameof(Resource.Method))
                        ?.SetValue(model, rewritten.Method);

                    allProperties
                        .SingleOrDefault(p => p.Name == nameof(Resource.Relations))
                        ?.SetValue(model, rewritten.Relations);
                }
            }

            IEnumerable<PropertyInfo> arrayProperties = allProperties.Where(p => p.PropertyType.IsArray);
            RewriteLinksInArrays(arrayProperties, model, rewriter);

            IEnumerable<PropertyInfo> objectProperties = allProperties
                .Except(linkProperties)
                .Except(arrayProperties);
            RewriteLinksInNestedObjects(objectProperties, model, rewriter);
        }

        private static void RewriteLinksInArrays(
            IEnumerable<PropertyInfo> arrayProperties,
            object model,
            LinkRewriter rewriter)
        {
            foreach (PropertyInfo arrayProperty in arrayProperties)
            {
                Array array = arrayProperty.GetValue(model) as Array ?? new Array[0];

                foreach (object element in array)
                {
                    RewriteAllLinks(element, rewriter);
                }
            }
        }

        private static void RewriteLinksInNestedObjects(
            IEnumerable<PropertyInfo> objectProperties,
            object model,
            LinkRewriter rewriter)
        {
            foreach (PropertyInfo objectProperty in objectProperties)
            {
                if (objectProperty.PropertyType == typeof(string))
                {
                    continue;
                }

                TypeInfo typeInfo = objectProperty.PropertyType.GetTypeInfo();
                if (typeInfo.IsClass)
                {
                    RewriteAllLinks(objectProperty.GetValue(model), rewriter);
                }
            }
        }
    }
}