# Database Relationships

## Current Core Tables

The live SQLite database currently contains the core school/class/student tables. The quiz tables are defined in code and are created or updated by `DatabaseInitializer` when the app starts.

```text
Schools
  1 --- many Classes
          1 --- many Students
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

## Quiz Tables Defined By Code

```text
Classes
  1 --- many QuizSessions
            1 --- many QuizSessionQuestions

Students
  1 --- many QuizAnswers
```

### QuizSessions

- Legacy/compatibility table.
- Belongs to one `Class`.
- Foreign key: `ClassId -> Classes.Id`.
- Delete behavior: deleting a class deletes its quiz sessions.
- Tracks old session fields such as title, status, active question index, start time, and completion time.
- Stage 4 teacher live quiz flow no longer uses this table as the live source.

### QuizSessionQuestions

- Legacy/compatibility table.
- Belongs to one `QuizSession`.
- Foreign key: `QuizSessionId -> QuizSessions.Id`.
- Delete behavior: deleting a quiz session deletes its session questions.
- Unique index: `(QuizSessionId, QuestionIndex)`.
- Stage 4 live quiz flow no longer uses this table as the live answer source.

### QuizAnswers

- Stage 4 live quiz table.
- Belongs to one `Student`.
- Foreign key: `StudentId -> Students.Id`.
- Delete behavior: deleting a student deletes that student's quiz answer rows.
- Does not point to `QuizSessionQuestions`.
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

## Stage 4 Live Quiz Identity

`QuizAnswers` groups live answers by:

```text
QuestionIndex + QuestionKey
```

Each student's answer row is identified by:

```text
QuestionIndex + QuestionKey + StudentId
```

`QuestionKey` and `QuestionIndex` are stored as data fields, not foreign keys.

## Practical Meaning

- School owns classes.
- Class owns students.
- Old quiz session tables may remain for compatibility.
- Stage 4 live answer state is stored in `QuizAnswers`.
- `QuizAnswers` is related only to `Students` by a foreign key.
- Question identity is stored directly in each answer row instead of being normalized through a question table.
