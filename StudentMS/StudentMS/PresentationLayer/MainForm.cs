using StudentMS.DataLayer;
using StudentMS.LogicLayer;

namespace StudentMS.PresentationLayer;

public class MainForm : Form
{
    private readonly StudentService _service = new();
    private Panel _sidePanel = null!;
    private Panel _contentPanel = null!;

    public MainForm()
    {
        DatabaseHelper.InitialiseDatabase();
        InitialiseComponent();
        LoadDashboard();
    }

    private void InitialiseComponent()
    {
        Text            = "Student Management System";
        Size            = new Size(1100, 700);
        StartPosition   = FormStartPosition.CenterScreen;
        MinimumSize     = new Size(900, 600);
        BackColor       = Color.FromArgb(245, 247, 250);
        Font            = new Font("Segoe UI", 9f);

        // ── Side panel ─────────────────────────────────────────
        _sidePanel = new Panel
        {
            Width     = 220,
            Dock      = DockStyle.Left,
            BackColor = Color.FromArgb(30, 30, 50),
        };

        // App title in sidebar
        var appTitle = new Label
        {
            Text      = "StudentMS",
            ForeColor = Color.White,
            Font      = new Font("Segoe UI", 16f, FontStyle.Bold),
            Bounds    = new Rectangle(0, 20, 220, 50),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var appSub = new Label
        {
            Text      = "Management System",
            ForeColor = Color.FromArgb(150, 150, 180),
            Font      = new Font("Segoe UI", 8f),
            Bounds    = new Rectangle(0, 60, 220, 20),
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Nav buttons
        var btnDashboard = CreateNavButton("  Dashboard",  80);
        var btnStudents  = CreateNavButton("  Students",  130);
        var btnGrades    = CreateNavButton("  Grades",    180);
        var btnSearch    = CreateNavButton("  Search",    230);

        btnDashboard.Click += (s, e) => { LoadDashboard(); HighlightNav(btnDashboard); };
        btnStudents.Click  += (s, e) => { LoadStudentList(); HighlightNav(btnStudents); };
        btnGrades.Click    += (s, e) => { LoadGradesPanel(); HighlightNav(btnGrades); };
        btnSearch.Click    += (s, e) => { LoadSearchPanel(); HighlightNav(btnSearch); };

        // Version at bottom
        var lblVersion = new Label
        {
            Text      = "v1.0  |  SQLite + C#",
            ForeColor = Color.FromArgb(100, 100, 130),
            Font      = new Font("Segoe UI", 7.5f),
            Dock      = DockStyle.Bottom,
            Height    = 30,
            TextAlign = ContentAlignment.MiddleCenter
        };

        _sidePanel.Controls.AddRange(new Control[]
            { appTitle, appSub, btnDashboard, btnStudents, btnGrades, btnSearch, lblVersion });

        // ── Content panel ──────────────────────────────────────
        _contentPanel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = Color.FromArgb(245, 247, 250),
            Padding   = new Padding(24)
        };

        Controls.Add(_contentPanel);
        Controls.Add(_sidePanel);

        HighlightNav(btnDashboard);
    }

    private Button CreateNavButton(string text, int top)
    {
        return new Button
        {
            Text      = text,
            Bounds    = new Rectangle(0, top, 220, 44),
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.FromArgb(180, 180, 210),
            BackColor = Color.Transparent,
            Font      = new Font("Segoe UI", 10f),
            TextAlign = ContentAlignment.MiddleLeft,
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };
    }

    private void HighlightNav(Button active)
    {
        foreach (Control c in _sidePanel.Controls)
        {
            if (c is Button b)
            {
                b.BackColor = Color.Transparent;
                b.ForeColor = Color.FromArgb(180, 180, 210);
            }
        }
        active.BackColor = Color.FromArgb(60, 60, 100);
        active.ForeColor = Color.White;
    }

    // ── Dashboard ──────────────────────────────────────────────
    private void LoadDashboard()
    {
        _contentPanel.Controls.Clear();
        var stats = _service.GetDashboardStats();

        var title = new Label
        {
            Text      = "Dashboard",
            Font      = new Font("Segoe UI", 18f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 30, 50),
            Bounds    = new Rectangle(0, 0, 600, 40),
        };

        var sub = new Label
        {
            Text      = "Welcome back, Mzingeli  |  " + DateTime.Today.ToString("dddd, dd MMMM yyyy"),
            Font      = new Font("Segoe UI", 9f),
            ForeColor = Color.Gray,
            Bounds    = new Rectangle(0, 42, 600, 22),
        };

        // Stat cards
        var cardTotal   = CreateStatCard("Total Students",  stats.TotalStudents.ToString(),  Color.FromArgb(79, 70, 229),  0);
        var cardActive  = CreateStatCard("Active Students", stats.ActiveStudents.ToString(),  Color.FromArgb(16, 185, 129), 200);
        var cardSWE     = CreateStatCard("Software Eng.",   stats.SWEStudents.ToString(),     Color.FromArgb(245, 158, 11), 400);
        var cardData    = CreateStatCard("Data Science",    stats.DataStudents.ToString(),    Color.FromArgb(239, 68, 68),  600);

        // Year breakdown panel
        var yearPanel = new Panel
        {
            Bounds    = new Rectangle(0, 200, 820, 120),
            BackColor = Color.White,
        };
        yearPanel.Paint += (s, e) =>
        {
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(230, 230, 240)), 0, 0, yearPanel.Width - 1, yearPanel.Height - 1);
        };

