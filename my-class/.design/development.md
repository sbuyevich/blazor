# Agentic Programming Development Methodology

This project is developed in small, reviewable stages. The human owns product intent and final decisions. The agent helps turn that intent into plans, tasks, code, checks, and documentation.

## High-Level Design

- Collect functional requirements, non-functional requirements, and constraints.
- Choose the technology stack.
- Choose the database approach.
- Choose delivery, launch, and infrastructure assumptions.
- Record important decisions in `.design` so later stages do not rediscover them.

> Use the agent in Plan mode for high-level design and stage planning.

## Stage Planning

- Split design and implementation into stages.
- For each stage:
  - Create a rough `brd.md` manually.
  - Ask the agent whether the BRD is clear.
  - Ask the agent to create `plan.md` from `brd.md`.
  - Review and edit the plan until it is decision-complete.
  - Ask the agent to split the plan into task files under `tasks/`.
  - Review task files before implementation starts.

## Implementation Workflow

- Commit or otherwise save a clean checkpoint before each agent implementation pass.
- Ask Agent to implement one task or one small group of related tasks at a time.
- Build after each implemented task group.
- Test the affected workflow manually when UI behavior changes.
- Mark completed tasks in the stage `plan.md`.

> Manual refactor after tasks/stage implementation

## Documentation Hygiene

- Update `AGENTS.md`, `README.md`, and relevant `.design` files after meaningful refactors or workflow changes.
- Keep current architecture notes accurate so future agent work stays consistent.
- Document accepted conventions, not every rejected idea.
- Keep BRDs short and business-focused; keep plans and task files implementation-focused.