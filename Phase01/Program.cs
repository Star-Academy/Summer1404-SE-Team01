using System.Text.Json;

namespace Phase01;

public class Program
{
    private const int TopCount = 3;
    static void Main(string[] args)
    {
        // First Approach using Linq with Join (Readable, concise, and high-level)

        string baseDir = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
        string scoresDataPath = Path.Combine(baseDir, "Data", "scores.json");

        using var scoresStream = File.OpenRead(scoresDataPath);

        var top3 = JsonSerializer.Deserialize<List<CourseGrade>>(scoresStream)!
            .GroupBy(cg => cg.StudentNumber)
                .Select(g =>
                {
                    var stId = g.Key;
                    var avg = g.Average(cg => cg.Score);
                    return (stId, avg);
                }).OrderByDescending(tup => tup.avg).Take(TopCount).ToList();

        var studentsDataPath = Path.Combine(baseDir, "Data", "students.json");
        using var studentsStream = File.OpenRead(studentsDataPath);

        var studentsData = JsonSerializer.Deserialize<List<Student>>(studentsStream)!;

        var StudentsGpa = top3.Join(studentsData,
            sc => sc.stId,
            st => st.StudentNumber,
            (gpa, student) => new
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                Gpa = gpa.avg,
            });

        int rank = 1;
        foreach (var student in StudentsGpa)
        {
            Console.WriteLine(($"{rank++}: Student {student.FirstName} {student.LastName} with average score {student.Gpa:F2}."));
        }


        // Second Approach using Linq without Join (Readable, concise, and high-level)

        //string baseDir = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
        //string scoresDataPath = Path.Combine(baseDir, "Data", "scores.json");

        //using var scoresStream = File.OpenRead(scoresDataPath);

        //var top3 = JsonSerializer
        //    .Deserialize<List<CourseGrade>>(scoresStream)!
        //        .GroupBy(cg => cg.StudentNumber)
        //        .Select(g =>
        //        {
        //            var stId = g.Key;
        //            var avg = g.Average(cg => cg.Score);
        //            return (stId, avg);
        //        }).OrderByDescending(tup => tup.avg).Take(TopCount).ToDictionary<int, double>();



        //var studentsDataPath = Path.Combine(baseDir, "Data", "students.json");
        //using var studentsStream = File.OpenRead(studentsDataPath);

        //var studentsData = JsonSerializer.Deserialize<List<Student>>(studentsStream)!
        //    .ToDictionary(
        //    s => s.StudentNumber,
        //    s => (s.FirstName, s.LastName));

        //int rank = 1;
        //foreach (var key in top3.Keys)
        //{
        //    var (fName, lName) = studentsData[key];
        //    Console.WriteLine($"{rank++}: Student {fName} {lName} with average score {top3[key]:F2}.");
        //}



        // Third approach, simpler approach not using much Linq (Less readable)

        //string baseDir = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
        //string scoresDataPath = Path.Combine(baseDir, "Data", "scores.json");
        //using var scoresStream = File.OpenRead(scoresDataPath);
        //var averages = new Dictionary<int, (double sum, int count)>();
        //var scoresData = JsonSerializer.Deserialize<List<CourseGrade>>(scoresStream)!;

        //foreach (var s in scoresData)
        //{
        //    var key = s.StudentNumber;
        //    if (!averages.ContainsKey(key))
        //        averages[key] = (s.Score, 1);
        //    else
        //    {
        //        var (sum, count) = averages[key];
        //        averages[key] = (sum + s.Score, count + 1);
        //    }
        //}

        //var result = averages.ToDictionary(
        //    kvp => kvp.Key,
        //    kvp => kvp.Value.sum / kvp.Value.count);

        //var top3 = result
        //    .OrderByDescending(kvp => kvp.Value)
        //    .Take(TopCount)
        //    .ToDictionary<int, double>();

        //var studentDict = new Dictionary<int, (string fName, string lName)>();

        //var studentsDataPath = Path.Combine(baseDir, "Data", "students.json");
        //using var studentsStream = File.OpenRead(studentsDataPath);

        //var studentsData = JsonSerializer.Deserialize<List<Student>>(studentsStream)!;

        //foreach (var s in studentsData)
        //{
        //    studentDict[s.StudentNumber] = (s.FirstName, s.LastName);
        //}

        //int rank = 1;
        //foreach (var key in top3.Keys)
        //{
        //    var (fName, lName) = studentDict[key];
        //    Console.WriteLine($"{rank++}: Student {fName} {lName} with average score {top3[key]:F2}.");
        //}
    }
}
