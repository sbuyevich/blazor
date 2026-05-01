# Task 01 - Reveal Configuration and State

## Goal

Add the configuration and service state needed to support teacher-controlled answer reveal.

## Work

- Extend `QuizOptions` with `RevealMessages`.
- Add `Correct`, `Incorrect`, and `NoAnswer` message values.
- Add default values to `appsettings.json`:

```json
{
  "Quiz": {
    "RevealMessages": {
      "Correct": "Correct. Good job!",
      "Incorrect": "Incorrect. Not this time.",
      "NoAnswer": "No answer submitted. Stay focused!"
    }
  }
}
```

- Extend quiz state models so teacher and student pages know whether the current answer is revealed.
- Persist or derive reveal state consistently enough that refreshes keep showing the revealed answer for the current question.
- Ensure correctness can be computed from submitted answer and `CorrectAnswer`.

## Deliverables

- Updated quiz options model.
- Updated app configuration.
- Updated teacher/student quiz state models.
- Service support for answer reveal state.

## Acceptance Criteria

- Reveal messages are read from configuration.
- Missing config values fall back to safe default messages.
- Current question state can distinguish question image vs answer image.
- Refreshing teacher or student pages preserves revealed state for the current question.

## Notes

- Do not add new quiz JSON fields.
- Prefer the current `QuizAnswers`-backed quiz state model.