        var yearTitle = new Label
        {
            Text      = "Students by Year of Study",
            Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 30, 50),
            Bounds    = new Rectangle(16, 12, 300, 24),
        };

        var yr1 = CreateYearBadge($"Year 1:  {stats.Year1Count}", 16,  50, Color.FromArgb(79, 70, 229));
        var yr2 = CreateYearBadge($"Year 2:  {stats.Year2Count}", 196, 50, Color.FromArgb(16, 185, 129));
        var yr3 = CreateYearBadge($"Year 3:  {stats.Year3Count}", 376, 50, Color.FromArgb(245, 158, 11));

        yearPanel.Controls.AddRange(new Control[] { yearTitle, yr1, yr2, yr3 });

        // Quick actions
        var btnAddStudent = CreateActionButton("+ Add New Student", 0,   340);
        var btnViewAll    = CreateActionButton("View All Students", 200, 340);

        btnAddStudent.Click += (s, e) => ShowAddStudentForm();
        btnViewAll.Click    += (s, e) => { LoadStudentList(); };

        _contentPanel.Controls.AddRange(new Control[]
            { title, sub, cardTotal, cardActive, cardSWE, cardData,
              yearPanel, btnAddStudent, btnViewAll });
    }

    private Panel CreateStatCard(string label, string value, Color accent, int left)
    {
        var card = new Panel
        {
            Bounds    = new Rectangle(left, 80, 180, 100),
            BackColor = Color.White,
        };
        card.Paint += (s, e) =>
        {
            e.Graphics.FillRectangle(new SolidBrush(accent), 0, 0, 5, card.Height);
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(230, 230, 240)), 0, 0, card.Width - 1, card.Height - 1);
        };

        var valLabel = new Label
        {
            Text      = value,
            Font      = new Font("Segoe UI", 26f, FontStyle.Bold),
            ForeColor = accent,
            Bounds    = new Rectangle(14, 18, 160, 48),
        };

        var nameLabel = new Label
        {
            Text      = label,
            Font      = new Font("Segoe UI", 9f),
            ForeColor = Color.Gray,
            Bounds    = new Rectangle(14, 66, 160, 22),
        };

        card.Controls.AddRange(new Control[] { valLabel, nameLabel });
        return card;
    }

    private Label CreateYearBadge(string text, int left, int top, Color color)
    {
        return new Label
        {
            Text      = text,
            Bounds    = new Rectangle(left, top, 160, 36),
            Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = color,
            BackColor = Color.FromArgb(240, 240, 255),
            TextAlign = ContentAlignment.MiddleCenter
        };
    }

    private Button CreateActionButton(string text, int left, int top)
    {
        return new Button
        {
            Text      = text,
            Bounds    = new Rectangle(left, top, 180, 42),
            BackColor = Color.FromArgb(79, 70, 229),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };
    }

    // ── Student List Panel ─────────────────────────────────────
    private void LoadStudentList(List<Models.Student>? students = null)
    {
        _contentPanel.Controls.Clear();
        students ??= _service.GetAllStudents();

        var title = new Label
        {
            Text      = "All Students",
            Font      = new Font("Segoe UI", 18f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 30, 50),
            Bounds    = new Rectangle(0, 0, 400, 40),
        };

        var btnAdd = new Button
        {
            Text      = "+ Add Student",
            Bounds    = new Rectangle(600, 2, 140, 36),
            BackColor = Color.FromArgb(79, 70, 229),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };
        btnAdd.Click += (s, e) => ShowAddStudentForm();

        // DataGridView
        var grid = new DataGridView
        {
            Bounds                = new Rectangle(0, 50, 820, 470),
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

        grid.ColumnHeadersDefaultCellStyle.BackColor         = Color.FromArgb(30, 30, 50);
        grid.ColumnHeadersDefaultCellStyle.ForeColor         = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font              = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 30, 50);
        grid.ColumnHeadersDefaultCellStyle.Padding           = new Padding(8, 0, 0, 0);
        grid.AlternatingRowsDefaultCellStyle.BackColor       = Color.FromArgb(248, 248, 253);
        grid.DefaultCellStyle.SelectionBackColor             = Color.FromArgb(79, 70, 229);
        grid.DefaultCellStyle.SelectionForeColor             = Color.White;
        grid.DefaultCellStyle.Padding                        = new Padding(8, 0, 0, 0);
        grid.EnableHeadersVisualStyles                       = false;

        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Student No.", DataPropertyName = "StudentNumber", FillWeight = 14, MinimumWidth = 100 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Full Name",   DataPropertyName = "FullName",      FillWeight = 22, MinimumWidth = 140 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Email",       DataPropertyName = "Email",         FillWeight = 28, MinimumWidth = 160 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Course",      DataPropertyName = "Course",        FillWeight = 22, MinimumWidth = 140 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Year",        DataPropertyName = "YearOfStudy",   FillWeight = 7,  MinimumWidth = 50  });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status",      DataPropertyName = "Status",        FillWeight = 10, MinimumWidth = 80  });

        grid.DataSource = students;

        // Color-code the Status column
        grid.CellFormatting += (s, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 5) return;
            var status = e.Value?.ToString();
            e.CellStyle.ForeColor = status switch
            {
                "Active"    => Color.FromArgb(16, 185, 129),
                "Suspended" => Color.FromArgb(239, 68, 68),
                "Graduated" => Color.FromArgb(79, 70, 229),
                _           => Color.Gray
            };
            e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        };

        // Action buttons below grid
        var btnEdit   = new Button { Text = "✎  Edit",         Bounds = new Rectangle(0,   530, 140, 38), BackColor = Color.FromArgb(245, 158, 11), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), FlatAppearance = { BorderSize = 0 } };
        var btnDelete = new Button { Text = "✕  Delete",       Bounds = new Rectangle(150, 530, 140, 38), BackColor = Color.FromArgb(239, 68, 68),  ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), FlatAppearance = { BorderSize = 0 } };
        var btnGrades = new Button { Text = "≡  View Grades",  Bounds = new Rectangle(300, 530, 150, 38), BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), FlatAppearance = { BorderSize = 0 } };

        btnEdit.Click += (s, e) =>
        {
            if (grid.CurrentRow?.DataBoundItem is Models.Student student)
                ShowEditStudentForm(student);
        };

        btnDelete.Click += (s, e) =>
        {
            if (grid.CurrentRow?.DataBoundItem is Models.Student student)
            {
                var confirm = MessageBox.Show(
                    $"Are you sure you want to delete {student.FullName}?\nThis will also delete all their grades.",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    var result = _service.DeleteStudent(student.Id);
                    MessageBox.Show(result.Message, result.Success ? "Success" : "Error",
                        MessageBoxButtons.OK, result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
                    if (result.Success) LoadStudentList();
                }
            }
        };

        btnGrades.Click += (s, e) =>
        {
            if (grid.CurrentRow?.DataBoundItem is Models.Student student)
                ShowGradesForm(student);
        };

        _contentPanel.Controls.AddRange(new Control[]
            { title, btnAdd, grid, btnEdit, btnDelete, btnGrades });
    }

    // ── Search Panel ───────────────────────────────────────────
    private void LoadSearchPanel()
    {
        _contentPanel.Controls.Clear();

        var title = new Label
        {
            Text      = "Search Students",
            Font      = new Font("Segoe UI", 18f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 30, 50),
            Bounds    = new Rectangle(0, 0, 400, 40),
        };

        var searchBox = new TextBox
        {
            Bounds      = new Rectangle(0, 52, 400, 32),
            Font        = new Font("Segoe UI", 11f),
            PlaceholderText = "Search by name, email, student number or course...",
        };

        var btnSearch = new Button
        {
            Text      = "Search",
            Bounds    = new Rectangle(410, 52, 100, 32),
            BackColor = Color.FromArgb(79, 70, 229),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor    = Cursors.Hand,
            FlatAppearance = { BorderSize = 0 }
        };

        var resultPanel = new Panel { Bounds = new Rectangle(0, 96, 780, 480) };

        Action doSearch = () =>
        {
            var results = _service.SearchStudents(searchBox.Text);
            resultPanel.Controls.Clear();
            LoadStudentList(results);
        };

        btnSearch.Click  += (s, e) => doSearch();
        searchBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) doSearch(); };

        _contentPanel.Controls.AddRange(new Control[] { title, searchBox, btnSearch });

        // Auto-load all on open
        LoadStudentList(_service.GetAllStudents());
    }

    // ── Grades panel ───────────────────────────────────────────
    private void LoadGradesPanel()
    {
        _contentPanel.Controls.Clear();
        var title = new Label
        {
            Text      = "Grades — Select a student first",
            Font      = new Font("Segoe UI", 18f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 30, 50),
            Bounds    = new Rectangle(0, 0, 600, 40),
        };
        var hint = new Label
        {
            Text      = "Go to Students → select a student → click 'View Grades'",
            Font      = new Font("Segoe UI", 10f),
            ForeColor = Color.Gray,
            Bounds    = new Rectangle(0, 48, 600, 24),
        };
        var btnGo = CreateActionButton("Go to Students", 0, 88);
        btnGo.Click += (s, e) => LoadStudentList();
        _contentPanel.Controls.AddRange(new Control[] { title, hint, btnGo });
    }

    // ── Add Student Form ───────────────────────────────────────
    private void ShowAddStudentForm()
    {
        var form = new StudentForm(_service, null);
        if (form.ShowDialog() == DialogResult.OK)
            LoadStudentList();
    }

    private void ShowEditStudentForm(Models.Student student)
    {
        var form = new StudentForm(_service, student);
        if (form.ShowDialog() == DialogResult.OK)
            LoadStudentList();
    }

    private void ShowGradesForm(Models.Student student)
    {
        var form = new GradesForm(_service, student);
        form.ShowDialog();
    }
}
