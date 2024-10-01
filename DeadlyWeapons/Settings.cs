using System.Collections.Generic;
using System.Windows.Forms;
using DeadlyWeapons.Configs;
using PyroCommon.PyroFunctions;
using Rage;
using Weapon = DeadlyWeapons.Configs.Weapon;

namespace DeadlyWeapons;

internal static class Settings
{
    internal static List<Weapon> Weapons = [];
    internal static List<WeaponType> WeaponTypes = [];
    internal static DamageValues PlayerValues = new ();
    internal static DamageValues PlayerArmorValues = new ();
    internal static DamageValues NpcValues = new ();
    internal static DamageValues NpcArmorValues = new ();
    internal static bool PlayerDamage = true;
    internal static bool NpcDamage = true;
    internal static float DamageRandomizer = 15;
    internal static bool Panic = true;
    internal static int PanicCooldown = 120;
    internal static bool Code3Backup = true;
    internal static bool SwatBackup;
    internal static bool NooseBackup;
    internal static bool Debug;

    internal static void LoadSettings()
    {
        Log.Info("Loading configs...");
        
        //INI Config
        Log.Info("DeadlyWeapons.ini");
        var ini = new InitializationFile("Plugins/LSPDFR/DeadlyWeapons.ini");
        ini.Create();
        PlayerDamage = ini.ReadBoolean("Features", "PlayerDamage", true);
        NpcDamage = ini.ReadBoolean("Features", "NPCDamage", true);
        DamageRandomizer = ini.ReadSingle("Features", "DamageRandomizer", 15);
        Panic = ini.ReadBoolean("Features", "Panic", true);
        PanicCooldown = ini.ReadInt32("Features", "PanicCooldown", 120);
        Code3Backup = ini.ReadBoolean("Backup", "Code3Backup", true);
        SwatBackup = ini.ReadBoolean("Backup", "SwatBackup");
        NooseBackup = ini.ReadBoolean("Backup", "NooseBackup");
        PyroCommon.PyroFunctions.UIManager.Manager.AddManagerKey(ini.ReadEnum("Keys", "PluginManager", Keys.K));
        Debug = ini.ReadBoolean("Debug", "Debug");
        
        // YAML Configs
        Log.Info("Weapons.yml");
        Weapons = PyroCommon.PyroFunctions.PyroFunctions.DeserializeYaml<List<Weapon>>("Plugins/LSPDFR/DeadlyWeapons/Weapons.yml");
        Log.Info("WeaponTypes.yml");
        WeaponTypes = PyroCommon.PyroFunctions.PyroFunctions.DeserializeYaml<List<WeaponType>>("Plugins/LSPDFR/DeadlyWeapons/WeaponTypes.yml");
        Log.Info("Damage.yml");
        var damageConfig = PyroCommon.PyroFunctions.PyroFunctions.DeserializeYaml<DamageConfigurations>("Plugins/LSPDFR/DeadlyWeapons/Damage.yml");
        PlayerArmorValues = damageConfig.PlayerDamage.WithArmor;
        NpcArmorValues = damageConfig.NpcDamage.WithArmor;
        PlayerValues = damageConfig.PlayerDamage.WithoutArmor;
        NpcValues = damageConfig.NpcDamage.WithoutArmor;

        Log.Info("Configs loaded.");
    }
}