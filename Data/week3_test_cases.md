# Week 3 QA Test Cases - Patient CRM Module

This document outlines the automated and manual test cases for verifying the Patient CRUD, Search, and Medical History functionalities.

## Environment & Pre-requisites
- **Backend Stack:** ASP.NET Core MVC (net10.0), EF Core, SQLite.
- **Database Reset:** Ensure database is reset by deleting the local `healthcare_crm.db`, `healthcare_crm.db-shm`, and `healthcare_crm.db-wal` files in the project root before starting the server. This ensures the database is seeded with initial roles and credentials.

---

## Default User Accounts & Credentials

The database seeding initializes three users with distinct roles:

| Full Name | Username | Email | Password | Role |
|-----------|----------|-------|----------|------|
| System Admin | admin | admin@healthcare.com | AdminPass123! | Admin |
| Dr. Sarah Connor | doctor | doctor@healthcare.com | DoctorPass123! | Doctor |
| Receptionist Rachel | receptionist | receptionist@healthcare.com | RecepPass123! | Receptionist |

---

## Test Cases

### Test Case 1: Database Initialization and Seeding
- **Action:** Delete SQLite db files, run the application (`dotnet run`), and check the created database.
- **Expected Outcome:** 
  - Database schema recreated successfully.
  - Test accounts (Admin, Doctor, Receptionist) are seeded.
  - Three initial patient records (John Doe, Jane Smith, Alice Johnson) and their respective medical histories are seeded.

### Test Case 2: User Access & Protected Routing
- **Scenario A: Guest User (Unauthenticated)**
  - **Action:** Attempt to access `/PatientView` directly in the browser without logging in.
  - **Expected Outcome:** Redirected to `/Account/Login`.
- **Scenario B: Authenticated User (Admin, Doctor, Receptionist)**
  - **Action:** Log in as `Doctor` and navigate to `/PatientView`.
  - **Expected Outcome:** Access granted. Patient Directory list rendered correctly.
- **Scenario C: Authenticated User (Unauthorized Route)**
  - **Action:** Log in as `Doctor` and attempt to access `/Home/AdminDashboard`.
  - **Expected Outcome:** Redirected to `/Account/UnauthorizedPage` (Access Denied).

### Test Case 3: Patient Directory & Search Filtering
- **Action:** Type "John" in the search input box.
- **Expected Outcome:** 
  - Loading spinner is displayed briefly.
  - The table filter applies instantly (debounced by 300ms) showing only "John Doe".
- **Action:** Type a phone number substring, e.g., "555-5678".
- **Expected Outcome:** Displays only matching patient profile ("Jane Smith").
- **Action:** Type a query that matches nothing, e.g., "XYZ999".
- **Expected Outcome:** Renders the "No Patient Records Found" empty state view.

### Test Case 4: Patient CRUD Operations
- **Scenario A: Register Patient (Create)**
  - **Action:** Click "Add Patient", fill in all details (Full Name, Email, Phone, DOB, Gender, Blood Type, Status, Address) and submit.
  - **Expected Outcome:** Success message displayed, page redirects to Patient Directory, and new patient is visible in the list.
- **Scenario B: Field Validation**
  - **Action:** Submit the "Add Patient" form with empty fields.
  - **Expected Outcome:** Form fields turn red using Bootstrap error styles, validation errors display below fields, and submission is prevented.
- **Scenario C: Modify Patient (Update)**
  - **Action:** Edit a patient, change their address, and submit.
  - **Expected Outcome:** Details successfully saved in db, success banner displayed, redirects to directory.
- **Scenario D: Delete Patient with Confirmation**
  - **Action:** Click the "Delete" icon on a patient in the list.
  - **Expected Outcome:** Glassmorphic modal appears asking to confirm.
  - **Action:** Click "Delete Record".
  - **Expected Outcome:** Patient record is deleted from database (cascade deletes associated medical histories), modal closes, list updates automatically.

### Test Case 5: Medical History Profile View
- **Action:** Click the "View Profile" (eye icon) for a patient.
- **Expected Outcome:** Patient profile dashboard renders. Displaying personal card with calculated Age, Gender, Blood Type, and past medical history records sorted chronologically.
- **Action:** Add a new diagnosis (e.g. "Migraine") with notes and click "Add Medical Record".
- **Expected Outcome:** 
  - Input validated.
  - Record saved to database.
  - List of medical history updates dynamically without a full page refresh.

---

## Verification Commands
```bash
# Verify code compiles cleanly
dotnet build

# Run all automated tests
dotnet test
```
