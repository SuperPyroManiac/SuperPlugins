using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DeadlyWeapons.Configs;
using PyroCommon.API;
using Rage;
using YamlDotNet.Serialization;
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
    internal static bool Panic = true;
    internal static int PanicCooldown = 120;
    internal static bool Code3Backup = true;
    internal static bool SwatBackup;
    internal static bool NooseBackup;
    internal static bool Debug;

    internal static void LoadSettings()
    {
        Log.Info("Loading configs.");
        
        //INI Config
        var ini = new InitializationFile("Plugins/LSPDFR/DeadlyWeapons.ini");
        ini.Create();
        PlayerDamage = ini.ReadBoolean("Features", "PlayerDamage", true);
        NpcDamage = ini.ReadBoolean("Features", "NPCDamage", true);
        Panic = ini.ReadBoolean("Features", "Panic", true);
        PanicCooldown = ini.ReadInt32("Features", "PanicCooldown", 120);
        Code3Backup = ini.ReadBoolean("Backup", "Code3Backup", true);
        SwatBackup = ini.ReadBoolean("Backup", "SwatBackup");
        NooseBackup = ini.ReadBoolean("Backup", "NooseBackup");
        Debug = ini.ReadBoolean("Debug", "Debug");
        
        // YAML Configs
        Weapons = DeserializeYaml<List<Weapon>>("Plugins/LSPDFR/DeadlyWeapons/Weapons.yml");
        WeaponTypes = DeserializeYaml<List<WeaponType>>("Plugins/LSPDFR/DeadlyWeapons/WeaponTypes.yml");

        var damageConfig = DeserializeYaml<DamageConfigurations>("Plugins/LSPDFR/DeadlyWeapons/Damage.yml");
        PlayerArmorValues = damageConfig.PlayerDamage.WithArmor;
        NpcArmorValues = damageConfig.NpcDamage.WithArmor;
        PlayerValues = damageConfig.PlayerDamage.WithoutArmor;
        NpcValues = damageConfig.NpcDamage.WithoutArmor;

        Log.Info("Configs loaded.");
    }

    private static T DeserializeYaml<T>(string path)
    {
        using (var reader = new StreamReader(path))
        {
            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<T>(reader);
        }
    }
}