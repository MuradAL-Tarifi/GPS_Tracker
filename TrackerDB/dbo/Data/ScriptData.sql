USE [TrackerDB]
GO
SET IDENTITY_INSERT [dbo].[Agent] ON 
GO
INSERT [dbo].[Agent] ([Id], [Name], [NameEn], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'GPS Tracker', N'GPS Tracker', 0, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Agent] OFF
GO
SET IDENTITY_INSERT [dbo].[Fleet] ON 
GO
INSERT [dbo].[Fleet] ([Id], [AgentId], [Name], [NameEn], [ManagerEmail], [ManagerMobile], [SupervisorEmail], [SupervisorMobile], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [TaxRegistrationNumber], [CommercialRegistrationNumber]) VALUES (402, 2, N'Microsoft', N'Microsoft Fleet', N'user@maill.com', N'96555645', N'user@maill.com', N'96644524', 0, NULL, NULL, CAST(N'2020-08-25T09:46:38.4633333' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', NULL, NULL)
GO
INSERT [dbo].[Fleet] ([Id], [AgentId], [Name], [NameEn], [ManagerEmail], [ManagerMobile], [SupervisorEmail], [SupervisorMobile], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [TaxRegistrationNumber], [CommercialRegistrationNumber]) VALUES (403, 2, N'شركة المنار', N'Al Manar', N'user@maill.com', N'96555645', N'user@maill.com', N'96644524', 0, NULL, NULL, CAST(N'2020-07-15T13:14:33.4933333' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Fleet] OFF
GO
SET IDENTITY_INSERT [dbo].[Group] ON 
GO
INSERT [dbo].[Group] ([Id], [AgentId], [FleetId], [Name], [NameEn], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, 2, 403, N'مجموعة الرياض', N'Group Riyadh', 0, CAST(N'2021-12-01T00:00:00.0000000' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', CAST(N'2021-12-01T00:00:00.0000000' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc')
GO
SET IDENTITY_INSERT [dbo].[Group] OFF
GO
INSERT [dbo].[User] ([Id], [UserName], [Password], [Name], [NameEn], [Email], [IsActive], [ExpirationDate], [IsAdmin], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AgentId], [FleetId], [GroupId], [AccountId], [AppId], [EnableMobileAlerts]) VALUES (N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', N'RWA19393', N'rwa@123', N'Admin', N'Admin', N'admin@gmail.com', 1, NULL, 1, 0, CAST(N'2021-12-01T00:00:00.0000000' AS DateTime2), NULL, CAST(N'2021-12-01T00:00:00.0000000' AS DateTime2), NULL, 2, 402, NULL, NULL, NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[Gateway] ON 
GO
INSERT [dbo].[Gateway] ([Id], [Name], [IMEI], [SIMNumber], [ExpirationDate], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Gateway - المنار ', 864376045064366, N'0', CAST(N'2021-11-08T00:00:00.0000000' AS DateTime2), 1, 0, CAST(N'2020-11-03T16:24:37.9500000' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', CAST(N'2020-11-16T09:58:06.2766667' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc')
GO
SET IDENTITY_INSERT [dbo].[Gateway] OFF
GO
SET IDENTITY_INSERT [dbo].[Warehouse] ON 
GO
INSERT [dbo].[Warehouse] ([Id], [FleetId], [GroupId], [Name], [Phone], [Address], [City], [Latitude], [Longitude], [LandCoordinates], [LandAreaInSquareMeter], [LicenseNumber], [LicenseIssueDate], [LicenseExpiryDate], [ManagerMobile], [Email], [IsActive], [ReferenceKey], [WaslActivityType], [IsLinkedWithWasl], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, 403,4, N'شركة المنار', N'+966564488481', N'الرياض - العليا', N'الرياض', CAST(24.66 AS Decimal(18, 2)), CAST(46.84 AS Decimal(18, 2)), N' [
        {
            "x": 46.838747,
            "y": 24.658830
        },
        {
            "x": 46.839000,
            "y": 24.658859
        },
        {
            "x": 46.839006,
            "y": 24.658598
        },
        {
            "x": 46.838767,
            "y": 24.658601
        }
    ]', 600, N'AFE000118', N'2018-12-09', N'2022-12-09', N'+966564488481', N'almnar@gmail.com', 1, NULL, N'1', 0, 0, CAST(N'2020-11-14T00:00:00.0000000' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', CAST(N'2020-11-14T00:00:00.0000000' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc')
GO
SET IDENTITY_INSERT [dbo].[Warehouse] OFF
GO
SET IDENTITY_INSERT [dbo].[Inventory] ON 
GO
INSERT [dbo].[Inventory] ([Id], [WarehouseId], [GatewayId], [Name], [InventoryNumber], [IsActive], [WaslActivityType], [SFDAStoringCategory], [IsLinkedWithWasl], [ReferenceKey], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, 4, 1, N'Vaccine Room', N'1', 1, NULL, N'SCM2', 0, N'', 0, CAST(N'2020-11-14T17:28:09.8533333' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', CAST(N'2021-05-22T16:33:20.9900000' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc')
GO
SET IDENTITY_INSERT [dbo].[Inventory] OFF
GO
SET IDENTITY_INSERT [dbo].[InventorySensor] ON 
GO
INSERT [dbo].[InventorySensor] ([Id], [InventoryId], [Serial], [Name], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, 1, 72200471, N'حساس 1', 1, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', NULL, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc')
GO
INSERT [dbo].[InventorySensor] ([Id], [InventoryId], [Serial], [Name], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, 1, 72200437, N'حساس 4', 1, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', NULL, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc')
GO
INSERT [dbo].[InventorySensor] ([Id], [InventoryId], [Serial], [Name], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, 1, 72200445, N'حساس 2', 0, CAST(N'2021-02-21T11:26:59.8200000' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', NULL, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc')
GO
INSERT [dbo].[InventorySensor] ([Id], [InventoryId], [Serial], [Name], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, 1, 72200454, N'حساس 3', 0, CAST(N'2021-02-21T11:26:59.9200000' AS DateTime2), N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', NULL, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc')
GO
SET IDENTITY_INSERT [dbo].[InventorySensor] OFF
GO
SET IDENTITY_INSERT [dbo].[RegisterType] ON 
GO
INSERT [dbo].[RegisterType] ([Id], [Name], [NameEn], [IsDeleted]) VALUES (1, N'مرتبط بهيئة النقل', N'Linked with Transport Authority', 0)
GO
INSERT [dbo].[RegisterType] ([Id], [Name], [NameEn], [IsDeleted]) VALUES (3, N'مرتبط بهيئة الغذاء والدواء', N'Linked with the Food and Drug Administration', 0)
GO
INSERT [dbo].[RegisterType] ([Id], [Name], [NameEn], [IsDeleted]) VALUES (5, N'مرتبط بهيئة النقل - اختباري', N'Linked with Transport Authority - Test', 0)
GO
INSERT [dbo].[RegisterType] ([Id], [Name], [NameEn], [IsDeleted]) VALUES (7, N'مرتبط بهيئة الغذاء والدواء - اختباري', N'Linked with the Food and Drug Administration - Test', 0)
GO
SET IDENTITY_INSERT [dbo].[RegisterType] OFF
GO


GO
SET IDENTITY_INSERT [dbo].[PrivilegeType] ON 

INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (1, N'مشاهدة الشركات', N'View Companies', 0, 1, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (2, N'إضافة / تعديل الشركات', N'Add \ Edit Companies', 0, 2, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (12, N'تفعيل التنبيهات', N'Enable Alert Notifications', 0, 12, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (14, N'مشاهدة Gateway', N'View Gateway', 0, 14, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (15, N'إضافة / تعديل Gateway', N'Add \ Edit Gateway', 0, 15, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (16, N'مشاهدة المستودعات', N'View Warehouses', 0, 16, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (17, N'إضافة / تعديل  المستودعات', N'Add \ Edit Warehouses', 0, 17, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (18, N'مشاهدة المخازن', N'View Inventories', 0, 18, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (19, N'إضافة / تعديل  المخازن', N'Add \ Edit Inventories', 0, 19, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (20, N'مشاهدة المستخدمين', N'View Users', 0, 20, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (21, N'إضافة / تعديل  المستخدمين', N'Add \ Edit Users', 0, 21, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (22, N'تمكين صلاحيات المستخدمين', N'Enable Users Privileges', 0, 22, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 0)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (23, N'مشاهدة  العمليات', N'View Operations', 0, 23, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (24, N'الاستعلام في وصل', N'Inquiry at Wasl', 0, 24, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (25, N'إدارة العمليات Job', N'Manage Jobs', 0, 25, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 0)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (26, N'حذف الشركات', N'Delete Companies', 0, 2, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (31, N'حذف Gateway', N'Delete Gateway', 0, 15, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (32, N'حذف المستودعات', N'Delete Warehouses', 0, 17, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (33, N'حذف المخازن', N'Delete Inventories', 0, 19, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (34, N'حذف المستخدمين', N'Delete Users', 0, 21, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (101, N'الشريط العلوي : إظهار قسم الرئيسية', N'Top Bar: Show The Main Header', 0, 1, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (102, N'الشريط العلوي : إظهار قسم المستودعات', N'Top Bar: Show The Warehouses Section', 0, 2, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (103, N'الشريط العلوي : إظهار قسم الإدارة', N'Top Bar: Show The Management Section', 0, 3, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (114, N'قسم الإدارة : مشاهدة الإنذارات المخصصة', N'Administration Section: View Groups', 1, 14, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (115, N'قسم الإدارة : إدارة الإنذارات المخصصة', N'Administration Section: View Custom Alerts', 0, 12, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (116, N'قسم التقارير : تقرير الحرارة والرطوبة', N'Reports Section: Temperature and Humidity Report', 0, 23, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (117, N'قسم التقارير :  تقرير الحرارة والرطوبة PDF, Excel', N'PDF, Excel Reports Section: Temperature and Humidity Report', 1, 17, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (128, N'قسم التقارير : تقرير سجل المستودعات', N'Reports Section: Warehouse Record Report', 0, 21, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (129, N'قسم التقارير : تقرير سجل المستودعات PDF, Excel', N'PDF, Excel Reports Section: Warehouse Record Report', 1, 29, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (130, N'قسم التقارير : جدولة التقارير', N'Reports Section: Scheduling Reports', 1, 30, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (131, N'قسم التقارير : إدارة جدولة التقارير', N'Reports Section: Report Scheduling Management', 0, 22, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (132, N'تعديل بيانات وصل', N'Edit Wasl Data', 0, 14, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (133, N'الشريط العلوي : لوحة الحرارة والرطوبة', N'Top Bar: Temperature And Humidity Panel', 0, 5, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (134, N'قسم التقارير : تقرير سجل المخزن', N'Reports Section: Inventories Record Report', 0, 24, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (135, N'قسم التقارير : تقرير سجل المخزن PDF, Excel', N'PDF, Excel Reports Section: Inventories Record Report', 1, 35, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (136, N'قسم التقارير : تقرير متوسط الحرارة والرطوبة اليومي', N'Reports Section: Daily Average Temperature And Humidity Report', 0, 25, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (144, N'قسم التقارير : تقرير الإنذارات', N'Reports Section: Alarms Report', 0, 41, N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (149, N'مشاهدة الحساسات', N'View Sensors', 0, 25, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (150, N'إضافة/تعديل الحساسات', N'Add/Update Sensors', 0, 26, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
INSERT [dbo].[PrivilegeType] ([Id], [Name], [NameEn], [IsDeleted], [Order], [RoleId], [Editable]) VALUES (151, N'حذف الحساسات', N'Delete Sensors', 0, 27, N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', 1)
SET IDENTITY_INSERT [dbo].[PrivilegeType] OFF
GO
INSERT [dbo].[Role] ([Id], [Name], [DisplayName], [DisplayNameEn], [Order]) VALUES (N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549', N'administrator', N'مدير النظام', N'Administrator', 99)
INSERT [dbo].[Role] ([Id], [Name], [DisplayName], [DisplayNameEn], [Order]) VALUES (N'eb954fe4-0e02-4b5e-a4ee-2cff1315d65d', N'agent', N'عميل', N'Agent', 1)
GO
SET IDENTITY_INSERT [dbo].[UserRole] ON 

INSERT [dbo].[UserRole] ([Id], [UserId], [RoleId]) VALUES (1, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', N'91e30d91-0ab9-4ec0-b45d-5c8de8cf2549')
SET IDENTITY_INSERT [dbo].[UserRole] OFF
GO
------------------------------------------------------------------------------------------------------------------------------
GO
SET IDENTITY_INSERT [dbo].[Brand] ON 
GO
INSERT [dbo].[Brand] ([Id], [Name], [NameEn], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'Lora', N'Lora', 0, CAST(N'2021-12-12T00:00:00.0000000' AS DateTime2), NULL, CAST(N'2021-12-12T00:00:00.0000000' AS DateTime2), NULL)
GO
INSERT [dbo].[Brand] ([Id], [Name], [NameEn], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'GM Sensor', N'GM Sensor', 0, CAST(N'2021-12-12T00:00:00.0000000' AS DateTime2), NULL, CAST(N'2021-12-12T00:00:00.0000000' AS DateTime2), NULL)
GO
SET IDENTITY_INSERT [dbo].[Brand] OFF
GO
------------------------------------------------------------------------------------------------------------------------------
GO
SET IDENTITY_INSERT [dbo].[InventorySensor] ON 
GO
INSERT [dbo].[InventorySensor] ([Id], [BrandId], [Serial], [Name], [IsDeleted], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, 1, 1125559007, N'Lora Sensors', 0, CAST(N'2021-12-12T00:00:00.0000000' AS DateTime2), NULL, CAST(N'2021-12-12T00:00:00.0000000' AS DateTime2), NULL)
GO
SET IDENTITY_INSERT [dbo].[InventorySensor] OFF
GO

------------------------------------------------------------------------------------------------------------------------------
GO
SET IDENTITY_INSERT [dbo].[UserPrivilege] ON 

INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (47, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 1, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (48, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 2, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (49, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 26, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (50, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 12, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (51, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 14, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (52, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 15, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (53, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 31, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (54, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 16, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (55, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 17, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (56, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 32, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (57, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 18, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (58, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 19, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (59, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 33, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (60, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 20, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (61, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 21, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (62, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 34, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (63, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 23, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (64, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 24, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (65, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 149, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (66, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 150, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (67, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 151, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (68, N'299606bc-b3b4-1b44-bd6n-2c7882f127dc', 22, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (69, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 101, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (70, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 102, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (71, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 103, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (72, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 133, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (73, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 115, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (74, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 132, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (75, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 128, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (76, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 131, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (77, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 116, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (78, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 134, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (79, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 136, 1)
INSERT [dbo].[UserPrivilege] ([Id], [UserId], [PrivilegeTypeId], [IsActive]) VALUES (80, N'9493b361-6590-498b-9a3e-967ad1ab71d0', 144, 1)
SET IDENTITY_INSERT [dbo].[UserPrivilege] OFF
GO
------------------------------------------------------------------------------------------------------------------------------
GO
SET IDENTITY_INSERT [dbo].[OnlineInventoryHistory] ON 
GO
INSERT [dbo].[OnlineInventoryHistory] ([Id], [GatewayIMEI], [Serial], [Temperature], [Humidity], [IsLowVoltage], [GpsDate], [Alram], [GSMStatus]) 
VALUES (1, 112233, N'123', CAST(21.00 AS Decimal(8, 2)), CAST(36.00 AS Decimal(8, 2)), 0, CAST(N'2022-01-22T18:06:25.220' AS DateTime), N'AA', N'00110111')
GO
INSERT [dbo].[OnlineInventoryHistory] ([Id], [GatewayIMEI], [Serial], [Temperature], [Humidity], [IsLowVoltage], [GpsDate], [Alram], [GSMStatus]) 
VALUES (2, 112233,  N'1234', CAST(26.00 AS Decimal(8, 2)), CAST(24.00 AS Decimal(8, 2)), 0, CAST(N'2022-01-22T18:35:10.360' AS DateTime), N'AA', N'00110111')
GO
SET IDENTITY_INSERT [dbo].[OnlineInventoryHistory] OFF
GO
------------------------------------------------------------------------------------------------------------------------------
INSERT [dbo].[ReportTypeLookup] ([Id], [Name], [NameEn], [IsDeleted]) VALUES (8, N'تقرير سجل المخزن', N'Inventory History Report', 0)
GO
------------------------------------------------------------------------------------------------------------------------------

GO
INSERT [dbo].[ScheduleTypeLookup] ([Id], [Name], [NameEn]) VALUES (1, N'يومي', N'Daily')
GO
INSERT [dbo].[ScheduleTypeLookup] ([Id], [Name], [NameEn]) VALUES (2, N'اسبوعي', N'Weekly')
GO
INSERT [dbo].[ScheduleTypeLookup] ([Id], [Name], [NameEn]) VALUES (3, N'شهري', N'Monthly')
GO
INSERT [dbo].[ScheduleTypeLookup] ([Id], [Name], [NameEn]) VALUES (4, N'سنوي', N'Yearly')

------------------------------------------------------------------------------------------------------------------------------
GO
INSERT [dbo].[DayOfWeekLookup] ([Id], [Name], [NameEn], [RowOrder]) VALUES (0, N'الاحد', N'Sunday', 2)
GO
INSERT [dbo].[DayOfWeekLookup] ([Id], [Name], [NameEn], [RowOrder]) VALUES (1, N'الاثنين', N'Monday', 3)
GO
INSERT [dbo].[DayOfWeekLookup] ([Id], [Name], [NameEn], [RowOrder]) VALUES (2, N'الثلاثاء', N'Tuesday', 4)
GO
INSERT [dbo].[DayOfWeekLookup] ([Id], [Name], [NameEn], [RowOrder]) VALUES (3, N'الاربعاء', N'Wednesday', 5)
GO
INSERT [dbo].[DayOfWeekLookup] ([Id], [Name], [NameEn], [RowOrder]) VALUES (4, N'الخميس', N'Thursday', 6)
GO
INSERT [dbo].[DayOfWeekLookup] ([Id], [Name], [NameEn], [RowOrder]) VALUES (5, N'الجمعة', N'Friday', 7)
GO
INSERT [dbo].[DayOfWeekLookup] ([Id], [Name], [NameEn], [RowOrder]) VALUES (6, N'السبت', N'Saturday', 1)
GO
------------------------------------------------------------------------------------------------------------------------------


USE [TrackerDB]
GO
SET IDENTITY_INSERT [dbo].[AlertTypeLookup] ON 

INSERT [dbo].[AlertTypeLookup] ([Id], [Name], [NameEn], [RowOrder], [IsRange], [HasMinValue], [HasMaxValue], [DataType], [Unit], [UnitEn], [IsDeleted]) VALUES (1, N'درجة الرطوبة خارج المعدل', N'Humidity Out Of Rang', 1, 1, 1, 1, N'number', NULL, NULL, 0)
INSERT [dbo].[AlertTypeLookup] ([Id], [Name], [NameEn], [RowOrder], [IsRange], [HasMinValue], [HasMaxValue], [DataType], [Unit], [UnitEn], [IsDeleted]) VALUES (2, N'درجة الحرارة خارج المعدل', N'Temperature Out Of Rang', 1, 1, 1, 1, N'number', NULL, NULL, 0)
INSERT [dbo].[AlertTypeLookup] ([Id], [Name], [NameEn], [RowOrder], [IsRange], [HasMinValue], [HasMaxValue], [DataType], [Unit], [UnitEn], [IsDeleted]) VALUES (3, N'درجة الحرارة والرطوبة خارج المعدل', N'Temperature And Humidity Out Of Rang', 1, 1, 1, 1, N'number', NULL, NULL, 0)
SET IDENTITY_INSERT [dbo].[AlertTypeLookup] OFF
GO