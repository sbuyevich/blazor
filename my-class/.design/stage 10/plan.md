# Update Stage 10 Quiz Result BRD

## Summary

Revise `my-class/.design/stage 10/brd.md` from a terse todo-style note into a clear BRD for the teacher-only Quiz Result page. The BRD should reflect verified repo reality: `QuizAnswers` stores the current active quiz run only, `/quiz-result` is already in teacher navigation, but no Quiz Result page exists yet.

## Key Changes

- Define the feature goal: teacher views quiz results for the current active quiz run.
- Clarify the two required grids:
  - Student summary grid grouped by student.
  - Question summary grid grouped by question.
- Define result semantics:
  - Correct count: `IsCorrect = true`.
  - Incorrect count: `IsCorrect = false`, including empty/no-answer rows.
  - Percent correct: correct / total rows in that group.
  - Total answer time: sum elapsed time for submitted answers, and for no-answer/timeouts use the full question time limit.
- Define CSV export requirements from current `QuizAnswers` data:
  - `StudentDisplayName`
  - `QuestionText`
  - `CorrectAnswer`
  - `Answer`
  - `AnswerTime`
  - `IsCorrect`
- Add empty/error states:
  - No active quiz results yet.
  - Teacher-only access.
  - Export disabled when no rows exist.

## Interfaces / Types

- No code changes are part of this BRD update.
- The BRD should state future implementation will likely need a quiz-result service/model for grouped student summaries, grouped question summaries, and flat CSV rows.
- No persistent historical report storage should be required for Stage 10.

## Test Plan

- Verify the BRD aligns with current `QuizAnswers` fields and active-run behavior.
- Confirm it does not imply creating `QuizSessions`, historical tables, or long-term reporting.
- Confirm acceptance criteria cover sorting, percentages, no-answer handling, teacher-only access, empty state, and CSV output.

## Assumptions

- Stage 10 reports only the current active quiz run.
- No-answer/timeouts count as incorrect and contribute full question time to aggregate time.
- The BRD update is limited to `brd.md`; `stage 10/plan.md` can be corrected separately if requested.
