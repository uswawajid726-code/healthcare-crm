-- SQL Server DDL script for creating the Patients table

CREATE TABLE [dbo].[Patients] (
    [Id]            INT            IDENTITY(1,1) NOT NULL,
    [FirstName]     NVARCHAR(50)   NOT NULL,
    [LastName]      NVARCHAR(50)   NOT NULL,
    [Email]         NVARCHAR(100)  NOT NULL,
    [PhoneNumber]   NVARCHAR(20)   NOT NULL,
    [DateOfBirth]   DATETIME2(7)   NOT NULL,
    [Gender]        NVARCHAR(20)   NOT NULL,
    [Address]       NVARCHAR(200)  NOT NULL DEFAULT '',
    [CreatedAt]     DATETIME2(7)   NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_Patients] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Unique index to prevent duplicate patient email entries
CREATE UNIQUE NONCLUSTERED INDEX [IX_Patients_Email]
    ON [dbo].[Patients]([Email] ASC);
GO
