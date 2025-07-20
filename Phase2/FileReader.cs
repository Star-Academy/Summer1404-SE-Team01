namespace FullTextSearch;

class FileReader
{
    public static Dictionary<string, string> ReadAllFiles(string basePath)
    {
        
        var docs = new Dictionary<string, string>();
        if (Directory.Exists(basePath))
        {
            foreach (string filePath in Directory.GetFiles(basePath))
            {
                
            
                string content = File.ReadAllText(filePath);
                string docId = Path.GetFileName(filePath);
                
                docs[docId] =  content;
                
            }
        }
        else
        {
            Console.WriteLine("Directory does not exist.");
        }
        return docs;
        
    }
}