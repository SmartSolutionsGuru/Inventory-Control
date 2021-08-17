USE [master]
GO

/****** Object:  Database [SmartSolutions.InventoryControl]    Script Date: 8/12/2021 11:07:38 AM ******/
CREATE DATABASE [SmartSolutions.InventoryControl]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SmartSolutions.InventoryControl', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SmartSolutions.InventoryControl.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'SmartSolutions.InventoryControl_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\SmartSolutions.InventoryControl_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
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

ALTER DATABASE [SmartSolutions.InventoryControl] SET QUERY_STORE = OFF
GO

ALTER DATABASE [SmartSolutions.InventoryControl] SET  READ_WRITE 
GO

