using System.Text.Json;

namespace Phase01;

public class Program
{
    private const int TopCount = 3;
    private const string ScoresDataPath = "Data/scores.json";
    private const string StudentsDataPath = "Data/students.json";
    static void Main(string[] args)
    {

        var (courseGrades, studentsData) = FetchData();

        var studentAverages = PreComputeStudentAverages(courseGrades);

        var top3 = FindTop3Averages(studentAverages);

        var StudentsGpa = GetCorrespondingStudentsInfo(top3, studentsData);

        int rank = 1;
        foreach (var student in StudentsGpa)
        {
            Console.WriteLine($"{rank++}: Student {student.FirstName} {student.LastName} with average score {student.Average:F2}.");
        }
    }

    private static (List<CourseGrade> courseGrades, List<Student> studentsData) FetchData()
    {
        var scoresJson = File.ReadAllText(ScoresDataPath);
        var grades = JsonSerializer.Deserialize<List<CourseGrade>>(scoresJson)!;

        var studentsJson = File.ReadAllText(StudentsDataPath);
        var studentsInfo = JsonSerializer.Deserialize<List<Student>>(studentsJson)!;

        return (grades, studentsInfo);
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
