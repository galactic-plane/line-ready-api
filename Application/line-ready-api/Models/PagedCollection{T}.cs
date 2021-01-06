using Microsoft.AspNetCore.Routing;
using System;

namespace LineReadyApi.Models
{
    public class PagedCollection<T> : Collection<T>
    {
        public Link First { get; set; }

        public Link Last { get; set; }

        public int? Limit { get; set; }

        public Link Next { get; set; }

        public int? Offset { get; set; }

        public Link Previous { get; set; }

        public int Size { get; set; }

        public static PagedCollection<T> Create(
            Link self, T[] items, int size, PagingOptions pagingOptions)
        {
            return Create<PagedCollection<T>>(self, items, size, pagingOptions);
        }

        public static TResponse Create<TResponse>(
            Link self, T[] items, int size, PagingOptions pagingOptions) where TResponse : PagedCollection<T>, new()

        {
            return new TResponse
            {
                Self = self,
                Value = items,
                Size = size,
                Offset = pagingOptions.Offset,
                Limit = pagingOptions.Limit,
                First = self,
                Next = GetNextLink(self, size, pagingOptions),
                Previous = GetPreviousLink(self, size, pagingOptions),
                Last = GetLastLink(self, size, pagingOptions)
            };
        }

        private static Link GetLastLink(Link self, int size, PagingOptions pagingOptions)
        {
            if (pagingOptions?.Limit == null) return null;

            int limit = pagingOptions.Limit.Value;

            if (size <= limit) return null;

            double offset = Math.Ceiling((size - ((double)limit)) / limit) * limit;

            RouteValueDictionary parameters = new RouteValueDictionary(self.RouteValues)
            {
                ["limit"] = limit,
                ["offset"] = offset
            };
            Link newLink = Link.ToCollection(self.RouteName, parameters);

            return newLink;
        }

        private static Link GetNextLink(
            Link self, int size, PagingOptions pagingOptions)
        {
            if (pagingOptions?.Limit == null) return null;
            if (pagingOptions?.Offset == null) return null;

            int limit = pagingOptions.Limit.Value;
            int offset = pagingOptions.Offset.Value;

            int nextPage = offset + limit;
            if (nextPage >= size)
            {
                return null;
            }

            RouteValueDictionary parameters = new RouteValueDictionary(self.RouteValues)
            {
                ["limit"] = limit,
                ["offset"] = nextPage
            };

            Link newLink = Link.ToCollection(self.RouteName, parameters);
            return newLink;
        }

        private static Link GetPreviousLink(Link self, int size, PagingOptions pagingOptions)
        {
            if (pagingOptions?.Limit == null) return null;
            if (pagingOptions?.Offset == null) return null;

            int limit = pagingOptions.Limit.Value;
            int offset = pagingOptions.Offset.Value;

            if (offset == 0)
            {
                return null;
            }

            if (offset > size)
            {
                return GetLastLink(self, size, pagingOptions);
            }

            int previousPage = Math.Max(offset - limit, 0);

            if (previousPage <= 0)
            {
                return self;
            }

            RouteValueDictionary parameters = new RouteValueDictionary(self.RouteValues)
            {
                ["limit"] = limit,
                ["offset"] = previousPage
            };
            Link newLink = Link.ToCollection(self.RouteName, parameters);

            return newLink;
        }
    }
}