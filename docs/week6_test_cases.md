# Week 6 QA Test Cases — Prescriptions, Notifications, Emergency Contacts & SOS

## Test Cases (Track A — Prescriptions & Notifications)
1. **Prescription Creation** — Add a prescription (medicine, dosage, instructions) from Appointment Detail; verify it appears linked to that appointment.
2. **Prescription Retrieval by Appointment** — `GET /api/prescriptions?appointmentId={id}` returns only prescriptions for that appointment.
3. **Notification Creation** — `POST /api/notifications` creates a notification with `IsRead = false`.
4. **Unread Notification Retrieval** — `GET /api/notifications?userId={id}` returns only unread notifications; bell badge count matches.
5. **Mark Notification as Read** — Clicking a notification calls `PATCH /api/notifications/{id}/read`, badge count decreases, and navigation to `relatedUrl` occurs.

## Test Cases (Track B — Emergency Contacts & SOS)
1. **Add Emergency Contact** — Add a contact from the Emergency Contacts screen; verify it appears in the list.
2. **List Contacts by Patient** — Only contacts belonging to the current patient are shown.
3. **Delete Contact** — Deleting a contact removes it from the list immediately.
4. **SOS Confirmation Flow** — Clicking "SOS" opens a confirmation dialog before sending the alert.
5. **SOS Success/Error Feedback** — Confirming sends `POST /api/emergency/{id}/notify`; success message shown on success, error message shown on failure.