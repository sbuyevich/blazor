# Task 02 - Quiz Data and Services

## Goal

Add the data and service layer needed to run a teacher-led quiz session and validate student answer submissions.

## Work

- Add quiz session persistence for the current class.
- Track the current session question, status, start time, finish time, timeout, and correct answer.
- When the teacher starts a question, create in-progress answer records for each active student in the current class.
- Add backend validation so a student answer is accepted only when an in-progress answer record exists for that student and current question.
- Save a valid selected answer and prevent duplicate answers for the same student/question.
- When a question finishes or times out, mark active students without answers as failed/no-answer.
- Add service methods for teacher actions: load status, start, finish, and next.
- Add service methods for student actions: get answer-page state and submit selected answer.
- Enforce teacher/student role and current-class scope inside services.

## Deliverables

- Quiz EF entities/tables
- Quiz DbContext configuration
- Quiz session service
- Quiz answer service
- Result/view models for teacher status and student answer state

## Acceptance Criteria

- Teacher can create and advance a quiz session for the current class.
- Starting a question creates in-progress answer records only for active students in the current class.
- Student submit is rejected when no matching in-progress answer record exists.
- Student submit is rejected after the same student already answered the current question.
- Timeout and Finish close the current question and record failures for non-answering active students.
- Students from another class cannot submit answers for the current class quiz.
- Student and teacher permissions are validated server-side.

## Notes

- Keep data access in services, not Razor components.
- Use the existing `IDbContextFactory<ApplicationDbContext>` pattern.
- Do not add controllers, Minimal APIs, or external AJAX endpoints.
