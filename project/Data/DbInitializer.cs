using System;
using System.Linq;
using week1.Models;

namespace week1.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
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
            }
        }
    }
}
