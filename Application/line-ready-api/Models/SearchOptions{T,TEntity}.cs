using LineReadyApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LineReadyApi.Models
{
    public class SearchOptions<T, TEntity> : IValidatableObject
    {
        public string[] Search { get; set; }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            SearchOptionsProcessor<T, TEntity> processor = new SearchOptionsProcessor<T, TEntity>(Search);
            return processor.Apply(query);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            SearchOptionsProcessor<T, TEntity> processor = new SearchOptionsProcessor<T, TEntity>(Search);

            IEnumerable<string> validTerms = processor.GetValidTerms().Select(x => x.Name);
            IEnumerable<string> invalidTerms = processor.GetAllTerms().Select(x => x.Name)
                .Except(validTerms, StringComparer.OrdinalIgnoreCase);

            foreach (string term in invalidTerms)
            {
                yield return new ValidationResult(
                    $"Invalid search term '{term}'.",
                    new[] { nameof(Search) });
            }
        }
    }
}