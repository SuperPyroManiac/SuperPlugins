#region

using System;
using DamageTrackerLib.DamageInfo;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;

#endregion

namespace DeadlyWeapons.Modules;

internal static class PedShot
{
    internal static void OnPedDamaged(Ped victim, Ped attacker, PedDamageInfo damageInfo)
    {
        if (!victim.Exists()) return;
        if ( victim == Game.LocalPlayer.Character ) return;
        if (damageInfo.WeaponInfo.Group != DamageGroup.Bullet) return;
        var rnd = new Random(DateTime.Now.Millisecond).Next(1, 5);
        if (Settings.EnableDebug)
        {
            Log.Info("[DEBUG]: Detailed damage info Start");
            Log.Info(
                $"\n{victim.Model.Name} ({damageInfo.Damage} Dmg) ({(victim.IsAlive ? "Alive" : "Dead")})" +
                $"\n{attacker?.Model.Name ?? "None"}" +
                $"\n{damageInfo.WeaponInfo.Hash.ToString()} {damageInfo.WeaponInfo.Type.ToString()} {damageInfo.WeaponInfo.Group.ToString()}" +
                $"\n{damageInfo.BoneInfo.BoneId.ToString()} {damageInfo.BoneInfo.Limb.ToString()} {damageInfo.BoneInfo.BodyRegion.ToString()}");
            Log.Info("[DEBUG]: Detailed damage info Stop");
            Log.Info("[DEBUG]: " + Functions.GetPersonaForPed(victim).FullName + "'s health before shot: " + victim.Health);
            Log.Info("[DEBUG]: " + Functions.GetPersonaForPed(victim).FullName + "'s armor before shot: " + victim.Armor);
        }
        
        if (damageInfo.BoneInfo.BodyRegion == BodyRegion.Head)
        {
            Log.Info("" + Functions.GetPersonaForPed(victim).FullName +
                            " shot in head - killing.");
            victim.Kill();
            return;
        }

        if (damageInfo.BoneInfo.BodyRegion == BodyRegion.Legs)
        {
            var rnd2 = new Random(DateTime.Now.Millisecond).Next(1, 3);
            victim.Health -= 30;
            Log.Info(Functions.GetPersonaForPed(victim).FullName + " shot in leg - deducting 30 health.");
            if (rnd2 == 2) victim.Tasks.Ragdoll();
            Log.Info(Functions.GetPersonaForPed(victim).FullName + " tripped due to leg injury. (50/50 chance)");
            if (Settings.EnableDebug)
            {
                Log.Info("[DEBUG]: " + Functions.GetPersonaForPed(victim).FullName + "'s health after shot: " + victim.Health);
                Log.Info("[DEBUG]: " + Functions.GetPersonaForPed(victim).FullName + "'s armor after shot: " + victim.Armor);
            }
            return;
        }

        if (damageInfo.BoneInfo.BodyRegion == BodyRegion.Arms)
        {
            new Random(DateTime.Now.Millisecond).Next(1, 3);
            victim.Health -= 30;
            Log.Info("" + Functions.GetPersonaForPed(victim).FullName +
                            " shot in arm - deducting 30 health.");
            if (Settings.EnableDebug)
            {
                Log.Info("[DEBUG]: " + Functions.GetPersonaForPed(victim).FullName + "'s health after shot: " + victim.Health);
                Log.Info("[DEBUG]: " + Functions.GetPersonaForPed(victim).FullName + "'s armor after shot: " + victim.Armor);
            }
            return;
        }

        if (victim.Armor > 5)
        {
            Log.Info("" + Functions.GetPersonaForPed(victim).FullName +
                            " shot with armor. Rolled " + rnd);
            switch (rnd)
            {
                case 1:
                    victim.Armor = 0;
                    break;
                case 2:
                    victim.Health -= 45;
                    victim.Armor = 0;
                    victim.Tasks.Ragdoll();
                    break;
                case 3:
                    victim.Armor -= 35;
                    break;
                case 4:
                    victim.Armor -= 45;
                    break;
            }
            if (Settings.EnableDebug)
            {
                Log.Info("[DEBUG]: " + Functions.GetPersonaForPed(victim).FullName + "'s health after shot: " + victim.Health);
                Log.Info("[DEBUG]: " + Functions.GetPersonaForPed(victim).FullName + "'s armor after shot: " + victim.Armor);
            }
        }

        if (victim.Armor <= 5)
        {
            Log.Info("" + Functions.GetPersonaForPed(victim).FullName +
                            " shot without armor. Rolled " + rnd);
            switch (rnd)
            {
                case 1:
                    victim.Health = 5;
                    break;
                case 2:
                    victim.Kill();
                    break;
                case 3:
                    victim.Health -= 40;
                    break;
                case 4:
                    victim.Health -= 50;
                    victim.Tasks.Ragdoll();
                    break;
            }
            if (Settings.EnableDebug)
            {
                Log.Info("[DEBUG]: " + Functions.GetPersonaForPed(victim).FullName + "'s health after shot: " + victim.Health);
                Log.Info("[DEBUG]: " + Functions.GetPersonaForPed(victim).FullName + "'s armor after shot: " + victim.Armor);
            }
        }
    }
}