# Project Structure

## Repository Root

- `my-class/`: the active MyClass application workspace.
- `plugins/blazor-project/`: repo-local plugin that stores project knowledge in Git.
- `.github/`: repository GitHub configuration.
- `.gitignore`: repository ignore rules.

## Active App

Primary code lives under `my-class/src`:

- `MyClass.Web`: Blazor UI, layout, pages, app startup, configuration, SignalR hub wiring, and static assets.
- `MyClass.Core`: EF Core data model, services, options, auth, quiz, students, and class context.
- `database/my-class.db`: local SQLite database used by the app.
- `assets/quizzes/{quiz name}`: quiz content folder configured by `Quiz:RootFolder`.
- `MyClass.slnx`: solution file used for builds.

Project docs and planning artifacts live under `my-class`:

- `AGENTS.md`: authoritative agent/developer guidance for commands, architecture, and constraints.
- `README.md`: user-facing run and troubleshooting guide.
- `.design/`: staged business notes, implementation plans, task files, database notes, and TODOs.

Published/runnable output lives under `my-class/dist`. Do not treat `dist` as source code unless a task explicitly targets packaging or launch scripts.

## Repo-Local Plugin

This repository-local plugin exists so durable project data can be committed to GitHub instead of staying only in private home directories such as `~/.agents` or `~/.codex/skills`.

- `plugins/blazor-project/skills/blazor-project-context/SKILL.md`: entry point for project context.
- `plugins/blazor-project/skills/blazor-project-context/references/`: compact reference files loaded as needed.
- `plugins/blazor-project/skills/blazor-project-context/agents/openai.yaml`: UI metadata for this skill.

## Guidance

- Put repo-specific skills, references, and reusable project instructions under `plugins/blazor-project/skills/`.
- Avoid storing secrets, tokens, or machine-specific paths in committed skill files.
- Prefer `my-class/AGENTS.md` for detailed coding rules and this skill for concise discovery/navigation context.
- Update this reference when the source layout, run commands, or durable architecture assumptions change.
