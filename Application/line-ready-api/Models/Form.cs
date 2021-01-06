namespace LineReadyApi.Models
{
    public class Form : Collection<FormField>
    {
        public const string CreateRelation = "create-form";
        public const string EditRelation = "edit-form";
        public const string QueryRelation = "query-form";
        public const string Relation = "form";
    }
}