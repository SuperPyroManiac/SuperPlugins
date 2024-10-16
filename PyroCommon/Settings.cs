using System.Windows.Forms;
using Rage;

namespace PyroCommon;

internal static class Settings
{
    internal static bool UpdateNotifications = true;
    internal static bool ErrorReporting = true;
    internal static bool DisableManagerUI = false;
    internal static bool FirstTime = true;
    internal static Keys Manager = Keys.K;

    internal static void LoadSettings()
    {
        var ini = new InitializationFile("PyroCommon.ini");
        ini.Create();
        UpdateNotifications = ini.ReadBoolean("Settings", "UpdateNotifications", true);
        ErrorReporting = ini.ReadBoolean("Settings", "ErrorReporting", true);
        DisableManagerUI = ini.ReadBoolean("Settings", "DisableManagerUI", false);
        FirstTime = ini.ReadBoolean("Settings", "FirstTime", true);
        Manager = ini.ReadEnum("Keys", "PluginManager", Keys.K);
    }

    internal static void SaveSettings()
    {
        var ini = new InitializationFile("PyroCommon.ini");
        ini.Create();
        ini.Write("Settings", "UpdateNotifications", UpdateNotifications);
        ini.Write("Settings", "ErrorReporting", ErrorReporting);
        ini.Write("Settings", "FirstTime", FirstTime);
        ini.Write("Keys", "PluginManager", Manager);
    }
}