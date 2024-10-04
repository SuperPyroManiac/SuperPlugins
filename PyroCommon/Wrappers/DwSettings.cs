using System;
using System.Reflection;

namespace PyroCommon.Wrappers;

internal static class DwSettings
{
    internal static bool _playerDamage;
    internal static bool _npcDamage;
    internal static float _damageRandomizer;
    internal static bool _panic;
    internal static int _panicCooldown;
    internal static bool _code3Backup;
    internal static bool _swatBackup;
    internal static bool _nooseBackup;
    internal static bool _debug;

    internal static readonly Type SettingsType = Assembly.Load("DeadlyWeapons").GetType("DeadlyWeapons.Settings");

    internal static void GetSettings()
    {
        if (!Main.UsingDw) return;
        _playerDamage = (bool)SettingsType.GetField("PlayerDamage", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _npcDamage = (bool)SettingsType.GetField("NpcDamage", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _damageRandomizer = (float)SettingsType.GetField("DamageRandomizer", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _panic = (bool)SettingsType.GetField("Panic", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _panicCooldown = (int)SettingsType.GetField("PanicCooldown", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _code3Backup = (bool)SettingsType.GetField("Code3Backup", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _swatBackup = (bool)SettingsType.GetField("SwatBackup", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _nooseBackup = (bool)SettingsType.GetField("NooseBackup", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
        _debug = (bool)SettingsType.GetField("Debug", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null);
    }

    internal static void ApplySettings()
    {
        if (!Main.UsingDw) return;
        SettingsType.GetField("PlayerDamage", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _playerDamage);
        SettingsType.GetField("NpcDamage", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _npcDamage);
        SettingsType.GetField("DamageRandomizer", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _damageRandomizer);
        SettingsType.GetField("Panic", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _panic);
        SettingsType.GetField("PanicCooldown", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _panicCooldown);
        SettingsType.GetField("Code3Backup", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _code3Backup);
        SettingsType.GetField("SwatBackup", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _swatBackup);
        SettingsType.GetField("NooseBackup", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _nooseBackup);
        SettingsType.GetField("Debug", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, _debug);
    }

    internal static void SaveSettings()
    {
        if ( !Main.UsingDw ) return;
        var saveSettingsMethod = SettingsType.GetMethod("SaveSettings", BindingFlags.Static | BindingFlags.NonPublic);
        saveSettingsMethod?.Invoke(null, null);
    }
}