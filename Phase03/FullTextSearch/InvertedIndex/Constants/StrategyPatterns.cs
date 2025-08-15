namespace FullTextSearch.InvertedIndex.Constants
{
    public static class StrategyPatterns
    {
        public const string RequiredSingleWord = @"^[^-+""][a-zA-Z]+$";
        public const string OptionalSingleWord = @"^\+\w+";
        public const string ExcludedSingleWord = @"^\-\w+";

    }
}
