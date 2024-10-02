using System.Windows.Forms;
using Rage;

namespace PyroCommon;

internal static class Settings
{
    internal static bool UpdateNotifications = true;
    internal static bool ErrorReporting = true;
    internal static bool FirstTime = true;
    internal static Keys Manager = Keys.K;

    internal static void LoadSettings()
    {
        var ini = new InitializationFile("PyroCommon.ini");
        ini.Create();
        UpdateNotifications = ini.ReadBoolean("Settings", "UpdateNotifications", true);
        ErrorReporting = ini.ReadBoolean("Settings", "ErrorReporting", true);
        FirstTime = ini.ReadBoolean("Settings", "FirstTime", true);
        Manager = ini.ReadEnum("Keys", "PluginManager", Keys.K);
    }
}