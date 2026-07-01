# Member B — Week 3 Test Cases
## Patient CRUD + Search Feature

---

### TC-MB3-01: Search Patients by Name

**Steps:**
1. Go to `/Patients/PatientList`
2. Type `Ahmed` in search bar

**Expected Result:**
- `GET /api/patients?search=Ahmed` is called
- Only patients with "Ahmed" in their name appear in the table
- Result count text updates correctly

**Pass Criteria:** Search filters by name correctly, no full page reload needed.

---

### TC-MB3-02: Search Patients by Phone Number

**Steps:**
1. Go to Patient List
2. Type `0300` in search bar (partial phone number)

**Expected Result:**
- API call: `GET /api/patients?search=0300`
- All patients whose phone contains `0300` are shown
- Patients matching by name are also still included (search checks both fields)

**Pass Criteria:** Search works on partial phone number match.

---

### TC-MB3-03: Add Patient — Duplicate Phone Number Rejected

**Steps:**
1. Add a patient with phone `03001234567`
2. Try adding another patient using the same phone `03001234567`

**Expected Result:**
- API returns `400 Bad Request` with message: "A patient with this phone number already exists."
- Form stays on page, error shown to user
- Second patient is NOT created

**Pass Criteria:** Duplicate phone numbers blocked at API level.

---

### TC-MB3-04: Edit Patient — Age Recalculates Automatically

**Precondition:** Patient exists with DOB `1990-05-14`.

**Steps:**
1. Open Patient List
2. Check the Age column value

**Expected Result:**
- Age is calculated dynamically from DOB (not stored as a static field)
- If today is June 21, 2026, Ahmed Ali (DOB 1990-05-14) should show Age = 36

**Pass Criteria:** Age column always reflects current age regardless of when patient was added.

---

### TC-MB3-05: Delete Patient — Role Restriction

**Precondition:** Two test accounts exist — one with role `Receptionist`, one with role `Admin`.

**Steps:**
1. Log in as `Receptionist`
2. Try to delete a patient via the delete button

**Expected Result:**
- API returns `403 Forbidden` (Receptionist is not authorized for DELETE)
- Frontend shows an error toast / redirects to Unauthorized page

**Steps (continued):**
3. Log out, log in as `Admin`
4. Try deleting the same patient

**Expected Result:**
- Delete succeeds, patient removed from list, linked MedicalHistory rows cascade-deleted

**Pass Criteria:** Only Admin role can delete patients; Receptionist is blocked.

---

## Summary Table

| TC ID       | Feature                          | Type         | Priority |
|-------------|-----------------------------------|--------------|----------|
| TC-MB3-01   | Search by name                   | API/Frontend | High     |
| TC-MB3-02   | Search by phone                  | API/Frontend | High     |
| TC-MB3-03   | Duplicate phone validation       | API          | Critical |
| TC-MB3-04   | Age auto-calculation             | Frontend     | Medium   |
| TC-MB3-05   | Role-based delete restriction    | Security     | Critical |
