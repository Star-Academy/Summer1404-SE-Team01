namespace FullTextSearch.API.InvertedIndex.Constants
{
    public static class StrategyPatterns
    {
        public const string RequiredSingleWord = @"^[^-+""][a-zA-Z]+$";
        public const string OptionalSingleWord = @"^\+\w+";
        public const string ExcludedSingleWord = @"^\-\w+";
        public const string RequiredPhrase = @"(?<!\S)""([^""]+)""";
        public const string OptionalPhrase = @"\+""([^""]+)""";
        public const string ExcludedPhrase = @"-""([^""]+)""";

    }
}
