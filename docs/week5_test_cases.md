# Week 5 QA Test Cases - Billing / Invoice Module

This document outlines manual test cases for verifying the Invoice & Payment (Billing) module.

## Environment & Pre-requisites
- **Backend Stack:** ASP.NET Core MVC, EF Core, SQLite.
- **Database Reset:** Delete `healthcare_crm.db`, `healthcare_crm.db-shm`, `healthcare_crm.db-wal` before launching, so seed data (users, patients, appointments) is fresh.
- At least one Completed appointment must exist to generate an invoice against.

## User Accounts & Credentials
| Full Name | Email | Password | Role |
|-----------|-------|----------|------|
| System Admin | admin@healthcare.com | AdminPass123! | Admin |
| Receptionist Rachel | receptionist@healthcare.com | RecepPass123! | Receptionist |
| Dr. Sarah Connor | doctor@healthcare.com | DoctorPass123! | Doctor |

---

## Test Cases

### Test Case 1: Invoice Generation
- **Action:** Log in as `Receptionist`, call `POST /api/invoices` (via Swagger or app) with a valid `appointmentId` and `amount`.
- **Expected Outcome:** Invoice is created with `status = "Unpaid"`, linked to the correct patient/appointment, `paidAt = null`.

### Test Case 2: Invoice List & Status Badges
- **Action:** Navigate to `/InvoiceView`.
- **Expected Outcome:** All invoices load with correct color-coded badges — Unpaid (purple), Paid (green), Overdue (red).

### Test Case 3: Mark as Paid Flow
- **Action:** Open an Unpaid invoice's Detail screen and click "Mark as Paid", then confirm in the modal.
- **Expected Outcome:** `PATCH /api/invoices/{id}/pay` is called, status updates to "Paid", `paidAt` timestamp is recorded, success confirmation is shown, and the button becomes disabled/labelled "Already Paid".

### Test Case 4: Disabled State on Paid Invoices
- **Action:** Reload the Detail screen of an already-Paid invoice.
- **Expected Outcome:** "Mark as Paid" button is disabled (greyed out, not clickable) and reads "Already Paid".

### Test Case 5: Filtering by Status
- **Action:** On `/InvoiceView`, select each of "Unpaid", "Paid", "Overdue" from the status filter dropdown; also test `GET /api/invoices?status=Overdue` directly.
- **Expected Outcome:** Only invoices matching the selected status are returned/displayed in each case.

---

## Verification Commands
```bash
dotnet build
dotnet test
```