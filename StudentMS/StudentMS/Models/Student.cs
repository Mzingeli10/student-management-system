namespace StudentMS.Models;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.Today;
    public string Course { get; set; } = string.Empty;
    public int YearOfStudy { get; set; } = 1;
    public string Status { get; set; } = "Active"; // Active, Suspended, Graduated

    // Computed properties — OOP in action
    public string FullName => $"{FirstName} {LastName}";
    public int Age => DateTime.Today.Year - DateOfBirth.Year -
                     (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
    public override string ToString() => $"{StudentNumber} - {FullName}";
}
