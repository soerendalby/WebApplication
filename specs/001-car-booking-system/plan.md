# Implementation plan — Car booking system (MVP)

This plan outlines a small, incremental path to deliver the MVP described in the feature spec. The repo uses ASP.NET Core Razor Pages on .NET 9.

## Scope (MVP)
- Student can submit a booking request with required declarations
- Approver can approve/reject with comments; can retract approval
- MB Admin can manage study programs and approvers; view all requests
- Audit log and CSV export (per contracts/audit-log-export.csv.schema.md)
- Time-based behaviors: reminder and expiry processing
- Dark theme: white body text, orange headings

Out-of-scope (MVP)
- External integrations (email/SMS/SSO)
- Real identity store (use demo login/claims)
- Payments, fleets, or physical key management

## Assumptions
- Keep Razor Pages (no controllers) to match current repo
- Persistence: start with EF Core + SQLite, alternatively stub with in-memory for POC
- Demo auth: cookie auth + simple domain->role mapping for Student/Approver/Admin

## Architecture sketch
- Pages:
  - Pages/Requests/New.cshtml — student submission form
  - Pages/Approvals/Index.cshtml — approver queue + actions
  - Pages/Admin/Programs/*.cshtml — CRUD + assign approvers
  - Pages/Admin/Requests/Index.cshtml — list/filter/search
  - Pages/Admin/Audit/Index.cshtml — view + CSV export handler
- Services:
  - IRequestService, IApprovalService, IAuditService; thin PageModels
  - IReminderService (background) for reminders/expiry (HostedService)
- Data (if EF): DbContext with sets: Users, StudyPrograms, Requests, Approvals, AuditLogs

## Milestones and deliverables
M0 — Setup and scaffolding
- Add folders (Pages/… per above), service interfaces, models
- Optional: add EF Core + SQLite baseline and connection string
- Demo login page + cookie auth with claims (Student/Approver/Admin)

M1 — Student submission (FR: validation + declarations)
- New request page with model validation and mandatory checkboxes
- Creates Request, initial AuditLog entry
- Success banner and redirect to confirmation

M2 — Approver queue and decisions
- Approvals/Index listing with filters (pending, mine)
- Approve with optional comment; Reject with required comment
- Retract approval; audit all actions

M3 — Admin: Study program + Approver assignment
- CRUD for study programs
- Assign/unassign approvers UI
- Requests admin list with search/filter/pagination

M4 — Audit log + CSV export
- Audit list with filters by action/date/user
- CSV export endpoint adhering to contract (column order/types)

M5 — Time-based processing
- HostedService runs periodic job (e.g., every 5–10 min)
- Send reminder signals (UI indicators) and mark Expired when due
- Exclude Expired from actionable queues

M6 — Theming and polish
- Ensure dark theme coverage for forms, tables, nav, focus states
- Accessibility pass (contrast, focus, ARIA labels on controls)

## Data model mapping (summary)
- Request: Id, StudentId, StudyProgramId, State(Pending/Approved/Rejected/Expired), Declarations, CreatedAt, UpdatedAt
- Approval: Id, RequestId (unique), ApproverId, Decision(Approved/Rejected/Retracted), Comment, Timestamp
- StudyProgram: Id, Name (unique), Settings (reminder_days, expiry_days)
- AuditLog: Id, Timestamp, UserId, Role, Action, RequestId?, StudyProgramId?, Details

## Background processing
- Implement ReminderWorker : BackgroundService
- Uses clock abstraction (ITimeProvider) for testability
- Transaction boundary per batch; backoff on errors; idempotent marking

## Security and roles (MVP)
- Cookie auth + custom claims; simple sign-in UI maps email domain to role:
  - *@student.* => Student, *@admin.* => Approver, *@mb.* => MB Admin (example)
- Anti-forgery on POST handlers; validate resource ownership for Students

## Validation and UX
- Required checkboxes must be ticked before submit
- Clear error messages; keep entered values on error
- Sticky filters in queues via query params

## Risks and mitigations
- Time-based state drift — Use durable timestamps and single-writer job
- CSV encoding issues — Always UTF-8 with BOM; commas in fields quoted
- Role spoofing in demo — Scope to demo only; replace with real IdP later
- Razor Pages app structure drift — Keep services thin and tested, avoid fat PageModels

## Acceptance mapping (high level)
- FR-001..FR-0xx Student submission → M1
- Approver decisions and retract → M2
- Admin program/approver management → M3
- Audit CSV export → M4 (schema-compliant)
- Reminders/Expiry → M5
- Theming dark mode → M6

## Work breakdown (initial)
- Setup: 1–2d
- M1: 1–2d
- M2: 2–3d
- M3: 2–3d
- M4: 1d
- M5: 2d
- M6: 0.5–1d

## Definition of done
- Build succeeds, manual scenarios in quickstart pass
- CSV export matches contract; theme verified per spec
- Minimal logging in sensitive flows; no PII in logs
