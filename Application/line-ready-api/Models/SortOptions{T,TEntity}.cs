using LineReadyApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LineReadyApi.Models
{
    public class SortOptions<T, TEntity> : IValidatableObject
    {
        public string[] OrderBy { get; set; }

        // The service code will call this to apply these sort options to a database query
        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            SortOptionsProcessor<T, TEntity> processor = new SortOptionsProcessor<T, TEntity>(OrderBy);
            return processor.Apply(query);
        }

        // ASP.NET Core calls this validate incoming parameters
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            SortOptionsProcessor<T, TEntity> processor = new SortOptionsProcessor<T, TEntity>(OrderBy);

            IEnumerable<string> validTerms = processor.GetValidTerms().Select(x => x.Name);

            IEnumerable<string> invalidTerms = processor.GetAllTerms().Select(x => x.Name)
                .Except(validTerms, StringComparer.OrdinalIgnoreCase);

            foreach (string term in invalidTerms)
            {
                yield return new ValidationResult(
                    $"Invalid sort term '{term}'.",
                    new[] { nameof(OrderBy) });
            }
        }
    }
}