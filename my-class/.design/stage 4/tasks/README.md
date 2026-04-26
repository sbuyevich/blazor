# Stage 4 Task Breakdown

Implement quiz file model updates and denormalized live answer saving for the existing `my-class` Blazor app.

## Tasks

1. [ ] [Task 01 - Quiz File Model and Loading](01-quiz-file-model-and-loading.md)
2. [ ] [Task 02 - Denormalized QuizAnswers Table](02-denormalized-quizanswers-table.md)
3. [ ] [Task 03 - Teacher Quiz Flow Updates](03-teacher-quiz-flow-updates.md)
4. [ ] [Task 04 - Student Answer Flow Updates](04-student-answer-flow-updates.md)
10. [ ] [Task 10 - Verification](10-verification.md)

## Notes

- Stage 4 supports one live quiz session at a time.
- Root `quiz.json` uses `title` and `TimeLimitSeconds`.
- Question subfolders contain `q.jpg` and `a.json`.
- `correctAnswer` and student `Answer` are strings.
- Empty student `Answer` is treated as incorrect.
