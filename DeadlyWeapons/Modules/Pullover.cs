#region

using System;
using DeadlyWeapons.DFunctions;
using LSPD_First_Response.Mod.API;
using Rage;

#endregion

namespace DeadlyWeapons.Modules
{
    internal static class Pullover
    {
        internal static void PulloverModule(LHandle handler)
        {
            var bad = Functions.GetPulloverSuspect(handler);
            var checking = true;
            var hasWeapon = false;
            var rND = new Random().Next(1, 8);
            var checkFiber = new GameFiber(delegate
            {
                while (checking)
                {
                    GameFiber.Yield();

                    if (!Functions.IsPlayerPerformingPullover()) checking = false;

                    if (Game.LocalPlayer.Character.DistanceTo(bad) < 3f)
                    {
                        Game.LogTrivial("DeadlyWeapons: Pullover detected, using scenario: " + rND);
                        checking = false;
                        bad.Inventory.Weapons.Clear();
                        if (SimpleFunctions.IsWanted(bad))
                            switch (rND)
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
                                /*case 3:
                                    var pursuit = Functions.CreatePursuit();
                                    Functions.AddPedToPursuit(pursuit, bad);
                                    Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                                    break;*/
                                    //TODO
                                default:
                                    if (bad.Inventory.HasLoadedWeapon) hasWeapon = true;
                                    break;
                            }
                        else
                            switch (rND)
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
                                /*case 3:
                                    var pursuit = Functions.CreatePursuit();
                                    Functions.AddPedToPursuit(pursuit, bad);
                                    Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                                    break;*/
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
}