# Database Relationships

## Relationship Shape

The app database is centered on schools, classes, students, and live quiz answers.

```text
Schools
  1 --- many Classes
          1 --- many Students
                  1 --- many QuizAnswers
```

## Tables

### Schools

- Root table for school data.
- Primary key: `Id`.
- Navigation: one school has many `Classes`.

### Classes

- Belongs to one `School`.
- Foreign key: `SchoolId -> Schools.Id`.
- Delete behavior: deleting a school deletes its classes.
- Unique index: `Code`.
- Navigation: one class has many `Students`.

### Students

- Belongs to one `Class`.
- Foreign key: `ClassId -> Classes.Id`.
- Delete behavior: deleting a class deletes its students.
- Unique index: `(ClassId, UserName)`, so usernames are unique inside a class.
- Navigation: one student can have many `QuizAnswers` rows while a quiz is active.

### QuizAnswers

- Active quiz answer table for the current quiz run.
- Belongs to one `Student`.
- Foreign key: `StudentId -> Students.Id`.
- Delete behavior: deleting a student deletes that student's quiz answer rows.
- Current class scope is inferred through `QuizAnswers.StudentId -> Students.ClassId`.
- Does not store `ClassId` directly.
- Does not point to a quiz session or quiz session question table.
- Stores denormalized student snapshot fields:
  - `StudentUserName`
  - `StudentFirstName`
  - `StudentLastName`
  - `StudentDisplayName`
- Stores denormalized question fields:
  - `QuestionKey`
  - `QuestionIndex`
  - `QuestionText`
  - `CorrectAnswer`
- Stores live answer fields:
  - `Answer`
  - `StartedAtUtc`
  - `EndedAtUtc`
  - `IsCorrect`

## Live Quiz Behavior

- `QuizAnswers` contains rows for the current active quiz run.
- For each started question, `QuizAnswers` contains one row for each active student in the current class.
- Starting a quiz clears/replaces old `QuizAnswers` rows and creates first-question rows.
- Starting the next question appends rows for the next question and keeps previous question rows.
- `QuizAnswers` keeps all questions and student answers for the active quiz run.
- `QuizAnswers` is active quiz-run storage, not long-term historical reporting.

## Live Quiz Identity

`QuizAnswers` groups question rows by:

```text
QuestionIndex + QuestionKey
```

Each student's answer row for a question is identified by:

```text
QuestionIndex + QuestionKey + StudentId
```

`QuestionKey` and `QuestionIndex` are stored as data fields, not foreign keys.

## Practical Meaning

- School owns classes.
- Class owns students.
- Student owns live quiz answer rows.
- `QuizAnswers` is connected only to `Students` by a foreign key.
- Current class is resolved through the student relationship.
- Question identity is stored directly in each answer row instead of being normalized through a question table.
