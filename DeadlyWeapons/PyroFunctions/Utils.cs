using System;
using System.Linq;
using DeadlyWeapons.Configs;
using Rage;
using Weapon = DeadlyWeapons.Configs.Weapon;

namespace DeadlyWeapons.PyroFunctions;

public class Utils
{
    internal static WeaponType GetWeaponType(Weapon weapon)
    {
        var matchingWeaponType = Settings.WeaponTypes.FirstOrDefault(w => w.Name == weapon.WeaponType);
        if (matchingWeaponType == null)
        {
            return new WeaponType
            {
                Name = "Invalid",
                DamageMultiplier = 1
            };
        }
        return matchingWeaponType;
    }
    internal static Weapon GetWeaponByHash(WeaponHash weaponHash)
    {
        var matchingWeapon = Settings.Weapons.FirstOrDefault(w => w.WeaponHash == (long)weaponHash);
        if (matchingWeapon == null)
        {
            return new Weapon
            {
                Name = "Default",
                WeaponHash = 6969,
                WeaponType = "Other",
                PanicIgnore = false,
                DamageMultiplier = 1.0f
            };
        }
        return matchingWeapon;
    }
    
    internal static float RandomizeValue(float input)
    {
        if (Settings.DamageRandomizer == 0) return input;
        var randomAdjustment = (float)(new Random().NextDouble() * (Settings.DamageRandomizer * 2)) - Settings.DamageRandomizer;
        var newValue = input + randomAdjustment;
        return (int)Math.Max(newValue, 1);
    }
}