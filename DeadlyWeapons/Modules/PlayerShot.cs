#region

using System;
using DamageTrackerLib.DamageInfo;
using DeadlyWeapons.DFunctions;
using Rage;

#endregion

namespace DeadlyWeapons.Modules
{
    internal static class PlayerShot
    {
        private static Ped Player => Game.LocalPlayer.Character;

        internal static void OnPlayerDamaged(Ped victim, Ped attacker, PedDamageInfo damageInfo)
        {
            if (Player.IsDead) return;
            if (damageInfo.WeaponInfo.Group != DamageGroup.Bullet) return;
            var rnd = new Random().Next(1, 5);
            if (Settings.EnableDebug)
                Game.DisplayHelp(
                    $"~w~{victim.Model.Name} (~r~{damageInfo.Damage} Dmg~w~) ({(victim.IsAlive ? "~g~Alive" : "~r~Dead")}~w~)" +
                    $"\n~r~{attacker?.Model.Name ?? "None"}" +
                    $"\n~y~{damageInfo.WeaponInfo.Hash.ToString()} {damageInfo.WeaponInfo.Type.ToString()} {damageInfo.WeaponInfo.Group.ToString()}" +
                    $"\n~r~{damageInfo.BoneInfo.BoneId.ToString()} {damageInfo.BoneInfo.Limb.ToString()} {damageInfo.BoneInfo.BodyRegion.ToString()}");

            if (damageInfo.BoneInfo.BodyRegion == BodyRegion.Head && Settings.EnablePlayerHeadshotInstakill)
            {
                Game.LogTrivial("DeadlyWeapons: Player shot in head - killing.");
                Player.Kill();
                return;
            }

            if (damageInfo.BoneInfo.BodyRegion == BodyRegion.Legs)
            {
                var rnd2 = new Random().Next(1, 3);
                Player.Health -= 30;
                Game.LogTrivial("DeadlyWeapons: Player shot in leg - deducting 30 health.");
                if (rnd2 != 2) return;
                SimpleFunctions.Ragdoll(Player);
                Game.LogTrivial("DeadlyWeapons: Player tripped due to leg injury. (50/50 chance)");
                return;
            }

            if (damageInfo.BoneInfo.BodyRegion == BodyRegion.Arms)
            {
                Player.Health -= 30;
                Game.LogTrivial("DeadlyWeapons: Player shot in arm - deducting 30 health.");
                return;
            }

            if (Player.Armor > 5)
            {
                Game.LogTrivial("DeadlyWeapons: Player shot with armor. Rolled " + rnd);
                switch (rnd)
                {
                    case 1:
                        Player.Armor = 0;
                        break;
                    case 2:
                        Player.Health -= 45;
                        Player.Armor = 0;
                        SimpleFunctions.Ragdoll(Player);
                        break;
                    case 3:
                        Player.Armor -= 35;
                        break;
                    case 4:
                        Player.Armor -= 45;
                        break;
                }
            }

            if (Player.Armor <= 5)
            {
                Game.LogTrivial("DeadlyWeapons: Player shot without armor. Rolled " + rnd);
                switch (rnd)
                {
                    case 1:
                        Player.Health = 5;
                        break;
                    case 2:
                        Player.Kill();
                        break;
                    case 3:
                        Player.Health -= 40;
                        break;
                    case 4:
                        Player.Health -= 50;
                        SimpleFunctions.Ragdoll(Player);
                        break;
                }
            }
        }
    }
}