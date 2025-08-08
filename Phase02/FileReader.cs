namespace FullTextSearch;

class FileReader
{
    public static Dictionary<string, string> ReadAllFiles(string basePath)
    {

        var docs = new Dictionary<string, string>();
        if (Directory.Exists(basePath))
        {
            foreach (var filePath in Directory.GetFiles(basePath))
            {


                var content = File.ReadAllText(filePath);
                var docId = Path.GetFileName(filePath);

                docs[docId] = content;

            }
        }
        else
        {
            Console.WriteLine("Directory does not exist.");
        }
        return docs;

    }
}