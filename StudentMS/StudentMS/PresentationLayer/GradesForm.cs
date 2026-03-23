using StudentMS.LogicLayer;
using StudentMS.Models;

namespace StudentMS.PresentationLayer;

public class GradesForm : Form
{
    private readonly StudentService _service;
    private readonly Student _student;
    private DataGridView _grid = null!;
    private Label _lblAverage = null!;

    public GradesForm(StudentService service, Student student)
    {
        _service = service;
        _student = student;
        InitialiseComponent();
        LoadGrades();
    }

    private void InitialiseComponent()
    {
        Text          = $"Grades — {_student.FullName} ({_student.StudentNumber})";
        Size          = new Size(760, 560);
        StartPosition = FormStartPosition.CenterParent;
        BackColor     = Color.White;
        Font          = new Font("Segoe UI", 9.5f);

        // Header
        var lblName = new Label
        {
            Text      = _student.FullName,
            Font      = new Font("Segoe UI", 16f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 30, 50),
            Bounds    = new Rectangle(16, 16, 500, 36),
        };

        var lblInfo = new Label
        {
            Text      = $"{_student.StudentNumber}  |  {_student.Course}  |  Year {_student.YearOfStudy}  |  {_student.Status}",
            Font      = new Font("Segoe UI", 9f),
            ForeColor = Color.Gray,
            Bounds    = new Rectangle(16, 52, 600, 22),
        };

        _lblAverage = new Label
        {
            Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = Color.FromArgb(79, 70, 229),
            Bounds    = new Rectangle(16, 76, 400, 24),
        };

        // Grid
        _grid = new DataGridView
        {
            Bounds                = new Rectangle(16, 110, 710, 340),
            BackgroundColor       = Color.White,
            BorderStyle           = BorderStyle.None,
            RowHeadersVisible     = false,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
            ReadOnly              = true,
            SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
            AutoGenerateColumns   = false,
            AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
            Font                  = new Font("Segoe UI", 9.5f),
            GridColor             = Color.FromArgb(235, 235, 245),
            ColumnHeadersHeight   = 40,
            RowTemplate           = { Height = 36 },
            CellBorderStyle       = DataGridViewCellBorderStyle.SingleHorizontal,
        };

        _grid.ColumnHeadersDefaultCellStyle.BackColor         = Color.FromArgb(30, 30, 50);
        _grid.ColumnHeadersDefaultCellStyle.ForeColor         = Color.White;
        _grid.ColumnHeadersDefaultCellStyle.Font              = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        _grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 30, 50);
        _grid.ColumnHeadersDefaultCellStyle.Padding           = new Padding(8, 0, 0, 0);
        _grid.DefaultCellStyle.SelectionBackColor             = Color.FromArgb(79, 70, 229);
        _grid.DefaultCellStyle.SelectionForeColor             = Color.White;
        _grid.DefaultCellStyle.Padding                        = new Padding(8, 0, 0, 0);
        _grid.AlternatingRowsDefaultCellStyle.BackColor       = Color.FromArgb(248, 248, 253);
        _grid.EnableHeadersVisualStyles                       = false;

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Module Code", DataPropertyName = "ModuleCode",       FillWeight = 16, MinimumWidth = 100 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Module Name", DataPropertyName = "ModuleName",       FillWeight = 34, MinimumWidth = 180 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mark (%)",    DataPropertyName = "Mark",             FillWeight = 14, MinimumWidth = 80  });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Grade",       DataPropertyName = "GradeDescription", FillWeight = 18, MinimumWidth = 100 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Year",        DataPropertyName = "Year",             FillWeight = 10, MinimumWidth = 60  });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Semester",    DataPropertyName = "Semester",         FillWeight = 10, MinimumWidth = 70  });

        // Color code the grade description column
        _grid.CellFormatting += (s, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 3) return;
            var grade = e.Value?.ToString();
            e.CellStyle.ForeColor = grade switch
            {
                "Distinction" => Color.FromArgb(16, 185, 129),
                "Merit"       => Color.FromArgb(79, 70, 229),
                "Pass"        => Color.FromArgb(59, 130, 246),
                "Partial Pass"=> Color.FromArgb(245, 158, 11),
                "Fail"        => Color.FromArgb(239, 68, 68),
                _             => Color.Gray
            };
            e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        };

        // Action buttons
        var btnAdd = new Button
        {
            Text      = "+ Add Grade",
            Bounds    = new Rectangle(16, 462, 130, 36),
            BackColor = Color.FromArgb(79, 70, 229),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };

        var btnEdit = new Button
        {
            Text      = "Edit Grade",
            Bounds    = new Rectangle(156, 462, 120, 36),
            BackColor = Color.FromArgb(245, 158, 11),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };

        var btnDelete = new Button
        {
            Text      = "Delete Grade",
            Bounds    = new Rectangle(286, 462, 130, 36),
            BackColor = Color.FromArgb(239, 68, 68),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };

        var btnClose = new Button
        {
            Text      = "Close",
            Bounds    = new Rectangle(590, 462, 100, 36),
            BackColor = Color.FromArgb(230, 230, 240),
            ForeColor = Color.FromArgb(60, 60, 80),
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f),
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };

        btnAdd.Click += (s, e) => ShowAddGradeForm();
        btnEdit.Click += (s, e) =>
        {
            if (_grid.CurrentRow?.DataBoundItem is Grade g) ShowEditGradeForm(g);
        };
        btnDelete.Click += (s, e) =>
        {
            if (_grid.CurrentRow?.DataBoundItem is Grade g)
            {
                var confirm = MessageBox.Show($"Delete grade for {g.ModuleName}?",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    _service.DeleteGrade(g.Id);
                    LoadGrades();
                }
            }
        };
        btnClose.Click += (s, e) => Close();

        Controls.AddRange(new Control[]
            { lblName, lblInfo, _lblAverage, _grid, btnAdd, btnEdit, btnDelete, btnClose });
    }

    private void LoadGrades()
    {
        var grades  = _service.GetStudentGrades(_student.Id);
        var average = _service.GetStudentAverage(_student.Id);
        _grid.DataSource = grades;
        _lblAverage.Text = average > 0
            ? $"Average Mark: {average:F1}%  —  Overall: {GetOverallGrade(average)}"
            : "No grades recorded yet.";
    }

    private void ShowAddGradeForm()
    {
        var form = new GradeEntryForm(_service, _student.Id, null);
        if (form.ShowDialog() == DialogResult.OK) LoadGrades();
    }

    private void ShowEditGradeForm(Grade grade)
    {
        var form = new GradeEntryForm(_service, _student.Id, grade);
        if (form.ShowDialog() == DialogResult.OK) LoadGrades();
    }

    private static string GetOverallGrade(double avg) => avg switch
    {
        >= 75 => "Distinction",
        >= 60 => "Merit",
        >= 50 => "Pass",
        >= 40 => "Partial Pass",
        _     => "Fail"
    };
}
