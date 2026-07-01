# Week 2 - Quality Assurance Test Cases

This document describes the test cases designed to verify the Week 2 Role-Based Access Control and Authentication Hardening features.

---

## Test Case 1: Successful Registration
*   **Description:** Verify that a user can register successfully with valid credentials, providing a Full Name and selecting an allowed role (Admin, Doctor, or Receptionist).
*   **Steps to Reproduce:**
    1. Navigate to `/Account/Register`.
    2. Enter a unique email address, a Full Name, and a password (at least 6 characters).
    3. Select a role (e.g., "Doctor") from the dropdown.
    4. Click "Register & Get Started".
*   **Expected Behavior:**
    *   The user is successfully registered in the database.
    *   A JWT token is generated and stored in `localStorage` and the `token` cookie.
    *   The user is redirected to the home page `/Home/Index` and then redirected to their specific dashboard (`/Home/DoctorDashboard`).

---

## Test Case 2: Successful Login
*   **Description:** Verify that a registered user can log in successfully using their credentials.
*   **Steps to Reproduce:**
    1. Navigate to `/Account/Login`.
    2. Enter the email/username and password of the registered account.
    3. Click "Sign In".
*   **Expected Behavior:**
    *   The API returns a success response with a JWT token.
    *   The token is saved in client storage (`localStorage` and the `token` cookie).
    *   The client is redirected to the appropriate role-based dashboard.

---

## Test Case 3: Invalid Login Handling
*   **Description:** Verify that correct error messages are returned and shown when logging in with incorrect credentials.
*   **Steps to Reproduce:**
    1. Navigate to `/Account/Login`.
    2. Enter an invalid email/username or an incorrect password.
    3. Click "Sign In".
*   **Expected Behavior:**
    *   The form triggers validation or the server returns `401 Unauthorized`.
    *   An alert is displayed saying "Invalid email/username or password."
    *   The page does not redirect, keeping the user on the login page.

---

## Test Case 4: Unauthorized Access Prevention
*   **Description:** Verify that unauthenticated requests to protected endpoints are blocked and redirected to the login page.
*   **Steps to Reproduce:**
    1. Clear the browser cache/cookies or sign out.
    2. Try to navigate directly to `/Home/Index` or `/PatientView`.
*   **Expected Behavior:**
    *   The server detects the lack of a JWT token cookie.
    *   The request is blocked.
    *   The user is redirected to `/Account/Login`.

---

## Test Case 5: Correct Role-Based Redirects
*   **Description:** Verify that users are redirected only to the dashboards matching their specific role, and blocked from others.
*   **Steps to Reproduce:**
    1. Register/Login as a "Receptionist".
    2. Verify you are automatically redirected to `/Home/ReceptionistDashboard`.
    3. Try navigating manually to `/Home/AdminDashboard`.
*   **Expected Behavior:**
    *   The request to `/Home/AdminDashboard` is evaluated against the `[Authorize(Roles = "Admin")]` attribute.
    *   Since the user's role is "Receptionist", the request is rejected as forbidden.
    *   The user is redirected to `/Account/UnauthorizedPage` (Access Denied).
