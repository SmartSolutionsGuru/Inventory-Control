USE [SmartBooksRetailDB]
GO
SET IDENTITY_INSERT dbo.Province ON;
INSERT INTO [dbo].[Province]
           ([Id]
		   ,[Name]
           ,[CountryId]
           ,[Description]
           ,[IsActive]
           ,[CreatedAt]
           ,[UpdatedAt]
           ,[CreatedBy]
           ,[UpdatedBy])
     VALUES
          (1,'Punjab',162, Null,1,GETDATE(),Null,Null,Null ),
		  (2,'Sindh',162, Null,1,GETDATE(),Null,Null,Null ),
		  (3,'Balochistan',162, Null,1,GETDATE(),Null,Null,Null ),
		  (4,'North-West Frontier',162, Null,1,GETDATE(),Null,Null,Null ),
		  (5,'Northern Areas',162, Null,1,GETDATE(),Null,Null,Null ),
		  (6,'Federally Administered Tribal Areas',162, Null,1,GETDATE(),Null,Null,Null ),
		  (7,'Azad Kashmir',162, Null,1,GETDATE(),Null,Null,Null ),
		  (8,'Islamabad',162, Null,1,GETDATE(),Null,Null,Null );
GO
SET IDENTITY_INSERT dbo.Country OFF;