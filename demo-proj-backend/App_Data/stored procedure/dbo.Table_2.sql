CREATE TABLE [dbo].[tbl_registration] (
    [id]        INT          IDENTITY (1, 1) NOT NULL,
    [firstName] VARCHAR (50) NULL,
    [lastName]  VARCHAR (50) NULL,
    [email]     VARCHAR (50) NULL,
    [mobile]    NUMERIC (13) NULL,
    [password]  CHAR (8)     NULL,
    [dob]       DATE         NULL,
    [createdAt] DATETIME     NULL,
    [updatedAt] NCHAR (10)   NULL,
    [isActive]  INT          NULL,
    CONSTRAINT [PK__tbl_regi__3213E83FD843C60B] PRIMARY KEY CLUSTERED ([id] ASC)
);

