-- USP_GetComplaints
CREATE OR ALTER PROCEDURE [dbo].[USP_GetComplaints]
(
    @UserId INT = NULL,
    @Role VARCHAR(20) = NULL
)
AS
BEGIN

    IF(@Role='Admin' OR @Role='Support')
    BEGIN
        SELECT
            C.ComplaintId,
            U.UserName,
            CC.CategoryName,
            C.Subject,
            C.Description,
            C.Status,
            C.AssignedTo,
            AU.UserName AS AssignedToName,
            C.CreatedDate,
            C.ResolvedDate

        FROM Complaints C

        INNER JOIN Users U
            ON C.UserId = U.UserId

        INNER JOIN ComplaintCategories CC
            ON C.CategoryId = CC.CategoryId

        LEFT JOIN Users AU
            ON C.AssignedTo = AU.UserId
    END

    ELSE
    BEGIN
        SELECT
            C.ComplaintId,
            U.UserName,
            CC.CategoryName,
            C.Subject,
            C.Description,
            C.Status,
            C.AssignedTo,
            AU.UserName AS AssignedToName,
            C.CreatedDate,
            C.ResolvedDate

        FROM Complaints C

        INNER JOIN Users U
            ON C.UserId = U.UserId

        INNER JOIN ComplaintCategories CC
            ON C.CategoryId = CC.CategoryId

        LEFT JOIN Users AU
            ON C.AssignedTo = AU.UserId

        WHERE C.UserId = @UserId
    END

END

--USP_InsertComplaint
-- Insert Complaint
 PROCEDURE USP_InsertComplaint
(
    @UserId INT,
    @CategoryId INT,
    @Subject VARCHAR(200),
    @Description VARCHAR(MAX)
)
AS
BEGIN

    INSERT INTO Complaints
    (
        UserId,
        CategoryId,
        Subject,
        Description,
        Status,
        CreatedDate
    )

    VALUES
    (
        @UserId,
        @CategoryId,
        @Subject,
        @Description,
        'Open',
        GETDATE()
    )

END

--USP_UpdateComplaint
-- Update Complaint

CREATE PROCEDURE USP_UpdateComplaint
(
    @ComplaintId INT,
    @CategoryId INT,
    @Subject VARCHAR(200),
    @Description VARCHAR(MAX)
)
AS
BEGIN

    UPDATE Complaints

    SET
    CategoryId=@CategoryId,
    Subject=@Subject,
    Description=@Description

    WHERE ComplaintId=@ComplaintId
    AND Status='Open'

END

--USP_DeleteComplaint
-- Delete Complaint

CREATE PROCEDURE USP_DeleteComplaint
(
    @ComplaintId INT
)
AS
BEGIN

    DELETE FROM Complaints

    WHERE ComplaintId=@ComplaintId
    AND Status='Open'

END

--USP_SearchComplaints

-- Search Complaints

CREATE PROCEDURE USP_SearchComplaints
(
    @Subject VARCHAR(200)=NULL,
    @Status VARCHAR(50)=NULL,
    @CategoryId INT=NULL
)
AS
BEGIN

    SELECT
    C.ComplaintId,
    U.UserName,
    CC.CategoryName,
    C.Subject,
    C.Description,
    C.Status,
    C.CreatedDate

    FROM Complaints C

    INNER JOIN Users U
    ON C.UserId=U.UserId

    INNER JOIN ComplaintCategories CC
    ON C.CategoryId=CC.CategoryId

    WHERE
    (@Subject IS NULL OR C.Subject LIKE '%'+@Subject+'%')
    AND
    (@Status IS NULL OR C.Status=@Status)
    AND
    (@CategoryId IS NULL OR C.CategoryId=@CategoryId)

END

--USP_AssignComplaint
-- Assign Complaint

CREATE PROCEDURE USP_AssignComplaint
(
    @ComplaintId INT,
    @AssignedTo INT
)
AS
BEGIN

    UPDATE Complaints

    SET AssignedTo=@AssignedTo,
        Status='In Progress'

    WHERE ComplaintId=@ComplaintId

END

--USP_UpdateComplaintStatus
-- Update Complaint Status

CREATE PROCEDURE USP_UpdateComplaintStatus
(
    @ComplaintId INT,
    @Status VARCHAR(50)
)
AS
BEGIN

    UPDATE Complaints

    SET
    Status=@Status,

    ResolvedDate=
    CASE
        WHEN @Status='Resolved'
        THEN GETDATE()

        ELSE NULL
    END

    WHERE ComplaintId=@ComplaintId

