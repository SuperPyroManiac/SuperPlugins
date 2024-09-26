using System;
using DamageTrackerLib.DamageInfo;
using DeadlyWeapons.PyroFunctions;
using PyroCommon.API;
using Rage;

namespace DeadlyWeapons.Modules;

internal static class PlayerShot
{
    private static Ped Player => Game.LocalPlayer.Character;

    internal static void OnPlayerDamaged(Ped victim, Ped attacker, PedDamageInfo damageInfo)
    {
        if ( Player.IsDead ) return;
        if ( victim != Player ) return;
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
            Log.Info("[DEBUG]: Player health before shot: " + Player.Health);
            Log.Info("[DEBUG]: Player armor before shot: " + Player.Armor);
        }

        var weapon = Utils.GetWeaponByHash(( Rage.WeaponHash )damageInfo.WeaponInfo.Hash);
        Player.Health += damageInfo.Damage;
        switch ( damageInfo.BoneInfo.BodyRegion )
        {
            case BodyRegion.Head:
                if (Player.Armor > 5)
                {
                    Player.Health -= (int)Utils.RandomizeValue(Settings.PlayerArmorValues.Head * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                else
                {
                    Player.Health -= (int)Utils.RandomizeValue(Settings.PlayerValues.Head * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                break;
            case BodyRegion.Torso:
                if (Player.Armor > 5)
                {
                    Player.Health -= (int)Utils.RandomizeValue(Settings.PlayerArmorValues.Torso * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                else
                {
                    Player.Health -= (int)Utils.RandomizeValue(Settings.PlayerValues.Torso * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                break;
            case BodyRegion.Arms:
                if (Player.Armor > 5)
                {
                    Player.Health -= (int)Utils.RandomizeValue(Settings.PlayerArmorValues.Arms * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                else
                {
                    Player.Health -= (int)Utils.RandomizeValue(Settings.PlayerValues.Arms * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                break;
            case BodyRegion.Legs:
                if (new Random(DateTime.Now.Millisecond).Next(0, 3) == 0) Player.Tasks.Ragdoll();
                if (Player.Armor > 5)
                {
                    Player.Health -= (int)Utils.RandomizeValue(Settings.PlayerArmorValues.Legs * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                else
                {
                    Player.Health -= (int)Utils.RandomizeValue(Settings.PlayerValues.Legs * (weapon.DamageMultiplier + Utils.GetWeaponType(weapon).DamageMultiplier));
                }
                break;
            case BodyRegion.None:
            default:
                Log.Warning("Unknown body region " + damageInfo.BoneInfo.BodyRegion);
                break;
        }

        if ( !Settings.Debug ) return;
        Log.Info("[DEBUG]: Player health after shot: " + Player.Health);
        Log.Info("[DEBUG]: Player armor after shot: " + Player.Armor);
    }
}