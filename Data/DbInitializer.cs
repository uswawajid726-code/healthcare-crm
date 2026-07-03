using System;
using System.Linq;
using week1.Models;

namespace week1.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            // Seed Users (Admin, Doctors, Receptionist)
            if (!context.Users.Any())
            {
                var admin = new ApplicationUser
                {
                    Username = "admin@healthcare.com",
                    Email = "admin@healthcare.com",
                    FullName = "System Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPass123!"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                var doctor1 = new ApplicationUser
                {
                    Username = "doctor@healthcare.com",
                    Email = "doctor@healthcare.com",
                    FullName = "Dr. Sarah Connor",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("DoctorPass123!"),
                    Role = "Doctor",
                    CreatedAt = DateTime.UtcNow
                };

                var doctor2 = new ApplicationUser
                {
                    Username = "doctor2@healthcare.com",
                    Email = "doctor2@healthcare.com",
                    FullName = "Dr. John Watson",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor2Pass!"),
                    Role = "Doctor",
                    CreatedAt = DateTime.UtcNow
                };

                var receptionist = new ApplicationUser
                {
                    Username = "receptionist@healthcare.com",
                    Email = "receptionist@healthcare.com",
                    FullName = "Receptionist Rachel",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("RecepPass123!"),
                    Role = "Receptionist",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.AddRange(admin, doctor1, doctor2, receptionist);
                context.SaveChanges();
            }

            // Seed Patients
            if (!context.Patients.Any())
            {
                var patient1 = new Patient
                {
                    FullName = "Jane Doe",
                    DateOfBirth = new DateTime(1990, 5, 15),
                    Gender = "Female",
                    Phone = "03001234567",
                    Email = "jane@example.com",
                    Address = "House 123, Sector G-11, Islamabad",
                    BloodType = "O+",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10)
                };

                var patient2 = new Patient
                {
                    FullName = "John Smith",
                    DateOfBirth = new DateTime(1985, 8, 20),
                    Gender = "Male",
                    Phone = "03007654321",
                    Email = "johnsmith@example.com",
                    Address = "Flat 4B, Sector F-6, Islamabad",
                    BloodType = "A-",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                };

                context.Patients.AddRange(patient1, patient2);
                context.SaveChanges();

                // Seed Medical History
                if (!context.MedicalHistories.Any())
                {
                    context.MedicalHistories.AddRange(
                        new MedicalHistory
                        {
                            PatientId = patient1.Id,
                            Diagnosis = "Hypertension",
                            Notes = "Patient diagnosed with stage 1 hypertension. Prescribed Lisinopril 10mg daily. Advised low-sodium diet and lifestyle modifications.",
                            CreatedAt = DateTime.UtcNow.AddDays(-9)
                        },
                        new MedicalHistory
                        {
                            PatientId = patient1.Id,
                            Diagnosis = "Seasonal Allergies",
                            Notes = "Patient reports itchy eyes and sneezing during spring. Prescribed Cetirizine 10mg as needed.",
                            CreatedAt = DateTime.UtcNow.AddDays(-2)
                        },
                        new MedicalHistory
                        {
                            PatientId = patient2.Id,
                            Diagnosis = "Type 2 Diabetes",
                            Notes = "HbA1c level is 7.2%. Prescribed Metformin 500mg twice daily. Referred to a nutritionist for dietary management.",
                            CreatedAt = DateTime.UtcNow.AddDays(-4)
                        }
                    );
                    context.SaveChanges();
                }

                // Seed Appointments
                if (!context.Appointments.Any())
                {
                    var docSarah = context.Users.FirstOrDefault(u => u.Email == "doctor@healthcare.com");
                    var docJohn = context.Users.FirstOrDefault(u => u.Email == "doctor2@healthcare.com");

                    if (docSarah != null && docJohn != null)
                    {
                        context.Appointments.AddRange(
                            new Appointment
                            {
                                PatientId = patient1.Id,
                                DoctorId = docSarah.Id,
                                AppointmentDate = DateTime.Today.AddDays(1),
                                AppointmentTime = "10:00 AM",
                                Reason = "Chronic Hypertension checkup and prescription renewal",
                                Status = "Scheduled",
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            },
                            new Appointment
                            {
                                PatientId = patient2.Id,
                                DoctorId = docSarah.Id,
                                AppointmentDate = DateTime.Today.AddDays(2),
                                AppointmentTime = "11:30 AM",
                                Reason = "Discuss diabetes lab test results",
                                Status = "Scheduled",
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            },
                            new Appointment
                            {
                                PatientId = patient1.Id,
                                DoctorId = docJohn.Id,
                                AppointmentDate = DateTime.Today,
                                AppointmentTime = "02:00 PM",
                                Reason = "Follow-up consultation after flu symptoms",
                                Status = "Completed",
                                CreatedAt = DateTime.UtcNow.AddDays(-1),
                                UpdatedAt = DateTime.UtcNow.AddDays(-1)
                            }
                        );
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
