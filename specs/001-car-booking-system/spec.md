# Feature Specification: Car booking system

**Feature Branch**: `001-car-booking-system`  
**Created**: 2025-11-02  
**Status**: Draft  
**Input**: Public requirements specification — Car booking system (Spec Kit / GitHub + Copilot)

## Clarifications

### Session 2025-11-02

- Q: What UI theme should the application use? → A: Dark theme; body text white; headings orange.

## User Scenarios & Validation *(mandatory)*

<!--
  IMPORTANT: User stories should be PRIORITIZED as user journeys ordered by importance.
  Each user story/journey must be INDEPENDENTLY VALIDATABLE - meaning if you implement just ONE of them,
  you should still have a viable MVP (Minimum Viable Product) that delivers value.
  
  Assign priorities (P1, P2, P3, etc.) to each story, where P1 is the most critical.
  Think of each story as a standalone slice of functionality that can be:
  - Developed independently
  - Tested independently
  - Deployed independently
  - Demonstrated to users independently
-->

### User Story 1 - Student submits a car booking request (Priority: P1)

A student logs in using a student.dk email, fills the booking form, checks the two mandatory confirmations (valid license and guidelines read), and submits. If their study program has no approvers, the request is auto-approved on submission; otherwise it becomes Pending approval.

**Why this priority**: Core value delivery for students; enables the primary task of booking a car.

**Independent Validation**: A student user can create a request and immediately see the correct status based on study program configuration.

**Acceptance Scenarios**:

1. Given a student with a student.dk email assigned to a study program with at least one approver, when they complete the form and check both mandatory boxes, then submitting creates a request visible in their overview with status Pending approval and appears in the approver queue.
2. Given a student whose study program has zero approvers, when they submit a valid form, then the request is created and shown as Approved in their overview.
3. Given a student who leaves one or both checkboxes unchecked, when attempting to submit, then the submission is blocked and an inline validation message indicates both confirmations are required.

---

### User Story 2 - Approver processes requests (Priority: P2)

An approver (admin.dk) sees pending requests for their assigned study programs, can approve or reject with an optional short comment on approval and a mandatory comment on rejection, and may retract a previous approval if needed.

**Why this priority**: Ensures governance and safety by enforcing approvals where configured.

**Independent Validation**: An approver can take a decision that updates both their queue and the student's status; actions are logged.

**Acceptance Scenarios**:

1. Given a pending request in an approver’s queue, when the approver approves with an optional comment, then the request leaves the queue, the student sees status Approved and the comment (if any), and the action is recorded in the audit log.
2. Given a pending request in an approver’s queue, when the approver rejects and provides a mandatory comment, then the student sees status Rejected with the comment, and the audit log stores action and comment.
3. Given a previously approved request, when an approver retracts their approval, then MB administrators see an indicator of the retraction in their overview/audit log. The first valid decision (approve or reject) concludes the process; subsequent decisions are ignored except explicit retraction events.

---

### User Story 3 - MB administrator manages configuration and oversight (Priority: P3)

MB administrators (mb.dk) manage study programs and approver assignments, monitor all requests, and review/export the audit log. Overviews provide badge counts, search, sorting, and pagination.

**Why this priority**: Enables sustainable operations, compliance, and configuration without developer intervention.

**Independent Validation**: An MB admin can CRUD study programs/approvers, review indicators (retractions, expiries), and export the audit log to CSV.

**Acceptance Scenarios**:

1. Given MB admin access, when creating, updating, or deleting a study program and assigning approvers, then changes persist and are reflected in approver queues and student flows.
2. Given MB admin access, when viewing the audit log and applying filters, then relevant events display and an export action downloads a CSV with the current filtered result set.

---

[Add more user stories as needed, each with an assigned priority]

### Edge Cases

- Study program with 0 approvers: request auto-approves on submission; never enters approver queues.
- Multiple approvers: the first valid decision (approve or reject) concludes the process; later attempts are blocked or no-ops with explanation.
- Approval retraction: logged and surfaced to MB admins as an indicator; request remains in its last concluded state unless explicitly overwritten by policy (not required for MVP).
- Expiry: if no decision before the configured expiry, request is marked Expired; student sees Expired, approvers see it removed from actionable queue; audit event recorded.
- Reminders: unprocessed requests are highlighted in approver queue at reminder threshold; do not send emails (UI indicators only).
- Invalid email domain at login: role mapping fails and access is denied with guidance.
- Form validation: submit is disabled until both mandatory checkboxes are checked; server rejects malformed input.
- Search/pagination: empty results render a friendly empty-state; large result sets paginate with default 25 items/page.

## Requirements *(mandatory)*
### Constitution Alignment *(checklist)*

- Testable-by-Design: The proposed design exposes seams for testing later (DI, small pure units) even if tests are not requested now.
- Modern UI/UX: Uses Bootstrap-based, responsive, accessible UI patterns with unobtrusive scripts.
- Incremental Change: Scope is small and localized; avoids unnecessary refactors.
- ASP.NET Conventions: Uses tag helpers (`asp-controller`, `asp-action`, `asp-append-version`), assets in `wwwroot/`.
- Data Default: If persistence is required, EF Core + SQLite is preferred unless justified.
- Independent Validation: Manual steps included to validate each story on its own.


