CREATE DATABASE ComplaintDB;

USE ComplaintDB;

-- Users Table

CREATE TABLE Users
(
    UserId INT PRIMARY KEY IDENTITY(1,1),
    UserName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    Password VARCHAR(100) NOT NULL,
    Role VARCHAR(20) NOT NULL,
    IsActive BIT DEFAULT 1
);

-- Complaint Categories Table

CREATE TABLE ComplaintCategories
(
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName VARCHAR(100) NOT NULL,
    IsActive BIT DEFAULT 1
);

-- Complaints Table

CREATE TABLE Complaints
(
    ComplaintId INT PRIMARY KEY IDENTITY(1,1),

    UserId INT NOT NULL,

    CategoryId INT NOT NULL,

    Subject VARCHAR(200) NOT NULL,

    Description VARCHAR(MAX) NOT NULL,

    Status VARCHAR(50) DEFAULT 'Open',

    AssignedTo INT NULL,

    CreatedDate DATETIME DEFAULT GETDATE(),

    ResolvedDate DATETIME NULL,

    CONSTRAINT FK_Complaints_Users
    FOREIGN KEY(UserId)
    REFERENCES Users(UserId),

    CONSTRAINT FK_Complaints_Categories
    FOREIGN KEY(CategoryId)
    REFERENCES ComplaintCategories(CategoryId)
);

-- Error Log Table

CREATE TABLE ErrorLog
(
    Id INT PRIMARY KEY IDENTITY(1,1),

    ErrorMessage VARCHAR(MAX),

    CreatedDate DATETIME DEFAULT GETDATE()
);

-- Insert Users

INSERT INTO Users
(UserName,Email,Password,Role,IsActive)

VALUES
('Admin',
'admin@gmail.com',
'123',
'Admin',
1),

('Raju',
'raju@gmail.com',
'123',
'User',
1),

('Support1',
'support@gmail.com',
'123',
'Support',
1);


-- Insert Categories

INSERT INTO ComplaintCategories
(CategoryName,IsActive)

VALUES
('Software',1),
('Hardware',1),
('Network',1),
('Login Issue',1),
('Database',1);


-- Insert Complaints

INSERT INTO Complaints
(
UserId,
CategoryId,
Subject,
Description,
Status,
AssignedTo
)

VALUES
(
2,
1,
'Software Crash',
'Application closes automatically',
'Open',
NULL
),

(
2,
3,
'Internet Problem',
'Network not working properly',
'In Progress',
1
);


SELECT * FROM Users;

SELECT * FROM ComplaintCategories;

SELECT * FROM Complaints;

SELECT * FROM ErrorLog;

EXEC sp_help Complaints;