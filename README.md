# GPS_Tracker
Add-Migration -StartupProject GPS.Web.Admin -Context "TrackerDBContext" InitialCreate -o "Context/Migrations/initDB/"



update-database -StartupProject GPS.Web.Admin -Context "TrackerDBContext" InitialCreate