END

--USP_GetComplaintDashboard
-- Dashboard Counts



CREATE OR ALTER PROCEDURE [dbo].[USP_GetComplaintDashboard]
(
    @Role VARCHAR(20),
    @UserId INT
)
AS
BEGIN

    IF(@Role='Admin')
    BEGIN

        SELECT
        COUNT(CASE WHEN Status='Open' THEN 1 END) OpenCount,

        COUNT(CASE WHEN Status='In Progress' THEN 1 END) AssignedCount,

        COUNT(CASE WHEN Status='Resolved' THEN 1 END) ResolvedCount,

        COUNT(CASE WHEN Status='Rejected' THEN 1 END) RejectedCount

        FROM Complaints
    END

    ELSE
    BEGIN

        SELECT
        COUNT(*) TotalComplaints,

        COUNT(CASE WHEN Status='Open' THEN 1 END) OpenCount,

        COUNT(CASE WHEN Status='Resolved' THEN 1 END) ResolvedCount,
        COUNT(CASE WHEN Status='Rejected' THEN 1 END) RejectedCount


        FROM Complaints

        WHERE UserId=@UserId

    END

END






EXEC USP_GetComplaints 2,'User';

EXEC USP_InsertComplaint
2,
1,
'Login Error',
'Unable to login';



EXEC USP_UpdateComplaint
1,
2,
'Updated Subject',
'Updated Description';

EXEC USP_DeleteComplaint 1;

EXEC USP_SearchComplaints
'Login',
'Open',
1;

EXEC USP_AssignComplaint
2,
1;

EXEC USP_UpdateComplaintStatus
2,
'Resolved';

EXEC USP_GetComplaintDashboard
'Admin',
1;



----------------------------Login & Register Procedure -------------------

CREATE OR ALTER PROCEDURE USP_RegisterUser
(
    @UserName VARCHAR(100),
    @Email VARCHAR(100),
    @Password VARCHAR(100),
    @Role VARCHAR(20)
)
AS
BEGIN

    INSERT INTO Users
    (
        UserName,
        Email,
        Password,
        Role,
        IsActive
    )
    VALUES
    (
        @UserName,
        @Email,
        @Password,
        @Role,
        1
    )

END

CREATE OR ALTER PROCEDURE USP_LoginUser
(
    @Email VARCHAR(100),
    @Password VARCHAR(100)
)
AS
BEGIN

    SELECT
        UserId,
        UserName,
        Email,
        Role
    FROM Users
    WHERE Email = @Email
      AND Password = @Password
      AND IsActive = 1

END








EXEC USP_GetComplaints
    @UserId = 1,
    @Role = 'Admin'


    --Single complaint details
CREATE OR ALTER PROCEDURE USP_GetComplaintById
(
    @ComplaintId INT
)
AS
BEGIN
    SELECT
        C.ComplaintId,
        C.UserId,
        C.CategoryId,
        C.Subject,
        C.Description,
        C.Status,
        C.AssignedTo,
        C.CreatedDate,
        C.ResolvedDate,
        CC.CategoryName,
        U.UserName
    FROM Complaints C
    INNER JOIN ComplaintCategories CC
        ON C.CategoryId = CC.CategoryId
    INNER JOIN Users U
        ON C.UserId = U.UserId
    WHERE C.ComplaintId = @ComplaintId
END




CREATE OR ALTER PROCEDURE USP_GetComplaintChartData
(
    @UserId INT,
    @Role NVARCHAR(20),
    @FilterType NVARCHAR(20)
)
AS
BEGIN

    IF @FilterType = 'user'
    BEGIN
        SELECT
            U.UserName AS Name,
            C.Status,
            COUNT(*) AS Count
        FROM Complaints C
        INNER JOIN Users U
            ON C.UserId = U.UserId
        GROUP BY
            U.UserName,
            C.Status
        HAVING COUNT(*) > 0
    END
    ELSE
    BEGIN
        SELECT
            CC.CategoryName AS Name,
            C.Status,
            COUNT(*) AS Count
        FROM Complaints C
        INNER JOIN ComplaintCategories CC
            ON C.CategoryId = CC.CategoryId
        GROUP BY
            CC.CategoryName,
            C.Status
        HAVING COUNT(*) > 0
    END
END

EXEC USP_GetComplaintChartData
    @UserId = 1,
    @Role = 'Admin',
    @FilterType = 'category'