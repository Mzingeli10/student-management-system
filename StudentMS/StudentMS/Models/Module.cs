namespace StudentMS.Models;

public class Module
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Lecturer { get; set; } = string.Empty;
    public int Year { get; set; } = 1;
    public string Semester { get; set; } = "1";

    public override string ToString() => $"{Code} - {Name}";
}
