BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "ProductType" (
	"Id"	INTEGER NOT NULL,
	"Name"	TEXT NOT NULL,
	"CreatedAt"	TEXT,
	"CreatedBy"	TEXT,
	"UpdatedAt"	TEXT,
	"UpdatedBy"	TEXT,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "ProductSubType" (
	"Id"	INTEGER NOT NULL,
	"ProductTypeId"	INTEGER NOT NULL,
	"Name"	TEXT,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	"CreatedAt"	TEXT,
	"CreatedBy"	TEXT,
	"UpdatedAt"	TEXT,
	"UpdatedBy"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "ProductColor" (
	"Id"	INTEGER NOT NULL,
	"Name"	TEXT,
	"Color"	TEXT NOT NULL,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	"CreatedAt"	TEXT,
	"CreatedBy"	TEXT,
	"UpdatedAt"	TEXT,
	"UpdatedBy"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "ProductSize" (
	"Id"	INTEGER NOT NULL,
	"Name"	TEXT,
	"Size"	TEXT NOT NULL,
	"IsActive"	INTEGER,
	"IsDeleted"	REAL,
	"CreatedAt"	TEXT,
	"CreatedBy"	TEXT,
	"UpdatedAt"	TEXT,
	"UpdatedBy"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "BussinessPartner" (
	"Id"	INTEGER NOT NULL,
	"Name"	TEXT,
	"BussinessName"	TEXT,
	"PhoneNumber"	TEXT,
	"MobileNumber"	TEXT,
	"City"	TEXT,
	"Address"	TEXT,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	"CreatedAt"	TEXT,
	"CreatedBy"	TEXT,
	"UpdatedAt"	TEXT,
	"UpdatedBy"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "Product" (
	"Id"	INTEGER NOT NULL,
	"Name"	TEXT NOT NULL,
	"ProductTypeId"	INTEGER NOT NULL,
	"ProductSubTypeId"	INTEGER NOT NULL,
	"ProductColorId"	INTEGER NOT NULL,
	"ProductSizeId"	INTEGER NOT NULL,
	"Image"	BLOB,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	"CreatedAt"	TEXT,
	"CreatedBy"	TEXT,
	"UpdatedAt"	TEXT,
	"UpdatedBy"	TEXT,
	PRIMARY KEY("Id")
);
CREATE TABLE IF NOT EXISTS "Invoice" (
	"Id"	INTEGER NOT NULL,
	"InvoiceId"	INTEGER NOT NULL,
	"InvoiceGuid"	INTEGER NOT NULL,
	"IsPurchaseInvoice"	INTEGER,
	"IsSaleInvoice"	INTEGER,
	"IsPurchaseReturnInvoice"	INTEGER,
	"IsSaleReturnInvoice"	INTEGER,
	"IsAmountRecived"	INTEGER,
	"IsAmountPaid"	INTEGER,
	"InvoiceType"	TEXT,
	"SelectedPartnerId"	INTEGER,
	"SelectedPaymentType"	TEXT,
	"PercentDiscount"	INTEGER,
	"DiscountAmount"	INTEGER,
	"Description"	TEXT,
	"AmountImage"	BLOB,
	"Payment"	INTEGER,
	"InvoiceTotal"	INTEGER,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	"CreatedAt"	INTEGER,
	"CreatedBy"	TEXT,
	"UpdatedAt"	INTEGER,
	"UpdatedBy"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "Inventory" (
	"Id"	INTEGER NOT NULL,
	"InvoiceId"	TEXT NOT NULL,
	"InvoiceGuid"	TEXT NOT NULL,
	"ProductId"	INTEGER,
	"ProductColorId"	INTEGER,
	"ProductSizeId"	INTEGER,
	"Price"	INTEGER,
	"Quantity"	NUMERIC,
	"IsStockIn"	INTEGER,
	"IsStockOut"	INTEGER,
	"StockInHand"	NUMERIC,
	"Total"	NUMERIC,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	"CreatedAt"	INTEGER,
	"CreatedBy"	TEXT,
	"UpdatedAt"	INTEGER,
	"UpdatedBy"	INTEGER,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "PartnerLedger" (
	"Id"	INTEGER NOT NULL,
	"PartnerId"	INTEGER NOT NULL,
	"InvoiceId"	INTEGER NOT NULL,
	"InvoiceGuid"	INTEGER NOT NULL,
	"IsBalancePayable"	INTEGER,
	"AmountReciveable"	NUMERIC,
	"AmountPayable"	NUMERIC,
	"BalanceAmount"	NUMERIC,
	"Description"	TEXT,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	"CreatedAt"	TEXT,
	"CreatedBy"	TEXT,
	"UpdatedAt"	TEXT,
	"UpdatedBy"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "IdentityUserModel" (
	"Id"	INTEGER NOT NULL,
	"FirstName"	TEXT NOT NULL,
	"LastName"	TEXT,
	"Email"	TEXT,
	"SecretKey"	TEXT,
	"PasswordHash"	TEXT,
	"ConcurrencyStamp"	TEXT,
	"PhoneNumber"	TEXT,
	"PhoneNumberConfirmed"	INTEGER,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	"CreatedAt"	INTEGER,
	"CreatedBy"	INTEGER,
	"UpdatedAt"	INTEGER,
	"UpdatedBy"	INTEGER,
	"Username"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "Role" (
	"Id"	INTEGER NOT NULL,
	"ShortName"	TEXT,
	"ConcurrencyStamp"	TEXT,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	"CreatedAt"	TEXT,
	"CreatedBy"	TEXT,
	"UpdatedAt"	TEXT,
	"UpdatedBy"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "UserRole" (
	"Id"	INTEGER NOT NULL,
	"UserId"	INTEGER,
	"RoleId"	INTEGER,
	"Description"	TEXT,
	"IsActive"	INTEGER,
	"IsDeleted"	INTEGER,
	"CreatedAt"	TEXT,
	"CreatedBy"	TEXT,
	"UpdatedAt"	TEXT,
	"UpdatedBy"	TEXT,
	PRIMARY KEY("Id" AUTOINCREMENT)
);
INSERT INTO "IdentityUserModel" VALUES (1,'Shabab','Butt','admin@admin.com','4598','21232f297a57a5a743894a0e4a801fc3','2021-06-23 15:26:56','03230437552',3230437752,1,0,'2021-06-23 15:26:56','Shabab Butt',NULL,NULL,NULL);
INSERT INTO "IdentityUserModel" VALUES (2,'admin','admin','admin@admin.com','4598','21232f297a57a5a743894a0e4a801fc3','2021-06-25 07:13:26','03230437552',3230437752,1,0,'2021-06-25 07:13:26','Shabab Butt',NULL,NULL,'admin');
INSERT INTO "Role" VALUES (1,'Product','2021-06-25 07:54:18',1,0,'2021-06-25 07:54:18','admin',NULL,NULL);
INSERT INTO "Role" VALUES (2,'Purchase','2021-06-25 07:54:18',1,0,'2021-06-25 07:54:18','admin',NULL,NULL);
INSERT INTO "Role" VALUES (3,'Sale','2021-06-25 07:54:18',1,0,'2021-06-25 07:54:18','admin',NULL,NULL);
INSERT INTO "Role" VALUES (4,'Payment','2021-06-25 07:54:18',1,0,'2021-06-25 07:54:18','admin',NULL,NULL);
INSERT INTO "Role" VALUES (5,'Reports','2021-06-25 07:54:18',1,0,'2021-06-25 07:54:18','admin',NULL,NULL);
INSERT INTO "UserRole" VALUES (1,2,1,'Product CRUD Allowed',1,0,'2021-06-25 08:00:15','admin',NULL,NULL);
INSERT INTO "UserRole" VALUES (2,2,2,'Purchase CRUD Allowed',1,0,'2021-06-25 08:00:15','admin',NULL,NULL);
INSERT INTO "UserRole" VALUES (3,2,3,'Sales CRUD Allowed',1,0,'2021-06-25 08:00:15','admin',NULL,NULL);
INSERT INTO "UserRole" VALUES (4,2,4,'Payment CRUD Allowed',1,0,'2021-06-25 08:00:15','admin',NULL,NULL);
INSERT INTO "UserRole" VALUES (5,2,5,'Reports Allowed',1,0,'2021-06-25 08:00:15','admin',NULL,NULL);
COMMIT;
