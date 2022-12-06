USE [D:\JAYDIP\DOTNETBACKEND\DEMO-PROJ-BACKEND\DEMO-PROJ-BACKEND\APP_DATA\DATABASE.MDF]
GO
INSERT INTO [dbo].[tbl_registration] (
        [firstName],
        [lastName],
        [email],
        [mobile],
        [password],
        [dob],
        [createdAt],
        [updatedAt],
        [isActive]
    )
VALUES (
        'jaydip',
        'gohil',
        'ewjejj@jdfj.com',
        1234,
        '@2Ss123',
        '02/03/2022',
        '02/03/2022',
        '02/03/2022',
        1
    )
GO