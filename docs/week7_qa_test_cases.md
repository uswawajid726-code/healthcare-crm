# Healthcare CRM - Week 7 QA Test Cases Document

## Overview
This document contains the quality assurance test cases for the Week 7 Analytics & Polish sprint of the Healthcare CRM application.

---

## Summary of Test Cases

| Test Case ID | Category | Title | Priority |
| :--- | :--- | :--- | :--- |
| **TC-ANA-01** | API | Patient Analytics API Endpoint Validation (`GET /api/analytics/patients`) | High |
| **TC-ANA-02** | API | Appointment Analytics API Endpoint Validation (`GET /api/analytics/appointments`) | High |
| **TC-ANA-03** | API | Doctor Analytics API Endpoint Validation (`GET /api/analytics/doctors`) | High |
| **TC-ANA-04** | UI / Data Sync | Stat Cards Live Data Verification | High |
| **TC-ANA-05** | UI / Chart | Chart 1: Patient Gender Distribution Pie Chart Validation | Medium |
| **TC-ANA-06** | UI / Chart | Chart 2: 30-Day Appointments Volume Line Chart Validation | High |
| **TC-ANA-07** | UI / Chart | Chart 3: Appointments Per Doctor Bar Chart Validation | Medium |
| **TC-ANA-08** | Error Handling | Network Failure & API Exception Handling Test | High |
| **TC-ANA-09** | Security | Unauthenticated API Access Protection Test (JWT Enforcement) | Critical |
| **TC-ANA-10** | UI / UX | Empty State & Loading Indicator Verification | Medium |

---

## Detailed Test Cases

### TC-ANA-01: Patient Analytics API Endpoint Validation
- **Module**: Analytics API (Member C)
- **Endpoint**: `GET /api/analytics/patients`
- **Pre-conditions**: Valid JWT Bearer token supplied in request header (`Authorization: Bearer <token>`).
- **Test Steps**:
  1. Send a GET request to `/api/analytics/patients`.
  2. Inspect the HTTP status code.
  3. Validate the JSON payload response structure.
- **Expected Result**:
  - HTTP Status: `200 OK`.
  - JSON Body contains `success: true`, `message`, and `data` object with `totalPatients` (integer), `newPatientsThisMonth` (integer), and `genderDistribution` (dictionary with gender key-value pairs).
- **Status**: Pass

---

### TC-ANA-02: Appointment Analytics API Endpoint Validation
- **Module**: Analytics API (Member C)
- **Endpoint**: `GET /api/analytics/appointments`
- **Pre-conditions**: Valid JWT Bearer token.
- **Test Steps**:
  1. Send a GET request to `/api/analytics/appointments`.
  2. Verify response headers and JSON body format.
  3. Verify that `dailyAppointments` contains exactly 30 entries corresponding to the last 30 consecutive calendar days.
- **Expected Result**:
  - HTTP Status: `200 OK`.
  - JSON Body contains `totalAppointments`, `todayAppointments`, and an array `dailyAppointments` where each item has `date` (YYYY-MM-DD) and `count` (integer >= 0).
- **Status**: Pass

---

### TC-ANA-03: Doctor Analytics API Endpoint Validation
- **Module**: Analytics API (Member C)
- **Endpoint**: `GET /api/analytics/doctors`
- **Pre-conditions**: Active staff account authenticated via JWT.
- **Test Steps**:
  1. Send a GET request to `/api/analytics/doctors`.
  2. Verify that all doctors in the user registry are represented in `doctorAppointments`.
  3. Confirm `appointmentCount` matches appointments scheduled for each doctor in the current month.
- **Expected Result**:
  - HTTP Status: `200 OK`.
  - JSON Body contains `totalDoctors` (integer) and `doctorAppointments` array with `doctorId`, `doctorName`, and `appointmentCount`.
- **Status**: Pass

---

### TC-ANA-04: Stat Cards Live Data Verification
- **Module**: Analytics Dashboard (Member B)
- **Pre-conditions**: Authenticated user logged in and navigating to `/Home/AnalyticsDashboard`.
- **Test Steps**:
  1. Open `/Home/AnalyticsDashboard` in a web browser.
  2. Observe the initial state of stat cards while data fetches.
  3. Compare displayed card values against raw API response values from `/api/analytics/*`.
