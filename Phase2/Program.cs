using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace FullTextSearch;

class Program
{
    static void Main(string[] args)
    {
        CreateInvertedIndex();

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
                    SearchForSingleInput();
                    break;
                case "2":
                    SearchWithQuery();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Goodbye!");
    }


    private static void CreateInvertedIndex()
    {
        string baseDir = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
        string englishDataDir = Path.Combine(baseDir, "EnglishData");

        
        var docs = FileReader.ReadAllFiles(englishDataDir);
        var invertedIndexMap = InvertedIndex.ProcessFilesContent(docs);
        
        // foreach (var key in invertedIndexMap.Keys)
        // {
        //     Console.WriteLine(key);
        //     Console.WriteLine(string.Join(", ", invertedIndexMap[key]));
        //     
        // }

    }
    
    private static void SearchForSingleInput()
    {
        while (true)
        {
            Console.WriteLine("Please enter a word to search for (or 'q' to return to main menu):");
            var input = Console.ReadLine()?.Trim();

            if (input?.ToLower() == "q")
                break;

            var upperInput = input?.ToUpper();
            var postLists = InvertedIndex.SearchWord(upperInput!);
            Console.WriteLine(string.Join(", ", postLists));
        }
    }


    private static void SearchWithQuery()
    {
        while (true)
        {
            Console.WriteLine("Please enter a query to search for (or 'q' to return to main menu):");
            var input = Console.ReadLine()?.Trim();

            if (input?.ToLower() == "q")
                break;

            var upperInput = input?.ToUpper();
            var result = InvertedIndex.AdvancedSearch(upperInput!);
            Console.WriteLine(string.Join(", ", result));
        }
    }

    
}