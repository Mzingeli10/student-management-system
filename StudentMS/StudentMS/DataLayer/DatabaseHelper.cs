using Microsoft.Data.Sqlite;

namespace StudentMS.DataLayer;

public static class DatabaseHelper
{
    private static readonly string DbPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "StudentMS.db");

    public static string ConnectionString => $"Data Source={DbPath}";

    public static void InitialiseDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();

        // Students table
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Students (
                Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName       TEXT NOT NULL,
                LastName        TEXT NOT NULL,
                Email           TEXT NOT NULL UNIQUE,
                Phone           TEXT,
                StudentNumber   TEXT NOT NULL UNIQUE,
                DateOfBirth     TEXT NOT NULL,
                EnrollmentDate  TEXT NOT NULL,
                Course          TEXT NOT NULL,
                YearOfStudy     INTEGER NOT NULL DEFAULT 1,
                Status          TEXT NOT NULL DEFAULT 'Active'
            );";
        cmd.ExecuteNonQuery();

        // Modules table
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Modules (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                Code        TEXT NOT NULL UNIQUE,
                Name        TEXT NOT NULL,
                Credits     INTEGER NOT NULL DEFAULT 15,
                Lecturer    TEXT,
                Year        INTEGER NOT NULL DEFAULT 1,
                Semester    TEXT NOT NULL DEFAULT '1'
            );";
        cmd.ExecuteNonQuery();

        // Grades table
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Grades (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                StudentId   INTEGER NOT NULL,
                ModuleName  TEXT NOT NULL,
                ModuleCode  TEXT NOT NULL,
                Mark        REAL NOT NULL,
                Year        INTEGER NOT NULL,
                Semester    TEXT NOT NULL DEFAULT '1',
                FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE CASCADE
            );";
        cmd.ExecuteNonQuery();

        SeedDemoData(connection);
    }

    private static void SeedDemoData(SqliteConnection connection)
    {
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Students;";
        var count = (long)(cmd.ExecuteScalar() ?? 0);
        if (count > 0) return; // Already seeded

        // Seed demo students
        var students = new[]
        {
            ("Mzingeli", "Ncube",     "mzingeli@student.ac.za",   "083 111 2222", "601727", "2003-05-14", "Software Engineering", 3),
            ("Boitumelo","Mathabathe","boitu@student.ac.za",       "082 333 4444", "600002", "2002-08-22", "Data Science",         3),
            ("Lebo",     "Dlamini",   "lebo@student.ac.za",        "071 555 6666", "600003", "2004-01-10", "Software Engineering", 2),
            ("Sipho",    "Nkosi",     "sipho@student.ac.za",       "063 777 8888", "600004", "2003-11-30", "Cybersecurity",        2),
            ("Aisha",    "Patel",     "aisha@student.ac.za",       "074 999 0000", "600005", "2005-03-05", "Software Engineering", 1),
        };

        foreach (var (fn, ln, email, phone, num, dob, course, year) in students)
        {
            cmd.CommandText = @"
                INSERT INTO Students (FirstName, LastName, Email, Phone, StudentNumber, DateOfBirth, EnrollmentDate, Course, YearOfStudy, Status)
                VALUES (@fn, @ln, @email, @phone, @num, @dob, @enroll, @course, @year, 'Active');";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@fn", fn);
            cmd.Parameters.AddWithValue("@ln", ln);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@num", num);
            cmd.Parameters.AddWithValue("@dob", dob);
            cmd.Parameters.AddWithValue("@enroll", "2022-02-01");
            cmd.Parameters.AddWithValue("@course", course);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.ExecuteNonQuery();
        }

        // Seed demo modules
        var modules = new[]
        {
            ("SWE301", "Software Engineering III", 15, "Mr Solomon Ruwende", 3, "1"),
            ("DAT301", "Data Mining & ML",          15, "Ms Juanita Blignaut", 3, "1"),
            ("SWE201", "Database Systems",           15, "Mr Lourens Nel",     2, "2"),
            ("SWE101", "Introduction to Programming",15, "Ms Sarah Smith",     1, "1"),
        };

        foreach (var (code, name, credits, lecturer, year, sem) in modules)
        {
            cmd.CommandText = @"
                INSERT OR IGNORE INTO Modules (Code, Name, Credits, Lecturer, Year, Semester)
                VALUES (@code, @name, @credits, @lecturer, @year, @sem);";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@credits", credits);
            cmd.Parameters.AddWithValue("@lecturer", lecturer);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@sem", sem);
            cmd.ExecuteNonQuery();
        }

        // Seed demo grades
        var grades = new[]
        {
            (1, "Software Engineering III", "SWE301", 82.0, 2024, "1"),
            (1, "Data Mining & ML",          "DAT301", 76.5, 2024, "1"),
            (2, "Software Engineering III", "SWE301", 91.0, 2024, "1"),
            (2, "Data Mining & ML",          "DAT301", 88.0, 2024, "1"),
            (3, "Database Systems",           "SWE201", 65.0, 2024, "2"),
            (4, "Database Systems",           "SWE201", 55.0, 2024, "2"),
            (5, "Introduction to Programming","SWE101", 71.0, 2024, "1"),
        };

        foreach (var (sid, mname, mcode, mark, year, sem) in grades)
        {
            cmd.CommandText = @"
                INSERT INTO Grades (StudentId, ModuleName, ModuleCode, Mark, Year, Semester)
                VALUES (@sid, @mname, @mcode, @mark, @year, @sem);";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@sid", sid);
            cmd.Parameters.AddWithValue("@mname", mname);
            cmd.Parameters.AddWithValue("@mcode", mcode);
            cmd.Parameters.AddWithValue("@mark", mark);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@sem", sem);
            cmd.ExecuteNonQuery();
        }
    }
}
