-- Trịnh Quang Nghĩa - 1612422 - HCMUS
IF DB_Id('Management System') IS NULL CREATE DATABASE [Management System]
GO
USE [Management System]
GO
/****** Object:  Table [dbo].[Bill]    Script Date: 1/16/2019 5:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Bill](
	[Date] [datetime] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Phone] [varchar](13) NULL,
	[ProductId] [nvarchar](15) NOT NULL,
	[Number] [int] NOT NULL,
	[NumberGiven] [int] NOT NULL,
	[OriginalPrice] [bigint] NOT NULL,
	[FinalPrice] [bigint] NOT NULL,
	[Event] [nvarchar](max) NULL,
	[GoToShop] [bit] NOT NULL,
	[MoneyTaken] [bigint] NULL,
	[MoneyExchange] [bigint] NULL,
	[Address] [nvarchar](max) NULL,
	[Deposit] [bigint] NULL,
	[Ship] [bigint] NULL,
	[MoneyWillGet] [bigint] NULL,
	[Status] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_Bill] PRIMARY KEY CLUSTERED 
(
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Event]    Script Date: 1/16/2019 5:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Event](
	[Name] [nvarchar](50) NOT NULL,
	[DateBegin] [datetime] NOT NULL,
	[DateEnd] [datetime] NOT NULL,
	[Sale] [int] NULL,
	[BuyGet_Buy] [int] NULL,
	[BuyGet_Get] [int] NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Product]    Script Date: 1/16/2019 5:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Name] [nvarchar](50) NOT NULL,
	[Id] [nvarchar](15) NOT NULL,
	[Price] [bigint] NOT NULL,
	[Date] [datetime] NOT NULL,
	[InitialAmount] [int] NOT NULL,
	[CurrentAmount] [int] NOT NULL,
	[Capital] [bigint] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[ProductType] [nvarchar](15) NOT NULL,
	[ImagePath] [nvarchar](max) NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductType]    Script Date: 1/16/2019 5:45:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductType](
	[Name] [nvarchar](50) NOT NULL,
	[Id] [nvarchar](15) NOT NULL,
	[NumOfProduct] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_ProductType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2018-12-20 08:29:04.657' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'DOM-D010', 1, 0, 550000, 412500, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4587500, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2018-12-20 08:29:54.547' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'NB-V402', 1, 0, 590000, 442500, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4557500, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2018-12-28 08:31:19.767' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'DT001', 2, 1, 500000, 375000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4625000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2018-12-29 08:31:26.400' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'DT001', 1, 0, 250000, 187500, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4812500, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-01 08:28:28.993' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'CS001T', 3, 1, 4800000, 3600000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 1400000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-01 08:32:16.093' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'BA1819VV', 2, 1, 1800000, 1350000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 3650000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-04 08:31:40.757' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'OC002', 1, 0, 1000000, 750000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4250000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-05 08:31:46.940' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'OC002', 2, 1, 2000000, 1500000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 3500000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-06 08:31:38.080' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'OC002', 1, 0, 1000000, 750000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4250000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-07 08:29:20.873' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'BA1819VV', 2, 1, 1800000, 1350000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 3650000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-08 08:12:27.590' AS DateTime), N'Trịnh Quang Trung', N'0347630361', N'BS003D', 1, 0, 380000, 285000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 0, 0, 0, N'361/26 Phan Huy Ích, P14, Q.Gò vấp', 30, 0, 284970, N'Chưa hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-09 08:10:46.750' AS DateTime), N'Trịnh Quang Nghĩa', N'0376770792', N'BS003D', 2, 1, 760000, 570000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 600000, 30000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-10 08:28:51.457' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'KK2D', 1, 0, 800000, 600000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4400000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-10 08:30:18.817' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'TE001', 4, 2, 1000000, 750000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4250000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-11 08:13:23.347' AS DateTime), N'Trịnh Quang Trung', N'0347630361', N'BJD-3015GD', 3, 1, 4500000, 3375000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 4000000, 625000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-11 08:30:27.660' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'AE005V', 2, 1, 4000000, 3000000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 2000000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-12 08:29:09.333' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'DOM-D010', 2, 1, 1100000, 825000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4175000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-13 08:28:42.670' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'KK2D', 1, 0, 800000, 600000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4400000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-14 08:30:04.280' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'NB-V402', 1, 0, 590000, 442500, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4557500, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-15 08:29:24.160' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'BA1819VV', 2, 1, 1800000, 1350000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 3650000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-16 08:12:58.417' AS DateTime), N'Trịnh Quang Trung', N'0347630361', N'SM001', 2, 1, 700000, 525000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 0, 0, 0, N'361/26 Phan Huy Ích, P14, Q.Gò vấp', 30000, 60000, 555000, N'Chưa hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-16 08:28:46.853' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'KK2D', 1, 0, 800000, 600000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4400000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-16 08:29:33.110' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'AI010', 2, 1, 3600000, 2700000, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 2300000, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-16 08:29:59.367' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'NB-V402', 1, 0, 590000, 442500, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4557500, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-16 08:30:12.653' AS DateTime), N'Trịnh Quang Nghĩa', N'037677792', N'TE001', 1, 0, 250000, 187500, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 1, 5000000, 4812500, NULL, 0, 0, 0, N'Đã hoàn thành')
INSERT [dbo].[Bill] ([Date], [Name], [Phone], [ProductId], [Number], [NumberGiven], [OriginalPrice], [FinalPrice], [Event], [GoToShop], [MoneyTaken], [MoneyExchange], [Address], [Deposit], [Ship], [MoneyWillGet], [Status]) VALUES (CAST(N'2019-01-16 09:08:16.937' AS DateTime), N'Đặng Hoài Nam', NULL, N'DOM-D010', 1, 0, 550000, 412500, N'Black Friday - Giảm 25% - Mua 2 tặng 1', 0, 0, 0, N'KTX Khu A', 0, 60000, 472500, N'Chưa hoàn thành')
INSERT [dbo].[Event] ([Name], [DateBegin], [DateEnd], [Sale], [BuyGet_Buy], [BuyGet_Get]) VALUES (N'Black Friday', CAST(N'2019-01-11 00:00:00.000' AS DateTime), CAST(N'2019-01-18 00:00:00.000' AS DateTime), 25, 2, 1)
INSERT [dbo].[Event] ([Name], [DateBegin], [DateEnd], [Sale], [BuyGet_Buy], [BuyGet_Get]) VALUES (N'Cyber Monday', CAST(N'2019-01-14 00:00:00.000' AS DateTime), CAST(N'2019-01-16 00:00:00.000' AS DateTime), 25, 0, 0)
INSERT [dbo].[Event] ([Name], [DateBegin], [DateEnd], [Sale], [BuyGet_Buy], [BuyGet_Get]) VALUES (N'Tết tưng bừng', CAST(N'2019-01-28 00:00:00.000' AS DateTime), CAST(N'2019-02-28 00:00:00.000' AS DateTime), 50, 3, 2)
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ cơ AESOP Vàng', N'AE005V', 2000000, CAST(N'2019-01-01 00:00:00.000' AS DateTime), 3, 0, 1500000, N'Đầy đủ lịch thứ, ngày, tháng, năm', N'DH003', N'D:\Management System\Data\AE005V.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ cơ AILANG AI010', N'AI010', 1800000, CAST(N'2019-01-15 00:00:00.000' AS DateTime), 3, 0, 2000000, N'Automatic (tự động)', N'DH003', N'D:\Management System\Data\AI010.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ BAILISHI 1819VV', N'BA1819VV', 900000, CAST(N'2019-01-08 00:00:00.000' AS DateTime), 10, 1, 5000000, N'Kính khoáng, chống nước 3ATM', N'DH002', N'D:\Management System\Data\BA1819VV.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ BRIGADA 2018', N'BJD-3015GD', 1500000, CAST(N'2018-12-27 00:00:00.000' AS DateTime), 5, 1, 6000000, N'Kính khoáng, chống nước 3ATM, có lịch thứ ngày', N'DH001', N'D:\Management System\Data\BJD-3015GD.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ BOSCK 003', N'BS003D', 380000, CAST(N'2018-12-10 00:00:00.000' AS DateTime), 5, 1, 1000000, N'Kính khoáng, chống nước 3ATM', N'DH001', N'D:\Management System\Data\BS003D.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ CASIMA CS001T', N'CS001T', 1600000, CAST(N'2018-12-19 00:00:00.000' AS DateTime), 5, 1, 3000000, NULL, N'DH001', N'D:\Management System\Data\CS001T.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ Nam DOM D010', N'DOM-D010', 550000, CAST(N'2019-01-04 00:00:00.000' AS DateTime), 5, 0, 2500000, N'Kính khoáng, chống nước 3ATM', N'DH001', N'D:\Management System\Data\DOM-D010.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ điện tử cao su', N'DT001', 250000, CAST(N'2019-01-05 00:00:00.000' AS DateTime), 15, 11, 1000000, N'Màu đen sang trọng', N'DH005', N'D:\Management System\Data\DT001.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ Kim Jeder 002', N'KJ002', 800000, CAST(N'2018-12-12 00:00:00.000' AS DateTime), 10, 10, 1500000, N'Kính khoáng, chống nước 3ATM', N'DH001', N'D:\Management System\Data\JE002.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ DOM KK2D', N'KK2D', 800000, CAST(N'2018-12-25 00:00:00.000' AS DateTime), 10, 7, 3000000, N'Kính khoáng, chống nước 3ATM', N'DH001', N'D:\Management System\Data\KK2D.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ NIBOSI VITO 402', N'NB-V402', 590000, CAST(N'2019-02-12 00:00:00.000' AS DateTime), 5, 2, 2000000, N'Kính khoáng, chống nước 3ATM', N'DH002', N'D:\Management System\Data\NB-V402.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ cơ OCHSTIN', N'OC002', 1000000, CAST(N'2018-12-25 00:00:00.000' AS DateTime), 5, 0, 3000000, N'Đường kính 42mm - mặt kính cứng, chống trầy xước tương đối tốt', N'DH003', N'D:\Management System\Data\OC002.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ thông minh cao su', N'SM001', 350000, CAST(N'2019-01-05 00:00:00.000' AS DateTime), 15, 12, 2000000, N'Màn hình cảm ứng', N'DH006', N'D:\Management System\Data\SM001.jpg')
INSERT [dbo].[Product] ([Name], [Id], [Price], [Date], [InitialAmount], [CurrentAmount], [Capital], [Description], [ProductType], [ImagePath]) VALUES (N'Đồng hồ Minion 2018', N'TE001', 250000, CAST(N'2019-01-18 00:00:00.000' AS DateTime), 10, 3, 500000, N'Hình dáng Minion ngộ nghĩnh', N'DH004', N'D:\Management System\Data\TE001.jpg')
INSERT [dbo].[ProductType] ([Name], [Id], [NumOfProduct], [Date], [Description]) VALUES (N'Đồng hồ dây da', N'DH001', 6, CAST(N'2019-01-13 14:17:32.000' AS DateTime), N'Dây da nhân tạo. Mềm, bền và chịu lực tốt')
INSERT [dbo].[ProductType] ([Name], [Id], [NumOfProduct], [Date], [Description]) VALUES (N'Đồng hồ dây kim loại', N'DH002', 2, CAST(N'2019-01-13 14:18:00.000' AS DateTime), N'Dây làm bằng hợp kim thép không gỉ, chống nước tốt')
INSERT [dbo].[ProductType] ([Name], [Id], [NumOfProduct], [Date], [Description]) VALUES (N'Đồng hồ cơ tự động', N'DH003', 3, CAST(N'2019-01-13 14:18:37.000' AS DateTime), N'Không cần pin, chạy tự động')
INSERT [dbo].[ProductType] ([Name], [Id], [NumOfProduct], [Date], [Description]) VALUES (N'Đồng hồ trẻ em', N'DH004', 1, CAST(N'2019-01-14 13:43:04.787' AS DateTime), N'Dành cho trẻ em, màu sắc ngộ nghĩnh')
INSERT [dbo].[ProductType] ([Name], [Id], [NumOfProduct], [Date], [Description]) VALUES (N'Đồng hồ điện tử', N'DH005', 1, CAST(N'2019-01-15 16:44:00.387' AS DateTime), N'Đồng hồ hiển thị thời gian bằng số')
INSERT [dbo].[ProductType] ([Name], [Id], [NumOfProduct], [Date], [Description]) VALUES (N'Đồng hồ thông minh', N'DH006', 1, CAST(N'2019-01-15 17:41:09.013' AS DateTime), N'Đồng hồ màn hình cảm ứng, có báo thức, nhắc nhở')
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductType] FOREIGN KEY([ProductType])
REFERENCES [dbo].[ProductType] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductType]
GO
