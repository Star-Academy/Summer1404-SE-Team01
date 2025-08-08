namespace FullTextSearch
{
    class Program
    {
        private const string DataSetPath = "EnglishData";

        static void Main(string[] args)
        {
            var dto = CreateInvertedIndex();

            while (true)
            {
                Console.WriteLine("\nChoose search mode:");
                Console.WriteLine("1. Single word search");
                Console.WriteLine("2. Query search");
                Console.WriteLine("q. Quit");
                Console.Write("Your choice: ");

                var choice = Console.ReadLine()?.Trim().ToLower();
                if (choice == "q")
                    break;

                switch (choice)
                {
                    case "1":
                        SearchForSingleInput(dto);
                        break;
                    case "2":
                        SearchWithQuery(dto);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }

            Console.WriteLine("Goodbye!");
        }

        private static InvertedIndexDto CreateInvertedIndex()
        {
            var docs = FileReader.ReadAllFiles(DataSetPath);
            return InvertedIndex.Build(docs);
        }

        private static void SearchForSingleInput(InvertedIndexDto dto)
        {
            while (true)
            {
                Console.Write("Please enter a word to search for (or 'q' to return to main menu): ");
                var input = Console.ReadLine()?.Trim();

                if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                    break;

                var results = InvertedIndex.SearchWord(input!, dto);
                Console.WriteLine(results.Count > 0
                    ? string.Join(", ", results)
                    : "No matching documents.");
            }
        }

        private static void SearchWithQuery(InvertedIndexDto dto)
        {
            while (true)
            {
                Console.WriteLine("Please enter a query to search for (or 'q' to return to main menu): ");
                var input = Console.ReadLine()?.Trim();

                if (string.Equals(input, "q", StringComparison.OrdinalIgnoreCase))
                    break;

                var results = InvertedIndex.AdvancedSearch(input!, dto);
                Console.WriteLine(results.Count > 0
                    ? string.Join(", ", results)
                    : "No matching documents.");
            }
        }
    }
}