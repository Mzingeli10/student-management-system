using StudentMS.DataLayer;
using StudentMS.Models;
using System.Text.RegularExpressions;

namespace StudentMS.LogicLayer;

public class StudentService
{
    private readonly StudentRepository _studentRepo = new();
    private readonly GradeRepository _gradeRepo = new();

    // ── Student CRUD ───────────────────────────────────────────
    public List<Student> GetAllStudents() => _studentRepo.GetAll();

    public List<Student> SearchStudents(string term) =>
        string.IsNullOrWhiteSpace(term) ? _studentRepo.GetAll() : _studentRepo.Search(term);

    public Student? GetStudent(int id) => _studentRepo.GetById(id);

    public (bool Success, string Message) AddStudent(Student student)
    {
        var validation = ValidateStudent(student);
        if (!validation.Valid) return (false, validation.Message);

        try
        {
            student.StudentNumber = GenerateStudentNumber();
            _studentRepo.Add(student);
            return (true, $"Student {student.FullName} added successfully with number {student.StudentNumber}.");
        }
        catch (Exception ex) when (ex.Message.Contains("UNIQUE"))
        {
            return (false, "A student with this email already exists.");
        }
        catch (Exception ex)
        {
            return (false, $"Error adding student: {ex.Message}");
        }
    }

    public (bool Success, string Message) UpdateStudent(Student student)
    {
        var validation = ValidateStudent(student);
        if (!validation.Valid) return (false, validation.Message);

        try
        {
            _studentRepo.Update(student);
            return (true, $"Student {student.FullName} updated successfully.");
        }
        catch (Exception ex)
        {
            return (false, $"Error updating student: {ex.Message}");
        }
    }

    public (bool Success, string Message) DeleteStudent(int id)
    {
        var student = _studentRepo.GetById(id);
        if (student == null) return (false, "Student not found.");
        _studentRepo.Delete(id);
        return (true, $"Student {student.FullName} deleted successfully.");
    }

    // ── Grades ─────────────────────────────────────────────────
    public List<Grade> GetStudentGrades(int studentId) =>
        _gradeRepo.GetByStudentId(studentId);

    public (bool Success, string Message) AddGrade(Grade grade)
    {
        if (string.IsNullOrWhiteSpace(grade.ModuleName))
            return (false, "Module name is required.");
        if (grade.Mark < 0 || grade.Mark > 100)
            return (false, "Mark must be between 0 and 100.");

        _gradeRepo.Add(grade);
        return (true, "Grade added successfully.");
    }

    public (bool Success, string Message) UpdateGrade(Grade grade)
    {
        if (grade.Mark < 0 || grade.Mark > 100)
            return (false, "Mark must be between 0 and 100.");
        _gradeRepo.Update(grade);
        return (true, "Grade updated successfully.");
    }

    public void DeleteGrade(int gradeId) => _gradeRepo.Delete(gradeId);

    public double GetStudentAverage(int studentId) =>
        _gradeRepo.GetAverageByStudent(studentId);

    // ── Dashboard stats ────────────────────────────────────────
    public DashboardStats GetDashboardStats()
    {
        var students = _studentRepo.GetAll();
        return new DashboardStats
        {
            TotalStudents   = students.Count,
            ActiveStudents  = students.Count(s => s.Status == "Active"),
            SWEStudents     = students.Count(s => s.Course.Contains("Software")),
            DataStudents    = students.Count(s => s.Course.Contains("Data")),
            OtherStudents   = students.Count(s => !s.Course.Contains("Software") && !s.Course.Contains("Data")),
            Year1Count      = students.Count(s => s.YearOfStudy == 1),
            Year2Count      = students.Count(s => s.YearOfStudy == 2),
            Year3Count      = students.Count(s => s.YearOfStudy == 3),
        };
    }

    // ── Private helpers ────────────────────────────────────────
    private string GenerateStudentNumber()
    {
        var count = _studentRepo.GetTotalCount() + 1;
        return $"60{count:D4}";
    }

    private static (bool Valid, string Message) ValidateStudent(Student s)
    {
        if (string.IsNullOrWhiteSpace(s.FirstName)) return (false, "First name is required.");
        if (string.IsNullOrWhiteSpace(s.LastName))  return (false, "Last name is required.");
        if (string.IsNullOrWhiteSpace(s.Email))     return (false, "Email is required.");
        if (!Regex.IsMatch(s.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return (false, "Please enter a valid email address.");
        if (string.IsNullOrWhiteSpace(s.Course))    return (false, "Course is required.");
        if (s.YearOfStudy < 1 || s.YearOfStudy > 6) return (false, "Year of study must be between 1 and 6.");
        if (s.DateOfBirth > DateTime.Today.AddYears(-15))
            return (false, "Student must be at least 15 years old.");
        return (true, string.Empty);
    }
}

public class DashboardStats
{
    public int TotalStudents  { get; set; }
    public int ActiveStudents { get; set; }
    public int SWEStudents    { get; set; }
    public int DataStudents   { get; set; }
    public int OtherStudents  { get; set; }
    public int Year1Count     { get; set; }
    public int Year2Count     { get; set; }
    public int Year3Count     { get; set; }
}
