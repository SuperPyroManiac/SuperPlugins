#region

using System;
using DamageTrackerLib.DamageInfo;
using DeadlyWeapons.DFunctions;
using LSPD_First_Response.Mod.API;
using Rage;

#endregion

namespace DeadlyWeapons.Modules
{
    internal static class PedShot
    {
        internal static void OnPedDamaged(Ped victim, Ped attacker, PedDamageInfo damageInfo)
        {
            if (!victim.Exists()) return;
            if (victim.IsDead) return;
            if (damageInfo.WeaponInfo.Group != DamageGroup.Bullet) return;
            var rnd = new Random().Next(1, 5);
            if (Settings.EnableDebug)
                Game.DisplayHelp(
                    $"~w~{victim.Model.Name} (~r~{damageInfo.Damage} Dmg~w~) ({(victim.IsAlive ? "~g~Alive" : "~r~Dead")}~w~)" +
                    $"\n~r~{attacker?.Model.Name ?? "None"}" +
                    $"\n~y~{damageInfo.WeaponInfo.Hash.ToString()} {damageInfo.WeaponInfo.Type.ToString()} {damageInfo.WeaponInfo.Group.ToString()}" +
                    $"\n~r~{damageInfo.BoneInfo.BoneId.ToString()} {damageInfo.BoneInfo.Limb.ToString()} {damageInfo.BoneInfo.BodyRegion.ToString()}");

            if (damageInfo.BoneInfo.BodyRegion == BodyRegion.Head)
            {
                Game.LogTrivial("DeadlyWeapons: " + Functions.GetPersonaForPed(victim).FullName +
                                " shot in head - killing.");
                victim.Kill();
                return;
            }

            if (damageInfo.BoneInfo.BodyRegion == BodyRegion.Legs)
            {
                var rnd2 = new Random().Next(1, 3);
                victim.Health -= 30;
                Game.LogTrivial("DeadlyWeapons: " + Functions.GetPersonaForPed(victim).FullName +
                                " shot in leg - deducting 30 health.");
                if (rnd2 != 2) return;
                SimpleFunctions.Ragdoll(victim);
                Game.LogTrivial("DeadlyWeapons: " + Functions.GetPersonaForPed(victim).FullName +
                                " tripped due to leg injury. (50/50 chance)");
                return;
            }

            if (damageInfo.BoneInfo.BodyRegion == BodyRegion.Arms)
            {
                var rnd2 = new Random().Next(1, 3);
                victim.Health -= 30;
                Game.LogTrivial("DeadlyWeapons: " + Functions.GetPersonaForPed(victim).FullName +
                                " shot in arm - deducting 30 health.");
                return;
            }

            if (victim.Armor > 5)
            {
                Game.LogTrivial("DeadlyWeapons: " + Functions.GetPersonaForPed(victim).FullName +
                                " shot with armor. Rolled " + rnd);
                switch (rnd)
                {
                    case 1:
                        victim.Armor = 0;
                        break;
                    case 2:
                        victim.Health -= 45;
                        victim.Armor = 0;
                        SimpleFunctions.Ragdoll(victim);
                        break;
                    case 3:
                        victim.Armor -= 35;
                        break;
                    case 4:
                        victim.Armor -= 45;
                        break;
                }
            }

            if (victim.Armor <= 5)
            {
                Game.LogTrivial("DeadlyWeapons: " + Functions.GetPersonaForPed(victim).FullName +
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
                        SimpleFunctions.Ragdoll(victim);
                        break;
                }
            }
        }
    }
}