# healthcare-crm
Our group project for a Healthcare CRM using ASP.NET Core. Covering everything from secure user login to patient management, backed by a clean REST API
## Test Cases

| Test Case | Expected Result |
|---|---|
| Register with valid data | User created + token |
| Login with correct credentials | JWT token generated |
| Login with wrong password | Unauthorized |
| Access patient API without token | 401 Unauthorized |
| Access patient API with valid token | Success |
