namespace StudentMS.Models;

public class Grade
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string ModuleName { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
    public double Mark { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public string Semester { get; set; } = "1";

    // OOP computed properties
    public string LetterGrade => Mark switch
    {
        >= 75 => "D",  // Distinction
        >= 60 => "M",  // Merit
        >= 50 => "P",  // Pass
        >= 40 => "PP", // Partial Pass
        _     => "F"   // Fail
    };

    public string GradeDescription => Mark switch
    {
        >= 75 => "Distinction",
        >= 60 => "Merit",
        >= 50 => "Pass",
        >= 40 => "Partial Pass",
        _     => "Fail"
    };

    public bool IsPassing => Mark >= 50;
}
