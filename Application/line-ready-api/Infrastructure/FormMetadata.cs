using LineReadyApi.Extentions;
using LineReadyApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace LineReadyApi.Infrastructure
{
    public static class FormMetadata
    {
        public static Form FromModel(object model, Link self)
        {
            List<FormField> formFields = new List<FormField>();

            foreach (PropertyInfo prop in model.GetType().GetTypeInfo().DeclaredProperties)
            {
                object value = prop.CanRead
                    ? prop.GetValue(model)
                    : null;

                Attribute[] attributes = prop.GetCustomAttributes().ToArray();

                string name = attributes.OfType<DisplayAttribute>()
                    .SingleOrDefault()?.Name
                    ?? prop.Name.ToCamelCase();

                string label = attributes.OfType<DisplayAttribute>()
                    .SingleOrDefault()?.Description;

                bool required = attributes.OfType<RequiredAttribute>().Any();
                bool secret = attributes.OfType<SecretAttribute>().Any();
                string type = GetFriendlyType(prop, attributes);

                int? minLength = attributes.OfType<MinLengthAttribute>()
                    .SingleOrDefault()?.Length;
                int? maxLength = attributes.OfType<MaxLengthAttribute>()
                    .SingleOrDefault()?.Length;

                formFields.Add(new FormField
                {
                    Name = name,
                    Required = required,
                    Secret = secret,
                    Type = type,
                    Value = value,
                    Label = label,
                    MinLength = minLength,
                    MaxLength = maxLength
                });
            }

            return new Form()
            {
                Self = self,
                Value = formFields.ToArray()
            };
        }

        public static Form FromResource<T>(Link self)
        {
            PropertyInfo[] allProperties = typeof(T).GetTypeInfo().DeclaredProperties.ToArray();
            PropertyInfo[] sortableProperties = allProperties
                .Where(p => p.GetCustomAttributes<SortableAttribute>().Any()).ToArray();
            PropertyInfo[] searchableProperties = allProperties
                .Where(p => p.GetCustomAttributes<SearchableAttribute>().Any()).ToArray();

            if (!sortableProperties.Any() && !searchableProperties.Any())
            {
                return new Form { Self = self };
            }

            List<FormFieldOption> orderByOptions = new List<FormFieldOption>();
            foreach (PropertyInfo prop in sortableProperties)
            {
                string name = prop.Name.ToCamelCase();

                orderByOptions.Add(
                    new FormFieldOption { Label = $"Sort by {name}", Value = name });
                orderByOptions.Add(
                    new FormFieldOption { Label = $"Sort by {name} descending", Value = $"{name} desc" });
            }

            string searchPattern = null;
            if (searchableProperties.Any())
            {
                IEnumerable<string> applicableOperators = searchableProperties
                    .SelectMany(x => x
                        .GetCustomAttribute<SearchableAttribute>()
                        .ExpressionProvider.GetOperators())
                    .Distinct();

                string opGroup = $"{string.Join("|", applicableOperators)}";
                string nameGroup = $"{string.Join("|", searchableProperties.Select(x => x.Name.ToCamelCase()))}";

                searchPattern = $"/({nameGroup}) ({opGroup}) (.*)/i";
            }

            List<FormField> formFields = new List<FormField>();
            if (orderByOptions.Any())
            {
                formFields.Add(new FormField
                {
                    Name = nameof(SortOptions<string, string>.OrderBy).ToCamelCase(),
                    Type = "set",
                    Options = orderByOptions.ToArray()
                });
            }

            if (!string.IsNullOrEmpty(searchPattern))
            {
                formFields.Add(new FormField
                {
                    Name = nameof(SearchOptions<string, string>.Search).ToCamelCase(),
                    Type = "set",
                    Pattern = searchPattern
                });
            }

            return new Form()
            {
                Self = self,
                Value = formFields.ToArray()
            };
        }

        private static string GetFriendlyType(PropertyInfo property, Attribute[] attributes)
        {
            bool isEmail = attributes.OfType<EmailAddressAttribute>().Any();
            if (isEmail) return "email";

            string typeName = FormFieldTypeConverter.GetTypeName(property.PropertyType);
            if (!string.IsNullOrEmpty(typeName)) return typeName;

            return property.PropertyType.Name.ToCamelCase();
        }
    }
}