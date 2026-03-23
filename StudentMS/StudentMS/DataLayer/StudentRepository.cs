using Microsoft.Data.Sqlite;
using StudentMS.Models;

namespace StudentMS.DataLayer;

public class StudentRepository
{
    private readonly string _connectionString = DatabaseHelper.ConnectionString;

    public List<Student> GetAll()
    {
        var students = new List<Student>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Students ORDER BY LastName, FirstName;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            students.Add(MapStudent(reader));
        return students;
    }

    public List<Student> Search(string term)
    {
        var students = new List<Student>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM Students
            WHERE FirstName LIKE @term OR LastName LIKE @term
               OR Email LIKE @term OR StudentNumber LIKE @term
               OR Course LIKE @term
            ORDER BY LastName, FirstName;";
        cmd.Parameters.AddWithValue("@term", $"%{term}%");
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            students.Add(MapStudent(reader));
        return students;
    }

    public Student? GetById(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Students WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapStudent(reader) : null;
    }

    public int Add(Student student)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Students (FirstName, LastName, Email, Phone, StudentNumber,
                                  DateOfBirth, EnrollmentDate, Course, YearOfStudy, Status)
            VALUES (@fn, @ln, @email, @phone, @num, @dob, @enroll, @course, @year, @status);
            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@fn",     student.FirstName);
        cmd.Parameters.AddWithValue("@ln",     student.LastName);
        cmd.Parameters.AddWithValue("@email",  student.Email);
        cmd.Parameters.AddWithValue("@phone",  student.Phone);
        cmd.Parameters.AddWithValue("@num",    student.StudentNumber);
        cmd.Parameters.AddWithValue("@dob",    student.DateOfBirth.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@enroll", student.EnrollmentDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@course", student.Course);
        cmd.Parameters.AddWithValue("@year",   student.YearOfStudy);
        cmd.Parameters.AddWithValue("@status", student.Status);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Student student)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            UPDATE Students SET
                FirstName = @fn, LastName = @ln, Email = @email,
                Phone = @phone, StudentNumber = @num,
                DateOfBirth = @dob, EnrollmentDate = @enroll,
                Course = @course, YearOfStudy = @year, Status = @status
            WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@fn",     student.FirstName);
        cmd.Parameters.AddWithValue("@ln",     student.LastName);
        cmd.Parameters.AddWithValue("@email",  student.Email);
        cmd.Parameters.AddWithValue("@phone",  student.Phone);
        cmd.Parameters.AddWithValue("@num",    student.StudentNumber);
        cmd.Parameters.AddWithValue("@dob",    student.DateOfBirth.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@enroll", student.EnrollmentDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@course", student.Course);
        cmd.Parameters.AddWithValue("@year",   student.YearOfStudy);
        cmd.Parameters.AddWithValue("@status", student.Status);
        cmd.Parameters.AddWithValue("@id",     student.Id);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Students WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public int GetTotalCount() => GetScalar("SELECT COUNT(*) FROM Students;");
    public int GetActiveCount() => GetScalar("SELECT COUNT(*) FROM Students WHERE Status = 'Active';");
    public int GetCountByCourse(string course)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Students WHERE Course = @course;";
        cmd.Parameters.AddWithValue("@course", course);
        return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
    }

    private int GetScalar(string sql)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
    }

    private static Student MapStudent(SqliteDataReader r) => new()
    {
        Id             = r.GetInt32(r.GetOrdinal("Id")),
        FirstName      = r.GetString(r.GetOrdinal("FirstName")),
        LastName       = r.GetString(r.GetOrdinal("LastName")),
        Email          = r.GetString(r.GetOrdinal("Email")),
        Phone          = r.IsDBNull(r.GetOrdinal("Phone")) ? "" : r.GetString(r.GetOrdinal("Phone")),
        StudentNumber  = r.GetString(r.GetOrdinal("StudentNumber")),
        DateOfBirth    = DateTime.Parse(r.GetString(r.GetOrdinal("DateOfBirth"))),
        EnrollmentDate = DateTime.Parse(r.GetString(r.GetOrdinal("EnrollmentDate"))),
        Course         = r.GetString(r.GetOrdinal("Course")),
        YearOfStudy    = r.GetInt32(r.GetOrdinal("YearOfStudy")),
        Status         = r.GetString(r.GetOrdinal("Status")),
    };
}
