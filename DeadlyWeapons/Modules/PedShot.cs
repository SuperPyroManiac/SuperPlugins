using System;
using DamageTrackerLib.DamageInfo;
using DeadlyWeapons.PyroFunctions;
using PyroCommon.API;
using Rage;

namespace DeadlyWeapons.Modules;

internal static class PedShot
{
    internal static void OnPedDamaged(Ped victim, Ped attacker, PedDamageInfo damageInfo)
    {
        if ( victim.IsDead ) return;
        if ( victim == Game.LocalPlayer.Character ) return;
        if ( damageInfo.WeaponInfo.Group != DamageGroup.Bullet ) return;
        if (Settings.Debug)
        {
            Log.Info("[DEBUG]: Detailed damage info Start");
            Log.Info(
                $"\n{victim.Model.Name} ({damageInfo.Damage} Dmg) ({(victim.IsAlive ? "Alive" : "Dead")})" +
                $"\n{attacker?.Model.Name ?? "None"}" +
                $"\n{damageInfo.WeaponInfo.Hash.ToString()} {damageInfo.WeaponInfo.Type.ToString()} {damageInfo.WeaponInfo.Group.ToString()}" +
                $"\n{damageInfo.BoneInfo.BoneId.ToString()} {damageInfo.BoneInfo.Limb.ToString()} {damageInfo.BoneInfo.BodyRegion.ToString()}");
            Log.Info("[DEBUG]: Detailed damage info Stop");
            Log.Info("[DEBUG]: Vanilla damage amount: " + damageInfo.Damage);
            Log.Info("[DEBUG]: Victim health before shot: " + victim.Health);
            Log.Info("[DEBUG]: Victim armor before shot: " + victim.Armor);
        }

        var weapon = Utils.GetWeaponByHash(( Rage.WeaponHash )damageInfo.WeaponInfo.Hash);
        victim.Health += damageInfo.Damage;
        switch ( damageInfo.BoneInfo.BodyRegion )
        {
            case BodyRegion.Head:
                if (victim.Armor > 5)
                {
                    victim.Health -= (int)Utils.RandomizeValue(Settings.NpcArmorValues.Head * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                else
                {
                    victim.Health -= (int)Utils.RandomizeValue(Settings.NpcValues.Head * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                break;
            case BodyRegion.Torso:
                if (victim.Armor > 5)
                {
                    victim.Health -= (int)Utils.RandomizeValue(Settings.NpcArmorValues.Torso * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                else
                {
                    victim.Health -= (int)Utils.RandomizeValue(Settings.NpcValues.Torso * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                break;
            case BodyRegion.Arms:
                if (victim.Armor > 5)
                {
                    victim.Health -= (int)Utils.RandomizeValue(Settings.NpcArmorValues.Arms * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                else
                {
                    victim.Health -= (int)Utils.RandomizeValue(Settings.NpcValues.Arms * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                break;
            case BodyRegion.Legs:
                if (new Random(DateTime.Now.Millisecond).Next(0, 3) == 0) victim.Tasks.Ragdoll();
                if (victim.Armor > 5)
                {
                    victim.Health -= (int)Utils.RandomizeValue(Settings.NpcArmorValues.Legs * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                else
                {
                    victim.Health -= (int)Utils.RandomizeValue(Settings.NpcValues.Legs * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                break;
            case BodyRegion.None:
            default:
                Log.Warning("Unknown body region " + damageInfo.BoneInfo.BodyRegion);
                break;
        }

        if ( !Settings.Debug ) return;
        Log.Info("[DEBUG]: Victim health after shot: " + victim.Health);
        Log.Info("[DEBUG]: Victim armor after shot: " + victim.Armor);
    }
}