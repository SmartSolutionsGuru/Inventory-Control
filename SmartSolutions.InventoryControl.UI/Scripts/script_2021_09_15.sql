USE [master]
GO
/****** Object:  Database [SmartSolutions.InventoryControl]    Script Date: 9/14/2021 5:43:18 PM ******/
CREATE DATABASE [SmartSolutions.InventoryControl]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SmartSolutions.InventoryControl', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SmartSolutions.InventoryControl.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SmartSolutions.InventoryControl_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SmartSolutions.InventoryControl_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SmartSolutions.InventoryControl].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET ARITHABORT OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET RECOVERY FULL 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET  MULTI_USER 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'SmartSolutions.InventoryControl', N'ON'
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET QUERY_STORE = OFF
GO
USE [SmartSolutions.InventoryControl]
GO
/****** Object:  UserDefinedFunction [dbo].[SplitString]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitString]
(
@Input NVARCHAR(MAX),
@Character CHAR(1)
)
RETURNS @Output TABLE (
Item NVARCHAR(1000)
)
AS
BEGIN
DECLARE @StartIndex INT, @EndIndex INT

SET @StartIndex = 1
IF SUBSTRING(@Input, LEN(@Input) - 1, LEN(@Input)) <> @Character
BEGIN
SET @Input = @Input + @Character
END

WHILE CHARINDEX(@Character, @Input) > 0
BEGIN
SET @EndIndex = CHARINDEX(@Character, @Input)

INSERT INTO @Output(Item)
SELECT SUBSTRING(@Input, @StartIndex, @EndIndex - 1)

SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input))
END

RETURN
END
GO
/****** Object:  Table [dbo].[Bank]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bank](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Bank] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BankAccount]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BranchId] [int] NOT NULL,
	[AccountType] [varchar](50) NULL,
	[AccountStatus] [varchar](50) NULL,
	[OpeningDate] [datetime2](7) NULL,
	[AccountNumber] [varchar](150) NOT NULL,
	[OpeningBalance] [decimal](18, 0) NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_BankAccount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BankBranch]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankBranch](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[BankId] [int] NOT NULL,
	[Address] [varchar](150) NULL,
	[BranchDetail] [varchar](150) NULL,
	[BussinessPhone] [varchar](50) NULL,
	[BussinessPhone1] [varchar](50) NULL,
	[MobilePhone] [varchar](50) NULL,
	[MobilePhone1] [varchar](50) NULL,
	[Email] [varchar](100) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[Description] [varchar](150) NULL,
 CONSTRAINT [PK_BankBranch] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BussinessPartner]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BussinessPartner](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerTypeId] [int] NOT NULL,
	[PartnerCategoryId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[BussinessName] [varchar](50) NULL,
	[PhoneNumber] [varchar](50) NULL,
	[MobileNumber] [varchar](50) NOT NULL,
	[CityId] [int] NOT NULL,
	[Address] [varchar](500) NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_BussinessPartner] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChartOfAccount]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChartOfAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountCategory] [varchar](100) NULL,
	[AccountSubCategory] [varchar](100) NULL,
	[AccountHeading] [varchar](100) NULL,
	[AccountNumber] [varchar](50) NULL,
	[Description] [varchar](100) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_ChartOfAccount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[City]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[City](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ProvinceId] [int] NOT NULL,
	[PhoneCode] [int] NOT NULL,
	[Description] [varchar](250) NULL,
	[IsActive] [tinyint] NOT NULL,
	[IsDeleted] [tinyint] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[CountryId] [int] NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClosingStock]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClosingStock](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[WarehouseId] [int] NULL,
	[Quantity] [int] NULL,
	[Price] [decimal](18, 0) NULL,
	[Total] [decimal](18, 0) NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_ClosingStock] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Country]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Country](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Iso] [varchar](50) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[NiceName] [varchar](100) NULL,
	[Iso3] [varchar](50) NULL,
	[NumCode] [int] NULL,
	[PhoneCode] [int] NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [tinyint] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FinancialTransaction]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FinancialTransaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransactionId] [varchar](100) NOT NULL,
	[TransactionGuid] [varchar](100) NOT NULL,
	[PartnerLedgerAccountId] [int] NOT NULL,
	[PartnerId] [int] NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[InvoiceUniqueId] [varchar](100) NOT NULL,
	[InvoiceGuid] [varchar](100) NOT NULL,
	[Amount] [decimal](18, 0) NOT NULL,
	[IsAmountReceived] [tinyint] NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_FinancialTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IdentityUser]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IdentityUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[FullName] [varchar](50) NULL,
	[Email] [varchar](150) NULL,
	[SecretKey] [varchar](250) NOT NULL,
	[Password] [varchar](250) NOT NULL,
	[MobileNumber] [varchar](50) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_IdentityUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LedgerEntries]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LedgerEntries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransactionId] [int] NOT NULL,
	[PartnerId] [int] NOT NULL,
	[PartnerAccountId] [int] NOT NULL,
	[Amount] [decimal](18, 0) NOT NULL,
	[EntryType] [varchar](50) NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_LedgerEntries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OpeningStock]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OpeningStock](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[WarehouseId] [int] NULL,
	[Quantity] [int] NULL,
	[Price] [decimal](18, 0) NULL,
	[Total] [decimal](18, 0) NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_OpeningStock] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PartnerCategory]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PartnerCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](100) NULL,
	[IsActive] [tinyint] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdateBy] [varchar](50) NULL,
 CONSTRAINT [PK_PartnerCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PartnerLedgerAccounts]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PartnerLedgerAccounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[PaymentId] [int] NULL,
	[CurrentBalance] [decimal](18, 0) NULL,
	[CurrentBalanceType] [varchar](50) NULL,
	[Description] [varchar](max) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [nchar](10) NULL,
	[DR] [decimal](18, 0) NULL,
	[CR] [decimal](18, 0) NULL,
	[InvoiceId] [int] NULL,
	[TransactionId] [int] NULL,
	[PartnerSerupAccount] [varchar](50) NULL,
 CONSTRAINT [PK_PartnerLedgerAccounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PartnerSetupAccount]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PartnerSetupAccount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerAccountCode] [varchar](100) NOT NULL,
	[PartnerAccountTypeId] [int] NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[PartnerId] [int] NULL,
 CONSTRAINT [PK_PartnerSetupAccount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PartnerType]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PartnerType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](100) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_PurchaseType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[PaymentRefrencePartnerId] [int] NULL,
	[PaymentAmount] [decimal](18, 0) NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[PaymentMethodId] [int] NULL,
	[Description] [varchar](150) NULL,
	[DR] [varchar](20) NULL,
	[CR] [varchar](20) NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentType]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[PaymentType] [varchar](100) NOT NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_PaymentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductTypeId] [int] NOT NULL,
	[ProductSubTypeId] [int] NOT NULL,
	[ProductColorId] [int] NULL,
	[ProductSizeId] [int] NULL,
	[ProductUnitId] [int] NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[Name] [varchar](50) NULL,
	[ImagePath] [varchar](max) NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductColor]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductColor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Color] [varchar](50) NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_ProductColor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductSize]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductSize](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[Size] [varchar](50) NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_ProductSize] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductSubType]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductSubType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ProductTypeId] [int] NOT NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedBy] [datetime2](7) NULL,
	[UpdatedAt] [varchar](50) NULL,
 CONSTRAINT [PK_ProductSubType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductType]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_ProductType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductUnit]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductUnit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_ProductUnit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProprietorInfo]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProprietorInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProprietorName] [varchar](50) NULL,
	[BussinessName] [varchar](100) NULL,
	[BussinessTypeId] [int] NULL,
	[BussinessCategoryId] [int] NULL,
	[CityId] [int] NULL,
	[LandLineNumber] [varchar](50) NULL,
	[LandLineNumber1] [varchar](50) NULL,
	[MobileNumber] [varchar](50) NULL,
	[MobileNumber1] [varchar](50) NULL,
	[WhatsAppNumber] [varchar](50) NULL,
	[BussinessAddress] [varchar](100) NULL,
	[HomeAddress] [varchar](100) NULL,
	[Description] [varchar](250) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_ProprietorInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Province]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Province](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[CountryId] [int] NOT NULL,
	[Description] [nvarchar](250) NULL,
	[IsActive] [tinyint] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Province] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseInvoice]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseInvoice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [varchar](100) NOT NULL,
	[InvoiceGuid] [varchar](50) NOT NULL,
	[PaymentTypeId] [int] NULL,
	[PartnerId] [int] NOT NULL,
	[Discount] [decimal](18, 0) NULL,
	[PaymentImage] [image] NULL,
	[Description] [varchar](150) NULL,
	[PaymentId] [int] NULL,
	[InvoiceTotal] [decimal](18, 0) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_PurchaseInvoice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseOrderDetails]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PurchaseOrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Description] [varchar](150) NULL,
	[Price] [decimal](18, 0) NULL,
	[Quantity] [int] NULL,
	[Discount] [decimal](18, 0) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[Total] [decimal](18, 0) NULL,
	[WarehouseId] [int] NULL,
 CONSTRAINT [PK_PurchaseOrderDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseOrderMaster]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderMaster](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[Status] [varchar](50) NULL,
	[Description] [varchar](150) NULL,
	[ShippingId] [int] NULL,
	[SubTotal] [decimal](18, 0) NULL,
	[Discount] [decimal](18, 0) NULL,
	[GrandTotal] [decimal](18, 0) NULL,
	[IsActive] [tinyint] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_PurchaseOrderMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchasePaymentVoucher]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchasePaymentVoucher](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[PurchaseOrderId] [int] NOT NULL,
	[PaymentMode] [varchar](50) NULL,
	[Amount] [decimal](18, 0) NOT NULL,
	[RefrencePartnerId] [int] NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [datetime2](7) NULL,
 CONSTRAINT [PK_PurchasePaymentVoucher] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PurchaseReturnInvoice]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseReturnInvoice](
	[Id] [int] NOT NULL,
	[PurchaseReturnId] [varchar](50) NOT NULL,
	[PurchaseReturnGuid] [varchar](100) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[Description] [varchar](150) NULL,
	[Total] [decimal](18, 0) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[PurchaseInvoiceId] [int] NULL,
 CONSTRAINT [PK_PurchaseReturnInvoice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ShortName] [varchar](50) NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[Updatedat] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SaleInvoice]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaleInvoice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [varchar](50) NOT NULL,
	[InvoiceGuid] [varchar](50) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[PaymentTypeId] [int] NULL,
	[Discount] [decimal](18, 0) NULL,
	[Payment] [decimal](18, 0) NULL,
	[PaymentImage] [image] NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[InvoiceTotal] [decimal](18, 0) NULL,
 CONSTRAINT [PK_SaleInvoice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SaleOrderDetail]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaleOrderDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SaleOrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Description] [varchar](150) NULL,
	[Price] [decimal](18, 0) NULL,
	[Quantity] [int] NULL,
	[Discount] [decimal](18, 0) NULL,
	[Total] [decimal](18, 0) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_SaleOrderDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SaleOrderMaster]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaleOrderMaster](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[Status] [varchar](50) NULL,
	[Description] [varchar](510) NULL,
	[ShippingId] [int] NULL,
	[SubTotal] [decimal](18, 0) NULL,
	[Discount] [decimal](18, 0) NULL,
	[GrandTotal] [decimal](18, 0) NULL,
	[IsActive] [tinyint] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_SaleOrderMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SalePaymentVoucher]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalePaymentVoucher](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[SaleOrderId] [int] NOT NULL,
	[PaymentMode] [varchar](50) NULL,
	[Amount] [decimal](18, 0) NOT NULL,
	[RefrencePartnerId] [int] NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_SalePaymentVoucher] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SaleReturnInvoice]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaleReturnInvoice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SaleReturnId] [varchar](150) NOT NULL,
	[SaleReturnGuid] [varchar](150) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[Description] [varchar](250) NULL,
	[Total] [decimal](18, 0) NOT NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_SaleReturnInvoice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stock]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stock](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[StockInId] [int] NULL,
	[StockInQuantity] [int] NULL,
	[StockOutId] [int] NULL,
	[StockOutQuantity] [int] NULL,
	[StockBalance] [int] NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Stock] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockIn]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockIn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[PurchaseOrderDetailId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[PurchaseOrderId] [int] NOT NULL,
	[Price] [decimal](18, 0) NULL,
	[Description] [varchar](max) NULL,
	[IsActive] [tinyint] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[WarehouseId] [int] NULL,
	[Total] [int] NULL,
	[PurchaseInvoiceId] [int] NULL,
 CONSTRAINT [PK_StockIn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockIssue]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockIssue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[PartnerId] [int] NOT NULL,
	[PurchaseOrderId] [int] NOT NULL,
	[PurchaseOrderDetailId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Description] [varchar](150) NULL,
	[CityId] [int] NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Warehouse] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockOut]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockOut](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[SaleOrderId] [int] NOT NULL,
	[SaleOrderDetailId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](18, 0) NOT NULL,
	[Description] [varchar](150) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[WarehouseId] [int] NULL,
	[Total] [int] NULL,
 CONSTRAINT [PK_StockOut] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemSettings]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NULL,
	[SettingKey] [varchar](150) NOT NULL,
	[SettingValue] [int] NOT NULL,
	[DefaultValue] [tinyint] NULL,
	[Description] [varchar](150) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
	[Value] [varchar](500) NULL,
 CONSTRAINT [PK_SystemSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Warehouses]    Script Date: 9/14/2021 5:43:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Warehouses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[CityId] [int] NOT NULL,
	[PhoneNumber] [varchar](50) NULL,
	[MobileNumber] [varchar](50) NULL,
	[Address] [varchar](150) NULL,
	[Description] [varchar](50) NULL,
	[IsActive] [tinyint] NOT NULL,
	[CreatedAt] [datetime2](7) NULL,
	[CreatedBy] [varchar](50) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[UpdatedBy] [varchar](50) NULL,
 CONSTRAINT [PK_Warehouses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductColor] FOREIGN KEY([ProductColorId])
REFERENCES [dbo].[ProductColor] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductColor]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductSize] FOREIGN KEY([ProductSizeId])
REFERENCES [dbo].[ProductSize] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductSize]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductSubType] FOREIGN KEY([ProductSubTypeId])
REFERENCES [dbo].[ProductSubType] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductSubType]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductType] FOREIGN KEY([ProductTypeId])
REFERENCES [dbo].[ProductType] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductType]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductUnit] FOREIGN KEY([ProductUnitId])
REFERENCES [dbo].[ProductUnit] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductUnit]
GO
ALTER TABLE [dbo].[ProductSubType]  WITH CHECK ADD  CONSTRAINT [FK_ProductSubType_PartnerType] FOREIGN KEY([ProductTypeId])
REFERENCES [dbo].[PartnerType] ([Id])
GO
ALTER TABLE [dbo].[ProductSubType] CHECK CONSTRAINT [FK_ProductSubType_PartnerType]
GO
ALTER TABLE [dbo].[Province]  WITH CHECK ADD  CONSTRAINT [FK_Province_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([Id])
GO
ALTER TABLE [dbo].[Province] CHECK CONSTRAINT [FK_Province_Country]
GO
/****** Object:  StoredProcedure [dbo].[spGetAllProductWithColorAndSize]    Script Date: 9/14/2021 5:43:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[spGetAllProductWithColorAndSize]
 @v_productName AS varchar
AS
BEGIN
 SELECT P.Id,P.IsActive,P.ProductTypeId,P.ProductSubTypeId,P.ImagePath, P.Name ,Ps.Size,Pc.Color FROM product P
 Inner Join  ProductSize Ps ON P.ProductSizeId = Ps.Id
 Inner Join ProductColor Pc ON P.ProductColorId = Pc.Id
 WHERE P.Name  LIKE @v_productName  + '%' AND P.IsActive = 1
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'DR or CR' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'PartnerLedgerAccounts'
GO
USE [master]
GO
ALTER DATABASE [SmartSolutions.InventoryControl] SET  READ_WRITE 
GO
