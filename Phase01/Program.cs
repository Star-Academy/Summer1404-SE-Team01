using System.Text.Json;

namespace Phase01;

public class Program
{
    private const int TopCount = 3;
    private const string ScoresDataPath = "Data/scores.json";
    private const string StudentsDataPath = "Data/students.json";
    static void Main(string[] args)
    {

        var dataDto = FetchData();

        var studentAverages = PreComputeStudentAverages(dataDto.CourseGrades);

        var top3 = FindTop3Averages(studentAverages);

        var StudentsGpa = GetCorrespondingStudentsInfo(top3, dataDto.StudentsData);

        int rank = 1;
        foreach (var student in StudentsGpa)
        {
            Console.WriteLine($"{rank++}: Student {student.FirstName} {student.LastName} with average score {student.Average:F2}.");
        }
    }

    private static DataDto FetchData()
    {
        DataDto dataDto = new();
        try
        {
            var scoresJson = File.ReadAllText(ScoresDataPath);
            dataDto.CourseGrades = JsonSerializer.Deserialize<List<CourseGrade>>(scoresJson)!;

            var studentsJson = File.ReadAllText(StudentsDataPath);
            dataDto.StudentsData = JsonSerializer.Deserialize<List<Student>>(studentsJson)!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.Exit(1);
        }

        return dataDto;
    }

    private static Dictionary<int, (double Sum, int Count)> PreComputeStudentAverages(List<CourseGrade> courseGrades)
    {
        var studentAverages = new Dictionary<int, (double Sum, int Count)>();
        foreach (var cg in courseGrades)
        {
            if (studentAverages.TryGetValue(cg.StudentNumber, out var current))
            {
                studentAverages[cg.StudentNumber] = (current.Sum + cg.Score, current.Count + 1);
            }
            else
            {
                studentAverages[cg.StudentNumber] = (cg.Score, 1);
            }
        }
        return studentAverages;
    }

    private static List<(int stId, double avg)> FindTop3Averages(Dictionary<int, (double Sum, int Count)> studentAverages)
    {
        return studentAverages
            .Select(kvp => (stId: kvp.Key, avg: kvp.Value.Sum / kvp.Value.Count))
            .OrderByDescending(tup => tup.avg)
            .Take(TopCount)
            .ToList();
    }

    private static IEnumerable<StudentGpa> GetCorrespondingStudentsInfo(List<(int stId, double avg)> top3, List<Student> studentsData)
    {
        return top3.Join(studentsData,
            sc => sc.stId,
            st => st.StudentNumber,
            (gpa, student) => new StudentGpa
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                Average = gpa.avg
            });
    }
}
