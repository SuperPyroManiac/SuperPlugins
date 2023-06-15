using System;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;

namespace DeadlyWeapons.Modules;

internal static class CustomPullover
{
    internal static void PulloverModule(LHandle handler)
    {
        var bad = Functions.GetPulloverSuspect(handler);
        var checking = true;
        var hasWeapon = false;
        var rNd = new Random().Next(1, 8);
        var checkFiber = new GameFiber(delegate
        {
            while (checking)
            {
                GameFiber.Yield();

                if (!Functions.IsPlayerPerformingPullover() || !bad.Exists())
                {
                    checking = false;
                    return;
                }

                if (Game.LocalPlayer.Character.DistanceTo(bad) < 3f)
                {
                    Log.Info("Pullover detected, using scenario: " + rNd);
                    checking = false;
                    bad.Inventory.Weapons.Clear();
                    if (PyroFunctions.IsWanted(bad))
                        switch (rNd)
                        {
                            case 1:
                                if (!bad.Inventory.HasLoadedWeapon) bad.Inventory.Weapons.Add(WeaponHash.Pistol);
                                hasWeapon = true;
                                break;
                            case 2:
                                if (!bad.Inventory.HasLoadedWeapon) bad.Inventory.Weapons.Add(WeaponHash.Pistol);
                                hasWeapon = true;
                                bad.Tasks.FireWeaponAt(Game.LocalPlayer.Character, -1,
                                    FiringPattern.BurstFirePistol);
                                break;
                            // case 3:
                            //     var pursuit = Functions.CreatePursuit();
                            //     Functions.AddPedToPursuit(pursuit, bad);
                            //     Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                            //     break;
                            default:
                                if (bad.Inventory.HasLoadedWeapon) hasWeapon = true;
                                break;
                        }
                    else
                        switch (rNd)
                        {
                            case 1:
                                if (!bad.Inventory.HasLoadedWeapon) bad.Inventory.Weapons.Add(WeaponHash.Pistol);
                                hasWeapon = true;
                                bad.Metadata.hasGunPermit = false;
                                break;
                            case 2:
                                if (!bad.Inventory.HasLoadedWeapon) bad.Inventory.Weapons.Add(WeaponHash.Pistol);
                                hasWeapon = true;
                                bad.Metadata.hasGunPermit = true;
                                break;
                            // case 3:
                            //     var pursuit = Functions.CreatePursuit();
                            //     Functions.AddPedToPursuit(pursuit, bad);
                            //     Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                            //     break;
                            default:
                                if (bad.Inventory.HasLoadedWeapon) hasWeapon = true;
                                break;
                        }

                    if (hasWeapon)
                    {
                        bad.Metadata.searchPed = "~r~A Firearm~s~, " + bad.Metadata.searchPed;
                        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Traffic Stop",
                            "~r~Weapon Spotted",
                            "You noticed the suspect has a weapon in the vehicle!");
                    }
                }
            }
        });
        checkFiber.Start();
    }
}