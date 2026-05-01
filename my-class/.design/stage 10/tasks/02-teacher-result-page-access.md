# Task 02 - Teacher Result Page Access

## Goal

Add the teacher-only Quiz Result page route and connect it to existing navigation and class-context behavior.

## Work

- Add a new `/quiz-result` page under `MyClass.Web/Pages`.
- Use the existing `RoleGate` teacher-only pattern.
- Pass the current `ClassContext` into the result page logic or component.
- Load login state from the existing session-storage login service.
- Keep the existing Teacher nav link to `/quiz-result`.
- Show a concise access message for unauthenticated users and non-teachers.

## Deliverables

- New page route.
- Teacher-only access behavior.
- Page-level wiring to the quiz result service.

## Acceptance Criteria

- Teacher can open `/quiz-result` from navigation.
- Student and unauthenticated users cannot use the page.
- The page uses the current class context.
- Existing class-context pages continue to behave unchanged.
- Opening the page does not change session class code, login `ClassCode`, URL `c`, or current classroom context.

## Notes

- Preserve the existing custom auth model.
- Do not introduce ASP.NET Core Identity, cookie auth, or route authorization policies for this stage.
