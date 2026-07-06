# Week 4 QA Test Cases - Appointment Management Module

This document outlines the automated and manual test cases for verifying the Appointment Management Module.

## Environment & Pre-requisites
- **Backend Stack:** ASP.NET Core MVC, EF Core, SQLite.
- **Database Reset:** Reset the SQLite database by deleting `healthcare_crm.db`, `healthcare_crm.db-shm`, and `healthcare_crm.db-wal` in the project root before launching the server. This guarantees that all required schemas are re-initialized and seed users (Admin, Doctors, Receptionist), patients, and test appointments are fully populated.

---

## User Accounts & Credentials

The seed data initializes the following users:

| Full Name | Username | Email | Password | Role |
|-----------|----------|-------|----------|------|
| System Admin | admin@healthcare.com | admin@healthcare.com | AdminPass123! | Admin |
| Dr. Sarah Connor | doctor@healthcare.com | doctor@healthcare.com | DoctorPass123! | Doctor |
| Dr. John Watson | doctor2@healthcare.com | doctor2@healthcare.com | Doctor2Pass! | Doctor |
| Receptionist Rachel | receptionist@healthcare.com | receptionist@healthcare.com | RecepPass123! | Receptionist |

---

## Test Cases

### Test Case 1: Unauthorized Access Prevention
- **Action:** Open a private browser window and attempt to access `/AppointmentView`.
- **Expected Outcome:** 
  - Server intercepts request, checks for authentication cookie, finds none, and redirects to `/Account/Login`.

### Test Case 2: Role Restrictions (Doctor Access Restrictions)
- **Action:** Log in as `Doctor` (`doctor@healthcare.com` / `DoctorPass123!`) and navigate to `/AppointmentView`.
- **Expected Outcome:**
  - Access is granted. The Appointment Directory loads.
  - The "Schedule Appointment" button is **hidden** from the UI.
  - Only appointments assigned to `Dr. Sarah Connor` are listed (2 appointments seeded). Appointments assigned to other doctors are hidden.
  - Under "Actions", only the "Details" (eye) icon is visible. The "Edit" and "Cancel" buttons are hidden.
- **Action:** Attempt to force navigate to `/AppointmentView/Create`.
- **Expected Outcome:** Redirected to `/Account/UnauthorizedPage` (Access Denied).

### Test Case 3: Create Appointment (Admin/Receptionist Only)
- **Action:** Log in as `Receptionist` (`receptionist@healthcare.com` / `RecepPass123!`) and navigate to `/AppointmentView`.
- **Expected Outcome:**
  - Access granted. The "Schedule Appointment" button is **visible**.
- **Action:** Click "Schedule Appointment". Fill out the form:
  - Patient: Jane Doe
  - Doctor: Dr. John Watson
  - Date: (Choose a future date)
  - Time Slot: 10:30 AM
  - Status: Scheduled
  - Reason: Routine cardiovascular exam follow-up.
- **Action:** Submit the form.
- **Expected Outcome:**
  - Successful validation on client & server.
  - Redirects back to `/AppointmentView` showing a success banner.
  - The new appointment is displayed in the list.

### Test Case 4: Form Fields Validation
- **Action:** On the "Schedule Appointment" form, leave "Reason" blank and submit.
- **Expected Outcome:**
  - Client-side validation triggers, highlighting the field in red with a validation message. Submission is blocked.
- **Action:** On the "Schedule Appointment" form, enter a reason exceeding 500 characters and submit.
- **Expected Outcome:**
  - Client-side validation triggers, highlighting the field in red. Submission is blocked.

### Test Case 5: Edit Appointment (Admin/Receptionist Only)
- **Action:** Click the "Edit" pencil icon on any appointment.
- **Expected Outcome:**
  - Form pre-populates with patient, doctor, date, time slot, status, and reason details.
- **Action:** Change the Time Slot to "02:00 PM", change the Status to "Completed", and submit.
- **Expected Outcome:**
  - Changes are saved. Redirects to directory with success message. Status badge changes to green "COMPLETED".

### Test Case 6: Cancel/Delete Appointment (Admin/Receptionist Only)
- **Action:** Click the "Delete" trash icon on any appointment.
- **Expected Outcome:**
  - A glassmorphic confirmation modal overlays the screen, asking if you are sure you want to cancel.
- **Action:** Click "Cancel Appointment".
- **Expected Outcome:**
  - Request is processed via `DELETE /api/appointments/{id}`.
  - The appointment is deleted, modal closes, and directory list updates immediately.

### Test Case 7: Search and Filter Panel
- **Action:** Type "Smith" in the search input box.
- **Expected Outcome:** List dynamically filters (300ms debounce) to display only appointments related to "John Smith".
- **Action:** Filter by Status = "Completed".
- **Expected Outcome:** Lists only completed appointments.
- **Action:** Choose a date with no appointments.
- **Expected Outcome:** Renders the "No appointments found." empty state view.

---

## Verification Commands
```bash
# Verify the code builds with zero compilation errors
dotnet build

# Verify all test cases pass
dotnet test
```
