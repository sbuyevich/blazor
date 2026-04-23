# Task 04 - Project Services

## Goal
Implement server-side project CRUD behavior behind services instead of putting data access in components.

## Work
- Create `IProjectService` and `ProjectService`.
- Add methods for:
  - list current user's projects
  - get a single owned project
  - create a project
  - update a project
  - delete a project
- Use `ApplicationDbContext` via `IDbContextFactory`.
- Enforce ownership checks inside the service layer.
- Ensure users cannot edit or delete another user's records.

## Deliverables
- `IProjectService`
- `ProjectService`
- Ownership-safe CRUD methods scoped to the logged-in user

## Acceptance Criteria
- A user only receives their own projects from the list operation.
- Create stores the current user as the owner.
- Edit updates only records owned by the current user.
- Delete removes only records owned by the current user.
- Access to another user's project is rejected server-side.

## Notes
- UI checks are helpful but not sufficient.
- The service layer is the real authorization boundary for project ownership.
