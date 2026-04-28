# Task 01 - Answer Count Quiz Content

## Goal

Add configurable answer counts to quiz content loading.

## Work

- Add optional `answerCount` support to root `quiz.json`.
- Add optional `answerCount` support to each question `q.json`.
- Apply answer-count precedence:
  - root `quiz.json` defaults to `4` when `answerCount` is omitted
  - question `q.json` overrides the root value when present
- Add effective `AnswerCount` to the in-memory quiz question model.
- Validate that effective answer count is in the supported range `1` through `4`.
- Validate that each question `correctAnswer` is between `1` and the effective answer count.

## Deliverables

- Updated quiz metadata records used by `QuizContentService`.
- Updated `QuizQuestionContent` model.
- Clear quiz loading errors for invalid `answerCount` or `correctAnswer`.

## Acceptance Criteria

- Missing root and question `answerCount` keeps four answers.
- Root `answerCount` applies to questions without an override.
- Question `answerCount` overrides the root value.
- Invalid answer counts fail quiz loading with a clear message.
- `correctAnswer` outside the effective range fails quiz loading with a clear message.

## Notes

- Use JSON property name `answerCount`.
- Keep answer labels numeric strings.

