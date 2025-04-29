using System;
using DamageTrackerLib.DamageInfo;
using DeadlyWeapons.PyroFunctions;
using PyroCommon.Extensions;
using PyroCommon.Utils;
using Rage;

namespace DeadlyWeapons.Modules;

internal static class PlayerShot
{
    private static Ped Player => Game.LocalPlayer.Character;

    internal static void OnPlayerDamaged(Ped victim, Ped attacker, PedDamageInfo damageInfo)
    {
        if (Player.IsDead)
            return;
        if (victim != Player)
            return;
        if (damageInfo.WeaponInfo.Group != DamageGroup.Bullet)
            return;
        if (Settings.Debug)
        {
            LogUtils.Info("[DEBUG]: Detailed damage info Start");
            LogUtils.Info(
                $"\n{victim.Model.Name} ({damageInfo.Damage} Dmg) ({(victim.IsAlive ? "Alive" : "Dead")})"
                    + $"\n{attacker?.Model.Name ?? "None"}"
                    + $"\n{damageInfo.WeaponInfo.Hash.ToString()} {damageInfo.WeaponInfo.Type.ToString()} {damageInfo.WeaponInfo.Group.ToString()}"
                    + $"\n{damageInfo.BoneInfo.BoneId.ToString()} {damageInfo.BoneInfo.Limb.ToString()} {damageInfo.BoneInfo.BodyRegion.ToString()}"
            );
            LogUtils.Info("[DEBUG]: Detailed damage info Stop");
            LogUtils.Info("[DEBUG]: Player health before shot: " + Player.Health);
            LogUtils.Info("[DEBUG]: Player armor before shot: " + Player.Armor);
        }

        var weapon = Utils.GetWeaponByHash((Rage.WeaponHash)damageInfo.WeaponInfo.Hash);
        switch (damageInfo.BoneInfo.BodyRegion)
        {
            case BodyRegion.Head:
                if (Player.Armor > 5)
                {
                    var headDamage = (int)
                        Utils.RandomizeValue(
                            (Settings.PlayerArmorValues.Head - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"Player shot in head with armor! Damage: {headDamage}");
                    Player.Health -= headDamage;
                }
                else
                {
                    var withoutArmor = (int)
                        Utils.RandomizeValue(
                            (Settings.PlayerValues.Head - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"Player shot in head without armor! Damage: {withoutArmor}");
                    Player.Health -= withoutArmor;
                }
                break;
            case BodyRegion.Torso:
                if (Player.Armor > 5)
                {
                    var torsoDamage = (int)
                        Utils.RandomizeValue(
                            (Settings.PlayerArmorValues.Torso - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"Player shot in torso with armor! Damage: {torsoDamage}");
                    Player.Health -= torsoDamage;
                }
                else
                {
                    var torsoDamageWithoutArmor = (int)
                        Utils.RandomizeValue(
                            (Settings.PlayerValues.Torso - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"Player shot in torso without armor! Damage: {torsoDamageWithoutArmor}");
                    Player.Health -= torsoDamageWithoutArmor;
                }
                break;
            case BodyRegion.Arms:
                if (Player.Armor > 5)
                {
                    var armsDamage = (int)
                        Utils.RandomizeValue(
                            (Settings.PlayerArmorValues.Arms - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"Player shot in arms with armor! Damage: {armsDamage}");
                    Player.Health -= armsDamage;
                }
                else
                {
                    var armsDamageWithoutArmor = (int)
                        Utils.RandomizeValue(
                            (Settings.PlayerValues.Arms - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"Player shot in arms without armor! Damage: {armsDamageWithoutArmor}");
                    Player.Health -= armsDamageWithoutArmor;
                }
                break;
            case BodyRegion.Legs:
                if (new Random(DateTime.Now.Millisecond).Next(0, 3) == 0)
                    Player.Tasks.Ragdoll();
                if (Player.Armor > 5)
                {
                    var legsDamage = (int)
                        Utils.RandomizeValue(
                            (Settings.PlayerArmorValues.Legs - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"Player shot in legs with armor! Damage: {legsDamage}");
                    Player.Health -= legsDamage;
                }
                else
                {
                    var legsDamageWithoutArmor = (int)
                        Utils.RandomizeValue(
                            (Settings.PlayerValues.Legs - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"Player shot in legs without armor! Damage: {legsDamageWithoutArmor}");
                    Player.Health -= legsDamageWithoutArmor;
                }
                break;
            case BodyRegion.None:
            default:
                LogUtils.Warning("Unknown body region " + damageInfo.BoneInfo.BodyRegion);
                break;
        }

        if (!Settings.Debug)
            return;
        LogUtils.Info("[DEBUG]: Player health after shot: " + Player.Health);
        LogUtils.Info("[DEBUG]: Player armor after shot: " + Player.Armor);
    }
}
