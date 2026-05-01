# Stage 8 Show Question Answer Plan

## Summary

Stage 8 adds a teacher-controlled `Show Answer` flow. The teacher can reveal the answer image for the current question, see student answer time and correctness in the quiz grid, and students see the answer image plus a friendly result message after the reveal.

## Key Changes

- Add a `Show Answer` button on the Teacher Quiz page for the current question.
- Load and display each question's answer image from `a.jpg`, `a.JPG`, or `a.jpeg`.
- Replace the visible question image with the answer image after the teacher reveals the answer.
- Add answer time and correctness columns to the teacher student-answer grid.
- Notify student pages when the answer is shown so they refresh through the existing SignalR quiz notification flow.
- On the Student Quiz Answer page, show the answer image and a configurable message based on the student's result.
- Add app configuration for the three reveal messages: correct, incorrect, and no answer.

## Student Messages Configuration

- Add `Quiz:RevealMessages` to `appsettings.json`.
- Use these default values:

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

- The Student Quiz Answer page chooses exactly one configured message:
  - `Correct` when the student submitted the correct answer.
  - `Incorrect` when the student submitted a wrong answer.
  - `NoAnswer` when the student submitted nothing before reveal/finish.

## Public Interfaces / Types

- Extend quiz answer/student state with answer reveal information.
- Add answer image loading support to the quiz content service.
- Add enough state to distinguish unrevealed question image vs revealed answer image.
- Extend `QuizOptions` with a `RevealMessages` object containing `Correct`, `Incorrect`, and `NoAnswer`.
- No new quiz JSON fields are required.

## Test Plan

- Teacher can reveal the answer for the current question.
- Teacher image changes from question image to answer image after reveal.
- Student image changes from question image to answer image after reveal.
- Student sees the configured correct, incorrect, or no-answer message after reveal.
- Teacher grid shows answer time and correctness after reveal.
- Missing answer image shows a warning without breaking quiz flow.
- Existing start, finish, next, restart, timer, and SignalR behavior does not regress.
- `dotnet build .\MyClass.slnx` succeeds.

## Assumptions

- `Show Answer` is teacher-controlled and uses existing SignalR notifications.
- Default answer reveal messages should be friendly and lightly funny, not sarcastic or discouraging.
- `a.jpg`, `a.JPG`, and `a.jpeg` should be accepted for answer images.
- Correctness comes from comparing the submitted answer with `CorrectAnswer`.

## Task Breakdown

- [x] [Task 01 - Reveal Configuration and State](tasks/01-reveal-configuration-and-state.md)
- [x] [Task 02 - Answer Image Loading](tasks/02-answer-image-loading.md)
- [x] [Task 03 - Teacher Show Answer Flow](tasks/03-teacher-show-answer-flow.md)
- [x] [Task 04 - Student Reveal Experience](tasks/04-student-reveal-experience.md)
- [x] [Task 05 - Quiz Answer Page Changes](tasks/05-quiz-answer-changed.md)
- [ ] [Task 10 - Verification](tasks/10-verification.md)
