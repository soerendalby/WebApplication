# Tasks — Car booking system (MVP)

Feature: Car booking system (MVP)
Feature directory: C:/github/WebApplication/specs/001-car-booking-system

Context for task generation: (no extra arguments provided)

This checklist follows the required format and is organized by phases and user stories. Each task includes a file path and is independently actionable by an LLM.

---

## Phase 1 — Setup

- [X] T001 Add EF Core + SQLite packages to project file WebApplication.csproj (`WebApplication.csproj`)
- [X] T002 Add DefaultConnection to appsettings.json with SQLite file path (`appsettings.json`)
- [X] T003 Create AppDbContext with DbSets per data-model (`Data/AppDbContext.cs`)
- [X] T004 Register AppDbContext and SQLite in Program.cs (`Program.cs`)
- [X] T005 Add cookie authentication and simple authorization policies in Program.cs (`Program.cs`)
- [X] T006 Create Models: User, StudyProgram, Approver, Request, Approval, AuditLog (`Models/*.cs`)
- [X] T007 Create Services contracts: IRequestService, IApprovalService, IAuditService, IProgramService (`Services/Contracts/*.cs`)
- [X] T008 Create basic service implementations (no business logic yet) (`Services/*.cs`)

## Phase 2 — Foundational

- [X] T009 Create demo Login page (email + password) and sign-out link (`Pages/Auth/Login.cshtml`)
- [X] T010 Implement Login PageModel: map email domain → role claims; issue auth cookie (`Pages/Auth/Login.cshtml.cs`)
- [X] T011 Protect areas with Authorize: Students→/Requests, Approvers→/Approvals, Admins→/Admin (`Pages/Requests/_ViewImports.cshtml`, `Pages/Approvals/_ViewImports.cshtml`, `Pages/Admin/_ViewImports.cshtml`)
- [X] T012 Add shared partial for alerts/validation summaries (`Pages/Shared/_Alerts.cshtml`)
- [X] T013 Ensure dark theme styles apply to forms/tables/links per spec (`wwwroot/css/site.css`)
- [X] T014 Add CsvWriter helper (UTF-8 BOM, LF per contract, quoting) (`Services/Util/CsvWriter.cs`)
- [X] T015 Create ReminderWorker skeleton (BackgroundService) with timer (`Services/Hosted/ReminderWorker.cs`)

---

## Phase 3 — User Story 1 (P1): Student submits a booking request
Goal: Student can create a request with mandatory declarations and see correct status.
Independent test criteria: From quickstart "Student — submit request (P1)" steps succeed; invalid (unchecked) submissions are blocked.

- [ ] T016 [US1] Create Student Requests folder and New page UI (`Pages/Requests/New.cshtml`)
- [ ] T017 [US1] Implement New PageModel with DataAnnotations and ModelState validation (`Pages/Requests/New.cshtml.cs`)
- [ ] T018 [US1] Persist Request on submit and emit audit.request.create (`Services/RequestService.cs`)
- [ ] T019 [US1] Auto-approve if study program has 0 approvers; else Pending (`Services/RequestService.cs`)
- [ ] T020 [US1] Create Student Requests overview with statuses + pagination (`Pages/Requests/Index.cshtml`)
- [ ] T021 [US1] Implement overview PageModel with query filters (`Pages/Requests/Index.cshtml.cs`)
- [ ] T022 [US1] Link navigation to student requests (`Pages/Shared/_Layout.cshtml`)

Parallel examples (US1)
- [ ] T023 [P] [US1] Create Request entity mapping and indices (`Models/Request.cs`)
- [ ] T024 [P] [US1] Add Request DbSet and migration (`Data/AppDbContext.cs`)

---

## Phase 4 — User Story 2 (P2): Approver processes requests
Goal: Approver can approve/reject with comments and retract previous approval; actions logged.
Independent test criteria: From quickstart "Approver — decide (P2)" steps succeed; audit entries created.

