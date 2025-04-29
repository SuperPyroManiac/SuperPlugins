using System;
using DamageTrackerLib.DamageInfo;
using DeadlyWeapons.PyroFunctions;
using LSPD_First_Response.Mod.API;
using PyroCommon.Extensions;
using PyroCommon.Utils;
using Rage;

namespace DeadlyWeapons.Modules;

internal static class PedShot
{
    internal static void OnPedDamaged(Ped victim, Ped attacker, PedDamageInfo damageInfo)
    {
        if (victim.IsDead)
            return;
        if (victim == Game.LocalPlayer.Character)
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
            LogUtils.Info("[DEBUG]: Victim health before shot: " + victim.Health);
            LogUtils.Info("[DEBUG]: Victim armor before shot: " + victim.Armor);
        }

        var weapon = Utils.GetWeaponByHash((Rage.WeaponHash)damageInfo.WeaponInfo.Hash);
        switch (damageInfo.BoneInfo.BodyRegion)
        {
            case BodyRegion.Head:
                if (victim.Armor > 5)
                {
                    var headDamage = (int)
                        Utils.RandomizeValue(
                            (Settings.NpcArmorValues.Head - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"{Functions.GetPersonaForPed(victim).FullName} shot in head with armor! Damage: {headDamage}");
                    victim.Health -= headDamage;
                }
                else
                {
                    var withoutArmor = (int)
                        Utils.RandomizeValue((Settings.NpcValues.Head - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier);
                    LogUtils.Info($"{Functions.GetPersonaForPed(victim).FullName} shot in head without armor! Damage: {withoutArmor}");
                    victim.Health -= withoutArmor;
                }
                break;
            case BodyRegion.Torso:
                if (victim.Armor > 5)
                {
                    var torsoDamage = (int)
                        Utils.RandomizeValue(
                            (Settings.NpcArmorValues.Torso - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"{Functions.GetPersonaForPed(victim).FullName} shot in torso with armor! Damage: {torsoDamage}");
                    victim.Health -= torsoDamage;
                }
                else
                {
                    var torsoDamageWithoutArmor = (int)
                        Utils.RandomizeValue((Settings.NpcValues.Torso - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier);
                    LogUtils.Info(
                        $"{Functions.GetPersonaForPed(victim).FullName} shot in torso without armor! Damage: {torsoDamageWithoutArmor}"
                    );
                    victim.Health -= torsoDamageWithoutArmor;
                }
                break;
            case BodyRegion.Arms:
                if (victim.Armor > 5)
                {
                    var armsDamage = (int)
                        Utils.RandomizeValue(
                            (Settings.NpcArmorValues.Arms - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"{Functions.GetPersonaForPed(victim).FullName} shot in arms with armor! Damage: {armsDamage}");
                    victim.Health -= armsDamage;
                }
                else
                {
                    var armsDamageWithoutArmor = (int)
                        Utils.RandomizeValue((Settings.NpcValues.Arms - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier);
                    LogUtils.Info(
                        $"{Functions.GetPersonaForPed(victim).FullName} shot in arms without armor! Damage: {armsDamageWithoutArmor}"
                    );
                    victim.Health -= armsDamageWithoutArmor;
                }
                break;
            case BodyRegion.Legs:
                if (new Random(DateTime.Now.Millisecond).Next(0, 3) == 0)
                    victim.Tasks.Ragdoll();
                if (victim.Armor > 5)
                {
                    var legsDamage = (int)
                        Utils.RandomizeValue(
                            (Settings.NpcArmorValues.Legs - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier
                        );
                    LogUtils.Info($"{Functions.GetPersonaForPed(victim).FullName} shot in legs with armor! Damage: {legsDamage}");
                    victim.Health -= legsDamage;
                }
                else
                {
                    var legsDamageWithoutArmor = (int)
                        Utils.RandomizeValue((Settings.NpcValues.Legs - damageInfo.Damage) * Utils.GetWeaponType(weapon).DamageMultiplier);
                    LogUtils.Info(
                        $"{Functions.GetPersonaForPed(victim).FullName} shot in legs without armor! Damage: {legsDamageWithoutArmor}"
                    );
                    victim.Health -= legsDamageWithoutArmor;
                }
                break;
            case BodyRegion.None:
            default:
                LogUtils.Warning("Unknown body region " + damageInfo.BoneInfo.BodyRegion);
                break;
        }

        if (!Settings.Debug)
            return;
        LogUtils.Info("[DEBUG]: Victim health after shot: " + victim.Health);
        LogUtils.Info("[DEBUG]: Victim armor after shot: " + victim.Armor);
    }
}