<!--
  ACTION REQUIRED: The content in this section represents placeholders.
  Fill them out with the right functional requirements.
-->

### Functional Requirements

- Auth & RBAC
  - **FR-001**: Users MUST authenticate with email + password.
  - **FR-002**: The system MUST map roles by email domain: student.dk → Student; admin.dk → Approver; mb.dk → MB Administrator. Other domains are denied.
  - **FR-003**: Role-based access control MUST restrict admin capabilities to MB administrators and approval actions to assigned approvers.

- Study programs and approvers
  - **FR-010**: MB admins MUST be able to create, update, and delete study programs.
  - **FR-011**: MB admins MUST be able to assign 0..N approvers to each study program and toggle approver active status.
  - **FR-012**: Study programs MAY override reminder_days (default 8) and expiry_days (default 10).

- Request creation and states
  - **FR-020**: Students MUST be able to submit a car booking request only when both mandatory checkboxes are checked (valid license, guidelines read).
  - **FR-021**: On submission, if study program has ≥1 approver, the request MUST be set to Pending approval; otherwise it MUST be set to Approved.
  - **FR-022**: Students MUST see an overview of their requests with statuses: Pending, Approved, Rejected, Expired.
  - **FR-023**: Students MUST NOT be able to retract or cancel requests after submission.

- Approval workflow
  - **FR-030**: Approvers MUST see a queue of pending requests for their assigned study programs.
  - **FR-031**: Approvers MUST be able to Approve (optional short comment) or Reject (mandatory short comment).
  - **FR-032**: The first valid decision (approve or reject) MUST conclude the request; subsequent decisions MUST be prevented.
  - **FR-033**: Approvers MUST be able to retract a previous approval; retraction MUST be logged and surfaced to MB admins as an indicator.

- Time-based events and indicators
  - **FR-040**: The system MUST flag unprocessed requests as “Reminder” in approver queues after reminder_days.
  - **FR-041**: The system MUST automatically mark requests as Expired after expiry_days if still pending, and remove them from actionable queues.
  - **FR-042**: Time-based indicators MUST be configurable per study program (overriding defaults).

- Overviews and usability
  - **FR-050**: All overviews (student, approver, MB admin) MUST support search, sorting, and pagination (default page size 25).
  - **FR-051**: Overviews MUST display badge counts/indicators for unseen or actionable items.
  - **FR-052**: When a decision includes a comment, the comment MUST be visible to both student and approver in relevant overviews.
  - **FR-053**: The UI MUST use a dark theme with white body text and orange headings, maintaining accessible contrast across components.

- Audit and export
  - **FR-060**: The system MUST record audit events for: request creation/approval/rejection/retraction; study program CRUD; approver assignment changes; configuration changes; and decision comments.
  - **FR-061**: MB admins MUST be able to filter the audit log and export the current view as CSV.
  - **FR-062**: Audit data MUST be retained for 5 years.

- Security, privacy, and localization
  - **FR-070**: All access MUST be over HTTPS; personal data MUST be stored securely and shown only to authorized roles.
  - **FR-071**: The UI MUST support Danish and English with user-selectable language for visible labels in the MVP.
  - **FR-072**: For MVP, self-registration without email verification is allowed; domain check occurs at login. A common demo password value "demo" MAY be allowed in demo mode only.

### Key Entities *(include if feature involves data)*

- **User**: Represents an actor in the system; attributes include id, email, name, role (student|approver|admin), study_program_id?, verified, locale?, created_at.
- **StudyProgram**: Academic program; attributes include id, name, description, reminder_days?, expiry_days?.
- **Approver**: Assignment of a user as approver for a study program; attributes include id, user_id, study_program_id, active, extra_scope?.
- **Request**: A car booking request by a student; attributes include id, user_id, study_program_id, status (Pending|Approved|Rejected|Expired), submitted_at, expires_at.
- **Approval**: An approver decision; attributes include id, request_id, approver_id, decision (approved|rejected), comment?, decided_at.
- **AuditLog**: Records key actions with timestamp, user, action, and metadata (including comments on rejection).

## Success Criteria *(mandatory)*

<!--
  ACTION REQUIRED: Define measurable success criteria.
  These must be technology-agnostic and measurable.
-->

### Measurable Outcomes

- **SC-001**: 100% of requests submitted with both confirmations checked; 0% submissions accepted with missing confirmations.
- **SC-002**: For study programs with approvers, ≥90% of requests receive a decision before expiry; 100% of pending requests are marked Expired at expiry_days.
- **SC-003**: 100% of rejections include a non-empty comment; ≥95% of approval/rejection actions update student and approver overviews within 1 refresh cycle.
- **SC-004**: All overviews support search, sorting, and pagination; users can locate a specific request by filter within 10 seconds using built-in UI controls.
- **SC-005**: Audit log covers all listed actions; MB admin can export a CSV reflecting the current filtered view with ≥95% of expected rows and columns.
- **SC-006**: Role mapping by email domain is correct for 100% of logins, and unauthorized domains are denied.
- **SC-007**: Global theme applies: body text renders white on dark backgrounds and all headings render in orange across pages.
