using System.Reflection;

namespace PyroCommon.Wrappers;

internal static class DwSettings
{
    internal static bool PlayerDamage;
    internal static bool NpcDamage;
    internal static float DamageRandomizer;
    internal static bool Panic;
    internal static int PanicCooldown;
    internal static bool Code3Backup;
    internal static bool SwatBackup;
    internal static bool NooseBackup;
    internal static bool Debug;

    internal static void GetSettings()
    {
        if ( !Main.UsingDw ) return;
        var settingsType = Assembly.Load("DeadlyWeapons").GetType("DeadlyWeapons.Settings");
        PlayerDamage = ( bool )settingsType.GetField("PlayerDamage", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        NpcDamage = ( bool )settingsType.GetField("NpcDamage", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        DamageRandomizer = ( float )settingsType.GetField("DamageRandomizer", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Panic = ( bool )settingsType.GetField("Panic", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        PanicCooldown = ( int )settingsType.GetField("PanicCooldown", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Code3Backup = ( bool )settingsType.GetField("Code3Backup", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        SwatBackup = ( bool )settingsType.GetField("SwatBackup", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        NooseBackup = ( bool )settingsType.GetField("NooseBackup", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        Debug = ( bool )settingsType.GetField("Debug", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
    }

    internal static void ApplySettings()
    {
        if ( !Main.UsingDw ) return;
        var settingsType = Assembly.Load("DeadlyWeapons").GetType("DeadlyWeapons.Settings");
        settingsType.GetField("PlayerDamage", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, PlayerDamage);
        settingsType.GetField("NpcDamage", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, NpcDamage);
        settingsType.GetField("DamageRandomizer", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, DamageRandomizer);
        settingsType.GetField("Panic", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Panic);
        settingsType.GetField("PanicCooldown", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, PanicCooldown);
        settingsType.GetField("Code3Backup", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Code3Backup);
        settingsType.GetField("SwatBackup", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, SwatBackup);
        settingsType.GetField("NooseBackup", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, NooseBackup);
        settingsType.GetField("Debug", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, Debug);
    }

    internal static void SaveSettings()
    {
        if ( !Main.UsingDw ) return;
        var settingsType = Assembly.Load("DeadlyWeapons").GetType("DeadlyWeapons.Settings");
        var saveSettingsMethod = settingsType.GetMethod("SaveSettings", BindingFlags.Static | BindingFlags.NonPublic);
        saveSettingsMethod?.Invoke(null, null);
    }
}