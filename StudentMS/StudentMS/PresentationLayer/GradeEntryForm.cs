using StudentMS.LogicLayer;
using StudentMS.Models;

namespace StudentMS.PresentationLayer;

public class GradeEntryForm : Form
{
    private readonly StudentService _service;
    private readonly int _studentId;
    private readonly Grade? _existing;

    private TextBox _txtModuleCode = null!, _txtModuleName = null!;
    private NumericUpDown _numMark = null!;
    private ComboBox _cboSemester = null!;
    private NumericUpDown _numYear = null!;

    public GradeEntryForm(StudentService service, int studentId, Grade? existing)
    {
        _service   = service;
        _studentId = studentId;
        _existing  = existing;
        InitialiseComponent();
        if (existing != null) PopulateFields(existing);
    }

    private void InitialiseComponent()
    {
        Text          = _existing == null ? "Add Grade" : "Edit Grade";
        Size          = new Size(420, 380);
        StartPosition = FormStartPosition.CenterParent;
        BackColor     = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox   = false;
        Font          = new Font("Segoe UI", 9.5f);

        int inputX = 150, inputW = 220, y = 20, gap = 46;

        Label MakeLabel(string text, int top) => new()
        {
            Text      = text,
            Bounds    = new Rectangle(20, top + 4, 128, 22),
            ForeColor = Color.FromArgb(60, 60, 80)
        };

        TextBox MakeTxt(int top) => new()
        {
            Bounds = new Rectangle(inputX, top, inputW, 28),
            Font   = new Font("Segoe UI", 9.5f)
        };

        Controls.Add(MakeLabel("Module Code *", y));
        _txtModuleCode = MakeTxt(y); Controls.Add(_txtModuleCode); y += gap;

        Controls.Add(MakeLabel("Module Name *", y));
        _txtModuleName = MakeTxt(y); Controls.Add(_txtModuleName); y += gap;

        Controls.Add(MakeLabel("Mark (0-100) *", y));
        _numMark = new NumericUpDown
        {
            Bounds        = new Rectangle(inputX, y, 100, 28),
            Minimum       = 0,
            Maximum       = 100,
            DecimalPlaces = 1,
            Font          = new Font("Segoe UI", 9.5f)
        };
        Controls.Add(_numMark); y += gap;

        Controls.Add(MakeLabel("Year *", y));
        _numYear = new NumericUpDown
        {
            Bounds  = new Rectangle(inputX, y, 100, 28),
            Minimum = 2020,
            Maximum = 2030,
            Value   = DateTime.Today.Year,
            Font    = new Font("Segoe UI", 9.5f)
        };
        Controls.Add(_numYear); y += gap;

        Controls.Add(MakeLabel("Semester *", y));
        _cboSemester = new ComboBox
        {
            Bounds        = new Rectangle(inputX, y, 100, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 9.5f)
        };
        _cboSemester.Items.AddRange(new[] { "1", "2" });
        _cboSemester.SelectedIndex = 0;
        Controls.Add(_cboSemester); y += gap + 10;

        var btnSave = new Button
        {
            Text      = _existing == null ? "Add Grade" : "Save Changes",
            Bounds    = new Rectangle(inputX, y, 130, 38),
            BackColor = Color.FromArgb(79, 70, 229),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };

        var btnCancel = new Button
        {
            Text      = "Cancel",
            Bounds    = new Rectangle(inputX + 140, y, 90, 38),
            BackColor = Color.FromArgb(230, 230, 240),
            ForeColor = Color.FromArgb(60, 60, 80),
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };

        btnSave.Click   += Save;
        btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        Controls.AddRange(new Control[] { btnSave, btnCancel });
    }

    private void PopulateFields(Grade g)
    {
        _txtModuleCode.Text         = g.ModuleCode;
        _txtModuleName.Text         = g.ModuleName;
        _numMark.Value              = (decimal)g.Mark;
        _numYear.Value              = g.Year;
        _cboSemester.SelectedItem   = g.Semester;
    }

    private void Save(object? sender, EventArgs e)
    {
        var grade = _existing ?? new Grade { StudentId = _studentId };
        grade.ModuleCode = _txtModuleCode.Text.Trim();
        grade.ModuleName = _txtModuleName.Text.Trim();
        grade.Mark       = (double)_numMark.Value;
        grade.Year       = (int)_numYear.Value;
        grade.Semester   = _cboSemester.SelectedItem?.ToString() ?? "1";

        var result = _existing == null
            ? _service.AddGrade(grade)
            : _service.UpdateGrade(grade);

        if (result.Success)
        {
            MessageBox.Show(result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
        else
        {
            MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
