CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [DisplayName] NVARCHAR(100) NOT NULL, 
    [UserName] NVARCHAR(100) NOT NULL UNIQUE, 
    [Email] NVARCHAR(100) NULL, 
    [Active] BIT NOT NULL DEFAULT 0, 
    [IsSuperVisor] BIT NOT NULL DEFAULT 0, 
    [SuperVisorId] INT NULL, 
    [OTP] NVARCHAR(10) NULL,
    [OTPGeneratedOn] DATETIME NULL,
    [Region] NVARCHAR(20) NULL,
    [IsSuperUser] BIT NOT NULL DEFAULT 0, 
    [Password] NVARCHAR(50) NOT NULL DEFAULT '123'
)
