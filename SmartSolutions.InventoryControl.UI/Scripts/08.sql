INSERT INTO IdentityUser(UserName,FullName,Email,SecretKey,Password,MobileNumber,IsActive,CreatedAt,CreatedBy)
VALUES('admin','admin','shababarshadbutt@gmail.com',1234,'21232f297a57a5a743894a0e4a801fc3','03230437552',1,GETDATE(),'Shabab Butt');

INSERT INTO Role (Name,ShortName,Description,IsActive,CreatedAt,CreatedBy)
			VALUES('IsAdministrator','IsAdmin','Allow All Opreation',1,GETDATE(),'Shabab Butt'),
			('Is Allow Purchase','CanPurchase','Allow Enter Purchase ',1,GETDATE(),'Shabab Butt'),
			('Is Allow Sale','CanSale','Allow To Enter Sale',1,GETDATE(),'Shabab Butt'),
			('Is Allow Report','CanReport','Allow to See Reports',1,GETDATE(),'Shabab Butt'),
			('Is Alow To Edit','CanEdit','Allow To Edit',1,GETDATE(),'Shabab Butt');