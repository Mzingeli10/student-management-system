# Student Management System

A desktop application built with **C# Windows Forms** and **SQLite**, demonstrating a clean 3-layer architecture with full OOP principles. Built as part of a Software Engineering portfolio.

---

## Screenshots

> *Dashboard with live student statistics, student list with CRUD operations, and grade management per student.*

---

## Architecture

The project is structured into 4 distinct layers — each layer only communicates with the one directly below it. The GUI never touches the database directly.

```
StudentMS/
├── Models/                  ← OOP domain classes
│   ├── Student.cs           ← Student entity with computed properties
│   ├── Grade.cs             ← Grade with automatic letter grade calculation
│   └── Module.cs            ← Module/subject entity
│
├── DataLayer/               ← Database access only (SQLite)
│   ├── DatabaseHelper.cs    ← Creates tables on startup, seeds demo data
│   ├── StudentRepository.cs ← All student SQL operations (CRUD + search)
│   └── GradeRepository.cs   ← All grade SQL operations
│
├── LogicLayer/              ← Business rules and validation
│   └── StudentService.cs    ← Validates inputs, generates student numbers,
│                               computes dashboard stats
│
└── PresentationLayer/       ← GUI forms (Windows Forms)
    ├── MainForm.cs          ← Dashboard + navigation sidebar
    ├── StudentForm.cs       ← Add / Edit student dialog
    ├── GradesForm.cs        ← View all grades for a student
    └── GradeEntryForm.cs    ← Add / Edit a single grade entry
```

---

## Features

- **Dashboard** — live stats showing total students, active students, students per course, and year-of-study breakdown
- **Student Management** — add, edit, delete students with full input validation
- **Grade Management** — record and manage grades per student with automatic letter grade and average calculation
- **Search** — search students by name, email, student number or course
- **Auto-generated student numbers** — format `STU{year}{sequence}` e.g. `STU250001`
- **Demo data** — database seeds 5 sample students and grades on first run
- **SQLite** — no database server required, data stored in a local `.db` file

---

## OOP Concepts Demonstrated

| Concept | Where |
|---|---|
| Encapsulation | All model properties with private setters where appropriate |
| Computed properties | `Student.FullName`, `Student.Age`, `Grade.LetterGrade`, `Grade.GradeDescription` |
| Abstraction | `StudentService` hides all data access complexity from the UI |
| Separation of concerns | Strict 3-layer architecture — Presentation, Logic, Data |
| Single Responsibility | Each class has one job — repositories only query, services only validate |

---

## Tech Stack

| Technology | Purpose |
|---|---|
| C# (.NET 8) | Application language |
| Windows Forms | GUI framework |
| SQLite | Local embedded database |
| Microsoft.Data.Sqlite | SQLite driver / NuGet package |
| Visual Studio 2022 | IDE |

---

## Getting Started

### Prerequisites
- Windows 10 or 11
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/community/) (free Community edition)
- .NET 8 SDK — installed automatically with Visual Studio when you select the **"ASP.NET and web development"** workload

### Running the project
```bash
# 1. Clone the repo
git clone https://github.com/Mzingeli10/student-management-system.git

# 2. Open the solution in Visual Studio
#    Double-click StudentMS.sln

# 3. Build the solution
#    Press Ctrl + Shift + B

# 4. Run the application
#    Press F5
```

The SQLite database (`StudentMS.db`) is created automatically on first run in the same folder as the executable. Five demo students are pre-loaded so you can explore all features immediately.

---

## Database Schema

```sql
Students (
    Id              INTEGER PRIMARY KEY,
    FirstName       TEXT,
    LastName        TEXT,
    Email           TEXT UNIQUE,
    Phone           TEXT,
    StudentNumber   TEXT UNIQUE,
    DateOfBirth     TEXT,
    EnrollmentDate  TEXT,
    Course          TEXT,
    YearOfStudy     INTEGER,
    Status          TEXT       -- Active | Suspended | Graduated
)

Grades (
    Id          INTEGER PRIMARY KEY,
    StudentId   INTEGER REFERENCES Students(Id),
    ModuleName  TEXT,
    ModuleCode  TEXT,
    Mark        REAL,
    Year        INTEGER,
    Semester    TEXT
)

Modules (
    Id          INTEGER PRIMARY KEY,
    Code        TEXT UNIQUE,
    Name        TEXT,
    Credits     INTEGER,
    Lecturer    TEXT,
    Year        INTEGER,
    Semester    TEXT
)
```

---

## Grading Scale

| Mark | Letter | Description |
|------|--------|-------------|
| 75 – 100 | D | Distinction |
| 60 – 74 | M | Merit |
| 50 – 59 | P | Pass |
| 40 – 49 | PP | Partial Pass |
| 0 – 39 | F | Fail |

---

## Author

**Mzingeli Ncube**
3rd Year — Bachelor of Computing (Integrated Honours: Software Engineering)
Belgium Campus ITversity · Pretoria, South Africa

[GitHub](https://github.com/Mzingeli10) 
