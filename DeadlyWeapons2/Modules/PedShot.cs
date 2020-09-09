using System;
using DeadlyWeapons2.DFunctions;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;

namespace DeadlyWeapons2.Modules
{
    internal class PedShot
    {
        internal static void PedAi(Ped ped)
        {
            try
            {
                GameFiber.StartNew(delegate
                {
                    if (!ped || ped.IsDead) return;
                    ped.Accuracy = Settings.AiAccuracy;
                    ped.FiringPattern = FiringPattern.DelayFireByOneSecond;

                    foreach (var w in WeaponHashs.WeaponHashes)
                    {
                        if (RubberBullet.NonLeathal)
                        {
                            if (NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON<bool>(ped, (uint) w, 0) &&
                                Settings.EnableDamageSystem)
                            {
                                if (ped.LastDamageBone == PedBoneId.LeftUpperArm || ped.LastDamageBone == PedBoneId.LeftForeArm || ped.LastDamageBone == PedBoneId.RightUpperArm || ped.LastDamageBone == PedBoneId.RightForearm)
                                {
                                    if (ped.Inventory.HasLoadedWeapon)
                                    {
                                        ped.Inventory.EquippedWeapon.Drop();
                                    }
                                }
                                if (Game.LocalPlayer.Character.DistanceTo(ped) < 6)
                                {
                                    var rnd = new Random().Next(0, 3);
                                    switch (rnd)
                                    {
                                        case 0:
                                            ped.Kill();
                                            break;
                                        default:
                                            var rndd = new Random().Next(0, 6);
                                            switch (rndd)
                                            {
                                                case 0:
                                                    ped.Health = 100;
                                                    SimpleFunctions.Ragdoll(ped);
                                                    GameFiber.Wait(3000);
                                                    ped.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                                                    break;
                                                case 1:
                                                    ped.Health = 100;
                                                    SimpleFunctions.Ragdoll(ped);
                                                    GameFiber.Wait(3000);
                                                    ped.Tasks.Cower(-1);
                                                    break;
                                                default:
                                                    ped.Health = 100;
                                                    SimpleFunctions.Ragdoll(ped);
                                                    break;
                                            }
                                            break;
                                    }
                                }
                                else
                                {
                                    var rnd = new Random().Next(0, 6);
                                    switch (rnd)
                                    {
                                        case 0:
                                            ped.Health = 100;
                                            SimpleFunctions.Ragdoll(ped);
                                            GameFiber.Wait(3000);
                                            ped.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                                            break;
                                        case 1:
                                            ped.Health = 100;
                                            SimpleFunctions.Ragdoll(ped);
                                            GameFiber.Wait(3000);
                                            ped.Tasks.Cower(-1);
                                            break;
                                        default:
                                            ped.Health = 100;
                                            SimpleFunctions.Ragdoll(ped);
                                            break;
                                    }
                                }
                            }
                            NativeFunction.Natives.CLEAR_ENTITY_LAST_WEAPON_DAMAGE(ped);
                        }
                        else
                        {
                            if (NativeFunction.Natives.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON<bool>(ped, (uint) w, 0) &&
                                Settings.EnableDamageSystem)
                            {
                                if (ped.LastDamageBone == PedBoneId.LeftUpperArm || ped.LastDamageBone == PedBoneId.LeftForeArm || ped.LastDamageBone == PedBoneId.RightUpperArm || ped.LastDamageBone == PedBoneId.RightForearm)
                                {
                                    if (ped.Inventory.HasLoadedWeapon)
                                    {
                                        ped.Inventory.EquippedWeapon.Drop();
                                    }
                                }
                                if (ped.Armor >= 60)
                                {
                                    var rnd = new Random().Next(0, 10);
                                    switch (rnd)
                                    {
                                        case 1:
                                            ped.Health = 100;
                                            ped.Armor = 61;
                                            break;
                                        case 2:
                                            ped.Health = 100;
                                            ped.Armor = 61;
                                            SimpleFunctions.Ragdoll(ped);
                                            break;
                                        case 3:
                                            ped.Health = 80;
                                            ped.Armor = 0;
                                            PedCustomAI.PedReact(ped);
                                            break;
                                        default:
                                            ped.Health = 100;
                                            ped.Armor = 0;
                                            break;
                                    }

                                    Game.LogTrivial("Deadly Weapons: " + Functions.GetPersonaForPed(ped).FullName +
                                                    " rolled 1-" + rnd);
                                }
                                else
                                {
                                    var rnd = new Random().Next(0, 10);
                                    switch (rnd)
                                    {
                                        case 1:
                                            ped.Health -= 50;
                                            SimpleFunctions.Ragdoll(ped);
                                            break;
                                        case 2:
                                            goto case 1;
                                        case 3:
                                            goto case 1;
                                        case 4:
                                            ped.Kill();
                                            break;
                                        default:
                                            ped.Health -= 80;
                                            PedCustomAI.PedReact(ped);
                                            break;
                                    }

                                    Game.LogTrivial("Deadly Weapons: " + Functions.GetPersonaForPed(ped).FullName +
                                                    " rolled 2-" + rnd);
                                }

                                NativeFunction.Natives.CLEAR_ENTITY_LAST_WEAPON_DAMAGE(ped);
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
                Game.LogTrivial("Deadly Weapons Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("Deadly Weapons Error Report End");
            }
        }
    }
}