- [ ] T025 [US2] Create Approvals queue page with filters/sorting (`Pages/Approvals/Index.cshtml`)
- [ ] T026 [US2] Implement Approvals PageModel: load pending for assigned programs (`Pages/Approvals/Index.cshtml.cs`)
- [ ] T027 [US2] Implement Approve handler (optional comment) + audit.approval.approve (`Pages/Approvals/Index.cshtml.cs`)
- [ ] T028 [US2] Implement Reject handler (mandatory comment) + audit.approval.reject (`Pages/Approvals/Index.cshtml.cs`)
- [ ] T029 [US2] Enforce single decision per request (UNIQUE constraint) (`Models/Approval.cs`)
- [ ] T030 [US2] Implement retract approval action + audit.approval.retract (`Pages/Approvals/Index.cshtml.cs`)
- [ ] T031 [US2] Show decision comments to student in overview (`Pages/Requests/Index.cshtml`)

Parallel examples (US2)
- [ ] T032 [P] [US2] Create Approval entity and mapping (`Models/Approval.cs`)
- [ ] T033 [P] [US2] Implement ApprovalService with decision methods (`Services/ApprovalService.cs`)

---

## Phase 5 — User Story 3 (P3): MB admin manages programs and oversight
Goal: Admin can CRUD study programs, assign approvers, view all requests, audit log + CSV export.
Independent test criteria: From quickstart "MB Admin — config & audit (P3)" steps succeed; CSV matches contract.

- [ ] T034 [US3] Create Admin area: Programs list/create/edit/delete pages (`Pages/Admin/Programs/Index.cshtml`)
- [ ] T035 [US3] Implement Programs PageModels and validations (`Pages/Admin/Programs/Index.cshtml.cs`)
- [ ] T036 [US3] Create Approver assignment UI per program (`Pages/Admin/Programs/Assign.cshtml`)
- [ ] T037 [US3] Implement Approver assignment handlers + audit events (`Pages/Admin/Programs/Assign.cshtml.cs`)
- [ ] T038 [US3] Create Admin Requests overview with filters/search/paging (`Pages/Admin/Requests/Index.cshtml`)
- [ ] T039 [US3] Implement Admin Requests PageModel with query options (`Pages/Admin/Requests/Index.cshtml.cs`)
- [ ] T040 [US3] Create Audit log page with filters (`Pages/Admin/Audit/Index.cshtml`)
- [ ] T041 [US3] Implement Audit PageModel + query and paging (`Pages/Admin/Audit/Index.cshtml.cs`)
- [ ] T042 [US3] Implement CSV export endpoint following contract (`Pages/Admin/Audit/Export.cshtml.cs`)

Parallel examples (US3)
- [ ] T043 [P] [US3] Create StudyProgram and Approver entities/mapping (`Models/StudyProgram.cs`)
- [ ] T044 [P] [US3] Implement ProgramService (CRUD + assignments) (`Services/ProgramService.cs`)
- [ ] T045 [P] [US3] Implement AuditService (append/query/filter) (`Services/AuditService.cs`)

---

## Final Phase — Polish & Cross-cutting Concerns

- [ ] T046 Add ReminderWorker logic: reminder indicator and expiry marking (`Services/Hosted/ReminderWorker.cs`)
- [ ] T047 Surface reminder badges in Approvals UI (`Pages/Approvals/Index.cshtml`)
- [ ] T048 Exclude Expired from actionable queues; show Expired in student/admin views (`Pages/Approvals/Index.cshtml`)
- [ ] T049 Ensure accessible focus styles and contrast in dark theme (`wwwroot/css/site.css`)
- [ ] T050 Verify CSV encoding (UTF-8 BOM) and column order (`Pages/Admin/Audit/Export.cshtml.cs`)
- [ ] T051 Add basic error handling/logging (no PII) in services (`Services/*Service.cs`)

---

## Dependencies (story completion order)
1) US1 (Student submit) → 2) US2 (Approver decisions) → 3) US3 (Admin config/oversight)
- Foundational phases (1–2) must complete before any user story work
- Polish & Cross-cutting after stories

## Parallel execution examples
- US1: T023, T024 can run in parallel with UI tasks T016–T022
- US2: T032, T033 can run in parallel with queue UI T025–T031
- US3: T043–T045 can run in parallel with audit/export UI T040–T042

## Implementation strategy
- MVP-first: Deliver US1 end-to-end before starting US2
- Keep PageModels thin; put logic in services with DI for future testability
- Prefer EF Core + SQLite; if blocked, temporarily stub services with in-memory collections to unblock UI

---

## Format validation
- All tasks follow: `- [ ] T### [P?] [US?] Description (path)`
- Story labels only appear on user story phases (US1, US2, US3)
- Each task includes an explicit file path

