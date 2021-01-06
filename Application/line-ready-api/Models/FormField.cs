using System.ComponentModel;

namespace LineReadyApi.Models
{
    public class FormField
    {
        public const string DefaultType = "string";
        public string Label { get; set; }
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public string Name { get; set; }
        public FormFieldOption[] Options { get; set; }
        public string Pattern { get; set; }

        public bool Required { get; set; }
        public bool Secret { get; set; }

        [DefaultValue(DefaultType)]
        public string Type { get; set; } = DefaultType;

        public object Value { get; set; }
    }
}