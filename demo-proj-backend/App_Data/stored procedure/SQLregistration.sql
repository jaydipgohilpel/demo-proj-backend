-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE insertRegistration 
	-- Add the parameters for the stored procedure here
       @firstName varchar(50),
       @lastName varchar(50),
       @email varchar(50),
       @mobile numeric(13,0),
       @password char(8),
       @dob date,
       @createdAt datetime,
       @updatedAt datetime,
       @isActive int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 INSERT INTO tbl_registration
              (firstName,lastName,email,mobile,password,dob,createdAt,updatedAt,isActive)
       VALUES
              (@firstName,@lastName,@email,@mobile,@password,@dob,@createdAt,@updatedAt,@isActive)
END
GO
