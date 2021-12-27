INSERT INTO PaymentType (Name,PaymentType,Description,IsActive,CreatedAt,CreatedBy)
VALUES('Cash','Cash','Cash payment',1,GETDATE(),'admin'),
	  ('Credit','Credit','Credit Payment',1,GETDATE(),'admin'),
	  ('Jazz Cash','Jazz Cash','Transfer Through Jazz Cash',1,GETDATE(),'admin'),
	  ('UBL Omni','UBL Omni','Transfer through UBL Omni',1,GETDATE(),'admin'),
	  ('Easy paisa','Easy paisa','Transfer through Telenor Easy paisa',1,GETDATE(),'admin'),
	  ('Credit','Credit','Credit Payment',1,GETDATE(),'admin'),
	  ('Bank','Bank','Transfer through Bank Or Check',1,GETDATE(),'admin'),
	  ('Partial','Partial','Partial Or Mix Mode Payment',1,GETDATE(),'admin');