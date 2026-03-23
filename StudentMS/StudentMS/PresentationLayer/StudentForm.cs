using StudentMS.LogicLayer;
using StudentMS.Models;

namespace StudentMS.PresentationLayer;

public class StudentForm : Form
{
    private readonly StudentService _service;
    private readonly Student? _existing;

    private TextBox _txtFirst = null!, _txtLast = null!, _txtEmail = null!,
                    _txtPhone = null!;
    private ComboBox _cboCourse = null!, _cboYear = null!, _cboStatus = null!;
    private DateTimePicker _dtpDob = null!;

    public StudentForm(StudentService service, Student? existing)
    {
        _service  = service;
        _existing = existing;
        InitialiseComponent();
        if (existing != null) PopulateFields(existing);
    }

    private void InitialiseComponent()
    {
        Text          = _existing == null ? "Add New Student" : "Edit Student";
        Size          = new Size(480, 560);
        StartPosition = FormStartPosition.CenterParent;
        BackColor     = Color.White;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox   = false;
        MinimizeBox   = false;
        Font          = new Font("Segoe UI", 9.5f);

        int labelW = 120, inputW = 280, startX = 20, inputX = 150, y = 20, gap = 46;

        Label MakeLabel(string text, int top) => new()
        {
            Text      = text,
            Bounds    = new Rectangle(startX, top + 4, labelW, 22),
            ForeColor = Color.FromArgb(60, 60, 80),
            Font      = new Font("Segoe UI", 9.5f)
        };

        TextBox MakeInput(int top) => new()
        {
            Bounds = new Rectangle(inputX, top, inputW, 28),
            Font   = new Font("Segoe UI", 9.5f)
        };

        // Fields
        _txtFirst = MakeInput(y);
        Controls.Add(MakeLabel("First Name *", y)); Controls.Add(_txtFirst); y += gap;

        _txtLast = MakeInput(y);
        Controls.Add(MakeLabel("Last Name *", y)); Controls.Add(_txtLast); y += gap;

        _txtEmail = MakeInput(y);
        Controls.Add(MakeLabel("Email *", y)); Controls.Add(_txtEmail); y += gap;

        _txtPhone = MakeInput(y);
        Controls.Add(MakeLabel("Phone", y)); Controls.Add(_txtPhone); y += gap;

        // Date of birth
        Controls.Add(MakeLabel("Date of Birth *", y));
        _dtpDob = new DateTimePicker
        {
            Bounds   = new Rectangle(inputX, y, inputW, 28),
            Format   = DateTimePickerFormat.Short,
            Value    = DateTime.Today.AddYears(-20),
            MaxDate  = DateTime.Today.AddYears(-15)
        };
        Controls.Add(_dtpDob); y += gap;

        // Course
        Controls.Add(MakeLabel("Course *", y));
        _cboCourse = new ComboBox
        {
            Bounds        = new Rectangle(inputX, y, inputW, 28),
            DropDownStyle = ComboBoxStyle.DropDown,
            Font          = new Font("Segoe UI", 9.5f)
        };
        _cboCourse.Items.AddRange(new[] { "Software Engineering", "Data Science & BI",
            "Cybersecurity", "Information Technology", "Computer Science" });
        Controls.Add(_cboCourse); y += gap;

        // Year
        Controls.Add(MakeLabel("Year of Study *", y));
        _cboYear = new ComboBox
        {
            Bounds        = new Rectangle(inputX, y, 80, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 9.5f)
        };
        _cboYear.Items.AddRange(new object[] { 1, 2, 3, 4, 5, 6 });
        _cboYear.SelectedIndex = 0;
        Controls.Add(_cboYear); y += gap;

        // Status
        Controls.Add(MakeLabel("Status", y));
        _cboStatus = new ComboBox
        {
            Bounds        = new Rectangle(inputX, y, 150, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font          = new Font("Segoe UI", 9.5f)
        };
        _cboStatus.Items.AddRange(new[] { "Active", "Suspended", "Graduated" });
        _cboStatus.SelectedIndex = 0;
        Controls.Add(_cboStatus); y += gap + 10;

        // Buttons
        var btnSave = new Button
        {
            Text      = _existing == null ? "Add Student" : "Save Changes",
            Bounds    = new Rectangle(inputX, y, 135, 38),
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
            Bounds    = new Rectangle(inputX + 145, y, 100, 38),
            BackColor = Color.FromArgb(230, 230, 240),
            ForeColor = Color.FromArgb(60, 60, 80),
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f),
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };

        btnSave.Click   += Save;
        btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;

        Controls.AddRange(new Control[] { btnSave, btnCancel });
    }

    private void PopulateFields(Student s)
    {
        _txtFirst.Text        = s.FirstName;
        _txtLast.Text         = s.LastName;
        _txtEmail.Text        = s.Email;
        _txtPhone.Text        = s.Phone;
        _dtpDob.Value         = s.DateOfBirth;
        _cboCourse.Text       = s.Course;
        _cboYear.SelectedItem = s.YearOfStudy;
        _cboStatus.SelectedItem = s.Status;
    }

    private void Save(object? sender, EventArgs e)
    {
        var student = _existing ?? new Student();
        student.FirstName   = _txtFirst.Text.Trim();
        student.LastName    = _txtLast.Text.Trim();
        student.Email       = _txtEmail.Text.Trim();
        student.Phone       = _txtPhone.Text.Trim();
        student.DateOfBirth = _dtpDob.Value;
        student.Course      = _cboCourse.Text.Trim();
        student.YearOfStudy = _cboYear.SelectedItem != null ? (int)_cboYear.SelectedItem : 1;
        student.Status      = _cboStatus.SelectedItem?.ToString() ?? "Active";

        var result = _existing == null
            ? _service.AddStudent(student)
            : _service.UpdateStudent(student);

        if (result.Success)
        {
            MessageBox.Show(result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        }
        else
        {
            MessageBox.Show(result.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
