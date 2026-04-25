# Task 01 - Login Modal Gate

## Goal

Replace navigation-based login with a blocking login modal shown when no user is logged in.

## Work

- Remove the Login item from the navigation menu.
- Add an app-level authentication gate that checks the existing `my-class.loginState` local storage key.
- Show the existing login UI as a MudBlazor modal/dialog when no login state exists.
- Prevent unauthenticated users from interacting with the normal app shell behind the modal.
- Keep the modal scoped to the current class context so login still uses the active class code.
- Preserve student registration access from the modal.

## Deliverables

- No Login item in the navigation menu
- Blocking unauthenticated login modal
- Login modal uses existing teacher/student login flow
- Register link remains available from the login modal

## Acceptance Criteria

- Opening the app without login state shows a login modal.
- The normal app remains unavailable until login succeeds.
- The navigation menu does not contain a Login item.
- Login continues to validate teacher credentials from appsettings.
- Login continues to validate students against the current class.
- Missing or invalid class context still shows a clear class-context error.

## Notes

- Continue using `localStorage`; do not introduce ASP.NET Core Identity.
- Prefer reusing existing login form logic instead of duplicating validation rules.
