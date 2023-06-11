#region

using System.Reflection;
using PyroCommon.API;
using Rage;
// ReSharper disable InconsistentNaming

#endregion

namespace DeadlyWeapons;

internal static class Settings
{
    internal static bool EnablePlayerDamageSystem = true;
    internal static bool EnableAIDamageSystem = true;
    internal static bool EnablePlayerHeadshotInstakill;
    internal static bool EnablePanic = true;
    internal static int PanicCooldown = 120;
    internal static bool EnablePulloverAi = true;
    internal static bool Code3Backup = true;
    internal static bool SwatBackup;
    internal static bool NooseBackup;
    internal static bool EnableDebug;
    internal static readonly string CalloutVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

    internal static void LoadSettings()
    {
        Log.Info("Loading config.");
        var path = "Plugins/LSPDFR/DeadlyWeapons.ini";
        var ini = new InitializationFile(path);
        ini.Create();
        EnablePlayerDamageSystem = ini.ReadBoolean("Features", "EnablePlayerDamageSystem", true);
        EnableAIDamageSystem = ini.ReadBoolean("Features", "EnableAIDamageSystem", true);
        EnablePlayerHeadshotInstakill = ini.ReadBoolean("Features", "EnablePlayerHeadshotInstakill", false);
        EnablePanic = ini.ReadBoolean("Features", "EnablePanic", true);
        PanicCooldown = ini.ReadInt32("Features", "PanicCooldown", 120);
        EnablePulloverAi = ini.ReadBoolean("Features", "EnablePulloverAi", true);
        Code3Backup = ini.ReadBoolean("Backup", "Code3Backup", true);
        SwatBackup = ini.ReadBoolean("Backup", "SwatBackup");
        NooseBackup = ini.ReadBoolean("Backup", "NooseBackup");
        EnableDebug = ini.ReadBoolean("Debug", "EnableDebug", false);
        Log.Info("Config loaded.");
    }
}