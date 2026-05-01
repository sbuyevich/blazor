# Task 02 - Class-Independent Page Access

## Goal

Allow the new School/Class page to render without a current class context while keeping the rest of the app's class-context behavior unchanged.

## Work

- Add a new `/school-class` page under `MyClass.Web/Components/Pages`.
- Update `MainLayout` so `/school-class` can render when `ClassContextState.Result` is missing or not loaded.
- Keep existing class-context gating for pages that still require a current class.
- Load login state from session storage on the page or via a small page-local gate.
- Show a concise access message for unauthenticated users and non-teachers.
- Add a Teacher nav link to `/school-class` for signed-in teachers.

## Deliverables

- New page route.
- Layout exception for the class-independent route.
- Teacher nav link.

## Acceptance Criteria

- `/school-class` renders without `?c=...`.
- Existing class-context pages still require a loaded class.
- Teacher can access the page from navigation.
- Student and unauthenticated users cannot use the page.
- Opening or using the page does not change session class code, login `ClassCode`, URL `c`, or `ClassContextState`.

## Notes

- Preserve the existing custom auth model.
- Do not introduce ASP.NET Core Identity, cookie auth, or route authorization policies for this stage.
