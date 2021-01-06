namespace LineReadyApi.Infrastructure
{
    public class SearchTerm
    {
        public string EntityName { get; set; }

        public ISearchExpressionProvider ExpressionProvider { get; set; }
        public string Name { get; set; }

        public string Operator { get; set; }

        public bool ValidSyntax { get; set; }

        public string Value { get; set; }
    }
}