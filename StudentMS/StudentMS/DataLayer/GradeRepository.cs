using Microsoft.Data.Sqlite;
using StudentMS.Models;

namespace StudentMS.DataLayer;

public class GradeRepository
{
    private readonly string _connectionString = DatabaseHelper.ConnectionString;

    public List<Grade> GetByStudentId(int studentId)
    {
        var grades = new List<Grade>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Grades WHERE StudentId = @sid ORDER BY Year DESC, Semester;";
        cmd.Parameters.AddWithValue("@sid", studentId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            grades.Add(MapGrade(reader));
        return grades;
    }

    public int Add(Grade grade)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Grades (StudentId, ModuleName, ModuleCode, Mark, Year, Semester)
            VALUES (@sid, @mname, @mcode, @mark, @year, @sem);
            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("@sid",   grade.StudentId);
        cmd.Parameters.AddWithValue("@mname", grade.ModuleName);
        cmd.Parameters.AddWithValue("@mcode", grade.ModuleCode);
        cmd.Parameters.AddWithValue("@mark",  grade.Mark);
        cmd.Parameters.AddWithValue("@year",  grade.Year);
        cmd.Parameters.AddWithValue("@sem",   grade.Semester);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(Grade grade)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            UPDATE Grades SET
                ModuleName = @mname, ModuleCode = @mcode,
                Mark = @mark, Year = @year, Semester = @sem
            WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@mname", grade.ModuleName);
        cmd.Parameters.AddWithValue("@mcode", grade.ModuleCode);
        cmd.Parameters.AddWithValue("@mark",  grade.Mark);
        cmd.Parameters.AddWithValue("@year",  grade.Year);
        cmd.Parameters.AddWithValue("@sem",   grade.Semester);
        cmd.Parameters.AddWithValue("@id",    grade.Id);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM Grades WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public double GetAverageByStudent(int studentId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT AVG(Mark) FROM Grades WHERE StudentId = @sid;";
        cmd.Parameters.AddWithValue("@sid", studentId);
        var result = cmd.ExecuteScalar();
        return result == DBNull.Value || result == null ? 0 : Convert.ToDouble(result);
    }

    private static Grade MapGrade(SqliteDataReader r) => new()
    {
        Id         = r.GetInt32(r.GetOrdinal("Id")),
        StudentId  = r.GetInt32(r.GetOrdinal("StudentId")),
        ModuleName = r.GetString(r.GetOrdinal("ModuleName")),
        ModuleCode = r.GetString(r.GetOrdinal("ModuleCode")),
        Mark       = r.GetDouble(r.GetOrdinal("Mark")),
        Year       = r.GetInt32(r.GetOrdinal("Year")),
        Semester   = r.GetString(r.GetOrdinal("Semester")),
    };
}