- **Expected Result**:
  - Stat cards display loading skeleton pulses initially.
  - Card values for Total Patients, New Patients This Month, Total Doctors, and Today's Appointments accurately match the values returned by the API.
- **Status**: Pass

---

### TC-ANA-05: Chart 1 (Patient Gender Distribution Pie Chart) Validation
- **Module**: Analytics Dashboard (Member B)
- **Pre-conditions**: Patient records with assigned genders exist in the database.
- **Test Steps**:
  1. Navigate to `/Home/AnalyticsDashboard`.
  2. Inspect the "Patient Gender Distribution" chart element (`#genderPieChart`).
  3. Hover over pie segments and check tooltips.
- **Expected Result**:
  - Chart renders as an interactive Pie/Doughnut chart using Chart.js.
  - Legend items display distinct color tags for Male, Female, etc.
  - Tooltips show accurate category names and patient count values matching API data.
- **Status**: Pass

---

### TC-ANA-06: Chart 2 (30-Day Appointments Line Chart) Validation
- **Module**: Analytics Dashboard (Member B)
- **Pre-conditions**: Application running with historical appointment data.
- **Test Steps**:
  1. Navigate to `/Home/AnalyticsDashboard`.
  2. Locate "Appointments Volume (Last 30 Days)" line chart (`#appointmentsLineChart`).
  3. Check horizontal axis dates and data points.
- **Expected Result**:
  - Line chart plots 30 continuous date points on the X-axis.
  - Days with 0 appointments render gracefully with data points on the zero line without breaking line continuity.
- **Status**: Pass

---

### TC-ANA-07: Chart 3 (Appointments Per Doctor Bar Chart) Validation
- **Module**: Analytics Dashboard (Member B)
- **Pre-conditions**: Doctor accounts present in the system.
- **Test Steps**:
  1. Navigate to `/Home/AnalyticsDashboard`.
  2. Inspect "Appointments Per Doctor" bar chart (`#doctorBarChart`).
  3. Check bar heights and doctor name labels on the X-axis.
- **Expected Result**:
  - Bar chart displays styled vertical bars corresponding to doctor names.
  - Bar height reflects the exact count of appointments scheduled for that doctor in the current month.
- **Status**: Pass

---

### TC-ANA-08: Network Failure & API Exception Handling Test
- **Module**: Error Handling (Member B & C)
- **Pre-conditions**: User on `/Home/AnalyticsDashboard`.
- **Test Steps**:
  1. Simulate server disconnect or stop the backend service.
  2. Click "Refresh Live Data".
  3. Observe UI response.
- **Expected Result**:
  - Application does NOT crash or break layout.
  - A clean, user-friendly error banner (`#analytics-error-banner`) appears displaying a readable message without leaking raw stack traces or exceptions.
- **Status**: Pass

---

### TC-ANA-09: Unauthenticated API Access Protection Test
- **Module**: Security / Authorization
- **Pre-conditions**: No JWT token in localStorage / headers.
- **Test Steps**:
  1. Clear browser localStorage token.
  2. Send direct GET requests to `/api/analytics/patients`, `/api/analytics/appointments`, and `/api/analytics/doctors`.
  3. Attempt to navigate directly to `/Home/AnalyticsDashboard`.
- **Expected Result**:
  - API endpoints return HTTP Status `401 Unauthorized`.
  - Browser automatically redirects unauthenticated user to `/Account/Login`.
- **Status**: Pass

---

### TC-ANA-10: Empty State & Loading Indicator Verification
- **Module**: UI Polish (Member B)
- **Pre-conditions**: Empty database or new database instance.
- **Test Steps**:
  1. Start application with zero appointments/doctors.
  2. Open `/Home/AnalyticsDashboard`.
  3. Verify chart container visual states.
- **Expected Result**:
  - Loading text/indicators show briefly.
  - When API returns empty datasets, dedicated empty-state text (e.g. "No appointments booked in the last 30 days") is rendered instead of blank canvas glitches.
- **Status**: Pass
