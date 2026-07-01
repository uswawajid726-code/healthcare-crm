-- =========================================================
-- Week 3: Patients Table (full fields) + MedicalHistory Table
-- =========================================================

-- 1. PATIENTS TABLE
-- Note: If this table already exists from Week 1/2 with fewer columns,
-- run the ALTER TABLE section below instead of dropping it.

CREATE TABLE Patients (
    PatientId       INT IDENTITY(1,1) PRIMARY KEY,
    FullName        NVARCHAR(100)   NOT NULL,
    DateOfBirth     DATE            NOT NULL,
    Gender          NVARCHAR(10)    NOT NULL,           -- Male / Female / Other
    Phone           NVARCHAR(11)    NOT NULL,
    Email           NVARCHAR(100)   NULL,
    Address         NVARCHAR(300)   NULL,
    BloodType       NVARCHAR(5)     NULL,               -- A+, A-, B+, B-, AB+, AB-, O+, O-
    Status          NVARCHAR(20)    NOT NULL DEFAULT 'Active',  -- Active / Inactive
    CreatedAt       DATETIME        NOT NULL DEFAULT GETDATE(),
    UpdatedAt       DATETIME        NULL
);

-- Index for fast search by name and phone (used by GET /api/patients?search=)
CREATE INDEX IX_Patients_Search ON Patients (FullName, Phone);


-- =========================================================
-- IF TABLE ALREADY EXISTS FROM WEEK 1/2 — use this instead:
-- =========================================================
-- ALTER TABLE Patients ADD Email NVARCHAR(100) NULL;
-- ALTER TABLE Patients ADD BloodType NVARCHAR(5) NULL;
-- ALTER TABLE Patients ADD Status NVARCHAR(20) NOT NULL DEFAULT 'Active';
-- ALTER TABLE Patients ADD UpdatedAt DATETIME NULL;


-- =========================================================
-- 2. MEDICAL HISTORY TABLE (linked to Patients)
-- =========================================================

CREATE TABLE MedicalHistory (
    MedicalHistoryId   INT IDENTITY(1,1) PRIMARY KEY,
    PatientId           INT             NOT NULL,
    Condition            NVARCHAR(200)   NOT NULL,        -- e.g. "Diabetes Type 2"
    DiagnosedDate        DATE            NULL,
    Notes                 NVARCHAR(500)   NULL,
    Medications          NVARCHAR(300)   NULL,            -- comma separated or free text
    CreatedAt             DATETIME        NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_MedicalHistory_Patients
        FOREIGN KEY (PatientId) REFERENCES Patients(PatientId)
        ON DELETE CASCADE                                  -- delete history if patient deleted
);

CREATE INDEX IX_MedicalHistory_PatientId ON MedicalHistory (PatientId);


-- =========================================================
-- 3. SEED DATA (sample/test data)
-- =========================================================

INSERT INTO Patients (FullName, DateOfBirth, Gender, Phone, Email, Address, BloodType, Status)
VALUES
('Ahmed Ali', '1990-05-14', 'Male', '03001234567', 'ahmed.ali@example.com', 'House 12, Street 5, Lahore', 'B+', 'Active'),
('Sara Khan', '1985-11-02', 'Female', '03217654321', 'sara.khan@example.com', 'Block C, DHA, Karachi', 'O+', 'Active'),
('Bilal Hussain', '2000-02-20', 'Male', '03331122334', NULL, 'Model Town, Lahore', 'A-', 'Inactive'),
('Ayesha Tariq', '1995-08-09', 'Female', '03451239876', 'ayesha.t@example.com', 'F-10, Islamabad', 'AB+', 'Active'),
('Usman Sheikh', '1978-12-25', 'Male', '03009988776', NULL, 'Gulshan-e-Iqbal, Karachi', 'O-', 'Active');

INSERT INTO MedicalHistory (PatientId, Condition, DiagnosedDate, Notes, Medications)
VALUES
(1, 'Hypertension', '2022-03-10', 'Mild, controlled with medication', 'Amlodipine 5mg daily'),
(1, 'Seasonal Allergy', '2021-06-15', 'Spring season only', 'Cetirizine as needed'),
(2, 'Diabetes Type 2', '2020-01-20', 'Diet controlled', 'Metformin 500mg twice daily'),
(4, 'Asthma', '2019-09-05', 'Mild, exercise-induced', 'Salbutamol inhaler as needed'),
(5, 'Hypertension', '2023-02-18', 'Newly diagnosed', 'Lisinopril 10mg daily');
