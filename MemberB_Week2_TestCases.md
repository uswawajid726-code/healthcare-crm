# Member B — Week 2 Test Cases
## Patient Module: List, Add, Edit + Loading/Empty States

---

### TC-MB-01: Patient List — Empty State (No Patients in Database)

**Precondition:** Database has zero patients. User is logged in.

**Steps:**
1. Navigate to `/Patients/PatientList`
2. Observe the page content

**Expected Result:**
- No table is rendered
- Empty state UI shown: icon + "No patients yet" heading + "Add your first patient to get started"
- "Add Patient" button visible in empty state

**Pass Criteria:** Empty state renders without errors and Add Patient button is functional.

---

### TC-MB-02: Patient List — Search Returns No Results (Empty Search State)

**Precondition:** Database has patients, but none match the search term.

**Steps:**
1. Navigate to `/Patients/PatientList`
2. Type `zzzztest` in the search bar
3. Wait for debounce (400ms) or press Enter

**Expected Result:**
- Table disappears
- Empty search state shown: "No results for 'zzzztest'"
- "Clear search" link visible
- Clicking "Clear search" reloads full list

**Pass Criteria:** Search empty state displays correct query term and clear link works.

---

### TC-MB-03: Add Patient — Client-Side Validation (Required Fields)

**Precondition:** User is on the Add Patient page `/Patients/AddEditPatient`.

**Steps:**
1. Leave all fields blank
2. Click "Add Patient" submit button

**Expected Result:**
- Form does NOT submit (no network request)
- All required fields highlighted in red (`is-invalid` class)
- Individual error messages shown below each field:
  - Full Name → "Full name is required."
  - CNIC → "CNIC is required."
  - Phone → "Phone number is required."
  - Date of Birth → "Date of birth is required."
  - Gender → "Please select a gender."
- Focus moves to first invalid field

**Pass Criteria:** All 5 required fields show validation errors, form stays on page.

---

### TC-MB-04: Add Patient — CNIC Format Validation

**Precondition:** User is on the Add Patient page.

**Steps:**
1. Fill all other required fields correctly
2. Enter CNIC as `1234567890` (no dashes)
3. Click out of the CNIC field (blur event)

**Expected Result:**
- Field turns red with error: "CNIC format: 42101-1234567-1"
- Try submitting — form blocked

**Steps (correct format):**
4. Clear the field, type `42101-1234567-1`
5. Click out of field

**Expected Result:**
- Field turns green (`is-valid`)
- No error message

**Pass Criteria:** Invalid CNIC blocked, correct format accepted.

---

### TC-MB-05: Edit Patient — Loading State + Pre-Populated Form

**Precondition:** At least one patient exists in the database with ID = 1.

**Steps:**
1. On Patient List, click the Edit (pencil) icon for a patient
2. Observe the page while navigating to `/Patients/AddEditPatient/1`
3. Check the form fields after page load

**Expected Result:**
- Page title shows "Edit Patient" (not "Add Patient")
- Breadcrumb shows: Patients > Edit Patient
- All fields pre-populated with existing patient data (FullName, CNIC, Phone, DOB, Gender, Address, BloodGroup)
- Submit button label is "Save Changes"
- Editing a field and clicking "Save Changes" calls PUT `/api/patients/1`
- On success, redirected to Patient List with success toast: "Patient updated successfully."

**Pass Criteria:** Edit form loads correct data, save triggers PUT endpoint, redirect on success.

---

## Summary Table

| TC ID      | Feature                          | Type              | Priority |
|------------|----------------------------------|-------------------|----------|
| TC-MB-01   | Empty state — no patients        | UI/UX             | High     |
| TC-MB-02   | Empty state — no search results  | UI/UX             | High     |
| TC-MB-03   | Add form — required field check  | Client Validation | Critical |
| TC-MB-04   | CNIC format validation           | Client Validation | High     |
| TC-MB-05   | Edit patient — pre-populate form | Integration       | Critical |
