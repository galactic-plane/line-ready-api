using System;

namespace LineReadyApi.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SortableAttribute : Attribute
    {
        public bool Default { get; set; }
        public string EntityProperty { get; set; }
    }
}