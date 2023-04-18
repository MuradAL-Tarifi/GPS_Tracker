using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.DTO
{
    public enum HttpCode
    {
        /// <summary>
        /// تمت العملية بنجاح
        /// </summary>
        Success = 200,
        /// <summary>
        /// خطأ في القيم الممررة
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// دخول غير مصرح
        /// </summary>
        Unauthorized = 401,
        /// <summary>
        /// البيانات غير موجودة
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// حدث خطأ في النظام
        /// </summary>
        ServerError = 500
    }


    public enum ReportTypeEnum
    {
        /// <summary>
        /// تقرير الحرارة والرطوبة
        /// </summary>
        TemperatureAndHumidityReport = 1,

        /// <summary>
        /// تقرير سجل الحساسات
        /// </summary>
        InventorySensorsHistoryReport = 7,

        /// <summary>
        /// تقرير سجل المخزن 
        /// </summary>
        InventoryHistoryReport = 8,

        /// <summary>
        /// تقرير الإنذارات
        /// </summary>
        AlertsReport = 13,
    }

    public enum ScheduleTypeEnum
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4,
    }



    public enum AdminPrivilegeTypeEnum
    {
        /// <summary>
        /// مشاهدة  الأساطيل
        /// </summary>
        ViewFleets = 1,

        /// <summary>
        /// إضافة / تعديل /  الأساطيل
        /// </summary>
        AddUpdateFleets = 2,

        /// <summary>
        /// تفعيل التنبيهات
        /// </summary>
        EnableAlerts = 12,

        /// <summary>
        /// تعديل إدارة الأجهزة
        /// </summary>
        ManageManageAccounts = 13,

        /// <summary>
        /// مشاهدة Gateway
        /// </summary>
        ViewGateways = 14,

        /// <summary>
        /// إضافة / تعديل /  Gateway
        /// </summary>
        AddUpdateGateways = 15,

        /// <summary>
        /// مشاهدة المستودعات
        /// </summary>
        ViewWarehouses = 16,

        /// <summary>
        /// إضافة / تعديل /   المستودعات
        /// </summary>
        AddUpdateWarehouses = 17,

        /// <summary>
        /// مشاهدة المخازن
        /// </summary>
        ViewInventories = 18,

        /// <summary>
        /// إضافة / تعديل /   المخازن
        /// </summary>
        AddUpdateInventories = 19,

        /// <summary>
        /// مشاهدة المستخدمين
        /// </summary>
        ViewUsers = 20,

        /// <summary>
        /// إضافة / تعديل /   المستخدمين
        /// </summary>
        AddUpdateUsers = 21,

        /// <summary>
        /// تمكين صلاحيات المستخدمين
        /// </summary>
        EnableUserPrivileges = 22,

        /// <summary>
        /// مشاهدة العمليات
        /// </summary>
        ViewOperations = 23,

        /// <summary>
        /// الاستعلام في وصل
        /// </summary>
        WASLInquiries = 24,

        /// <summary>
        /// إدارة العمليات Job
        /// </summary>
        ManageJobs = 25,

        /// <summary>
        ///  حذف الأساطيل
        /// </summary>
        DeleteFleets = 26,

        /// <summary>
        /// حذف المجموعات
        /// </summary>
        //DeleteGroups = 27,

        /// <summary>
        ///  حذف السائقن
        /// </summary>
        DeleteDrivers = 28,

        /// <summary>
        /// حذف الأجهزة
        /// </summary>
        DeleteAccounts = 29,

        /// <summary>
        /// حذف المركبات
        /// </summary>
        DeleteVehicles = 30,

        /// <summary>
        /// حذف Gateway
        /// </summary>
        DeleteGateways = 31,

        /// <summary>
        ///  حذف المستودعات
        /// </summary>
        DeleteWarehouses = 32,

        /// <summary>
        ///  حذف المخازن
        /// </summary>
        DeleteInventories = 33,

        /// <summary>
        /// حذف المستخدمين
        /// </summary>
        DeleteUsers = 34,

        /// <summary>
        /// مشاهدة الحساسات
        /// </summary>
        ViewSensors = 149,

        /// <summary>
        /// إضافة/تعديل الحساسات
        /// </summary>
        AddUpdateSensors = 150,

        /// <summary>
        /// حذف الحساسات
        /// </summary>
        DeleteSensors = 151,

        /// <summary>
        /// إعدادات النظام
        /// </summary>
        SystemSettings = 158,

    }

    public enum AgentPrivilegeTypeEnum
    {

        /// <summary>
        /// الشريط العلوي : إظهار قسم المستودعات
        /// </summary>
        ShowWarehouseMenuItem = 102,

        /// <summary>
        /// قسم الإدارة : إدارة الإنذارات المخصصة
        /// </summary>
        ManageCustomAlerts = 115,

        /// <summary>
        /// قسم التقارير : تقرير الحرارة والرطوبة
        /// </summary>
        ViewTemperatureAndHumidityReport = 116,

        /// <summary>
        /// قسم التقارير : تقرير سجل المستودعات
        /// </summary>
        ViewWarehousesHistoryReport = 128,


        /// <summary>
        /// قسم التقارير : إدارة جدولة التقارير
        /// </summary>
        ManageReportSchedule = 131,

        /// <summary>
        /// تعديل بيانات وصل
        /// </summary>
        UpdateWaslInfo = 132,

        /// <summary>
        /// الشريط العلوي : لوحة الحرارة والرطوبة
        /// </summary>
        ShowWarehouseOnlineDashboardMenuItem = 133,

        /// <summary>
        /// قسم التقارير : تقرير سجل المخزن
        /// </summary>
        ViewInventoryHistoryReport = 134,

        /// <summary>
        /// قسم التقارير : تقرير سجل المخزن PDF, Excel
        /// </summary>
        InventoryHistoryReportPDFExcel = 135,

        /// <summary>
        /// قسم التقارير : تقرير متوسط الحرارة والرطوبة اليومي
        /// </summary>
        ViewAverageDailyTemperatureAndHumidityReport = 136,

        /// <summary>
        /// قسم التقارير : تقرير الإنذارات
        /// </summary>
        ViewAlertsReport = 144,

        /// <summary>
        /// قسم إعدادات النظام : مشاهدة المستخدمين
        /// </summary>
        ViewUsers = 153,

        /// <summary>
        /// قسم إعدادات النظام : إضافة/تعديل المستخدمين
        /// </summary>
        AddUpdateUsers = 154,

        /// <summary>
        /// قسم إعدادات النظام : حذف المستخدمين 
        /// </summary>
        DeleteUsers = 156,

        /// <summary>
        /// قسم إعدادات النظام : تمكين الصلاحيات المستخدمين
        /// </summary>
        EnableUserPrivileges = 157,

        /// <summary>
        /// تمكين تعديل إعدادات النظام
        /// </summary>
        UpdateCompanySettings = 161,
        /// <summary>
        /// قسم التقارير : تقرير الحساسات التي تعمل والتي لاتعمل
        /// </summary>
        ViewWorkingAndNotWorkingSensorReport = 164,

    }
    public enum Roles
    {
        administrator,
        agent
    }
    public enum Event
    {
        update,
        create,
        delete,
        registerWaslVehicle,
        updateWaslVehicle,
        deleteWaslVehicle,
        registerWaslDriver,
        updateWaslDriver,
        deleteWaslDriver,
        registerWaslCompany,
        UpdateWaslContactInfo,
        deleteWaslCompany,
        registerWaslIndividual,
        deleteWaslIndividual,
        registerWaslWarehouse,
        updateWaslWarehouse,
        deleteWaslWarehouse,
        registerWaslInventory,
        updateWaslInventory,
        deleteWaslInventory,
    }

    /// <summary>
    /// مجموعة الإنذارات
    /// </summary>
    public enum AlertGroupEnum
    {
        /// <summary>
        /// جهاز
        /// </summary>
        Device = 1,

        /// <summary>
        /// مخصص
        /// </summary>
        Custom = 2,

        /// <summary>
        /// صيانة
        /// </summary>
        Maintenance = 3,
    }

    public enum WaslIntegrationLogTypeEnum
    {
        OperatingCompany_RegisterCompany = 1,
        OperatingCompany_RegisterIndividual = 2,
        OperatingCompany_UpdateContactInfo = 3,
        OperatingCompany_Get = 4,
        OperatingCompany_Delete = 5,
        Vehicle_Register = 10,
        Vehicle_UpdateIMEI = 11,
        Vehicle_Get = 12,
        Vehicle_Delete = 13,
        Driver_Register = 20,
        Driver_Update = 21,
        Driver_Get = 22,
        Driver_Delete = 23,
        Taxi_StartTrip = 30,
        Taxi_EndTrip = 31,
        Warehouse_Register = 40,
        Warehouse_Update = 41,
        Warehouse_Delete = 42,
        Warehouse_Get = 43,
        Inventory_Register = 50,
        Inventory_StatsRegisteration = 51,
        Inventory_Update = 52,
        Inventory_Delete = 53,
        Vehicle_StoringCategory = 54
    }

    /// <summary>
    /// أنواع الإنذارات المخصصة
    /// </summary>
    public enum CustomAlertTypeEnum
    {
        /// <summary>
        /// درجة الرطوبة خارج المعدل
        /// </summary>
        HumidityOutOfRang = 1,

        /// <summary>
        /// درجة الحرارة خارج المعدل
        /// </summary>
        TemperatureOutOfRang = 2,

        /// <summary>
        /// درجة الحرارة والرطوبة خارج المعدل
        /// </summary>
        TemperatureAndHumidityOutOfRang = 3,
    }
}
