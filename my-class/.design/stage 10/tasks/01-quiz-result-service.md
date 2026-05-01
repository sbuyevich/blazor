# Task 01 - Quiz Result Service

## Goal

Add the service layer needed for the Quiz Result page to read current active quiz results and produce student, question, and export-ready result data.

## Work

- Add `IQuizResultService` under `MyClass.Core\Services\Quiz\Contracts`.
- Add `QuizResultService` under `MyClass.Core\Services\Quiz`.
- Register the service in `Program.cs`.
- Add lightweight result models for:
  - quiz result page state
  - student summary rows
  - question summary rows
  - CSV export rows or export content
- Implement teacher authorization from `LoginState`, `ClassContext`, and `TeacherOptions`.
- Query `QuizAnswers` through `IDbContextFactory<ApplicationDbContext>`.
- Scope rows to the current class through the `Student` relationship.
- Calculate student summaries grouped by student.
- Calculate question summaries grouped by `QuestionIndex` and `QuestionKey`.
- Calculate answer elapsed time from `StartedAtUtc` to `EndedAtUtc` for submitted answers.
- Treat no-answer/timeouts as incorrect and use the full question time for aggregate time.

## Deliverables

- New service contract and implementation.
- New result/view model records.
- DI registration in the web project.

## Acceptance Criteria

- Service methods require teacher access for the current class.
- Non-teacher and unauthenticated calls return failure results.
- Result queries return only current class rows.
- Correct count uses `IsCorrect = true`.
- Incorrect count includes wrong answers and empty/no-answer rows.
- Percent correct is based on all rows in the group.
- Student and question aggregate time includes full question time for no-answer/timeouts.
- Empty result sets return a successful empty state instead of an error.

## Notes

- Do not add migrations or database schema changes.
- Do not introduce quiz session tables.
- Use `IDbContextFactory<ApplicationDbContext>`.
