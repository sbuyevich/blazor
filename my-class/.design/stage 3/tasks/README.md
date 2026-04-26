# Stage 3 Task Breakdown

Implement the teacher-led live quiz flow for the existing `my-class` Blazor app.

## Tasks

1. [ ] [Task 01 - Quiz Configuration and Content](01-quiz-configuration-and-content.md)
2. [ ] [Task 02 - Quiz Data and Services](02-quiz-data-and-services.md)
3. [ ] [Task 03 - Teacher Quiz Page](03-teacher-quiz-page.md)
4. [ ] [Task 04 - Student Quiz Answer Page](04-student-quiz-answer-page.md)
5. [ ] [Task 05 - Navigation and Security](05-navigation-and-security.md)
10. [ ] [Task 10 - Verification](10-verification.md)

## Notes

- Stage 3 supports one configured quiz only.
- Questions are discovered from immediate subfolders under `Quiz:RootFolder`.
- Keep teacher and student operations scoped to the current class.
- Use polling for live teacher status updates; do not add SignalR in this stage.
