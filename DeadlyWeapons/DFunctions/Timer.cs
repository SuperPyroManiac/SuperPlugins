#region

using System;
using System.Runtime.InteropServices;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using Rage;

#endregion

namespace DeadlyWeapons.DFunctions
{
    internal static class Timer
    {
        private static bool _panic;
        internal static bool NonLeathal { get; set; }

        internal static void Ragdoll(Ped ped)
                 {
                     try
                     {
                         GameFiber.StartNew(delegate
                         {
                             if (!ped) return;
                             ped.IsRagdoll = true;
                             GameFiber.Wait(2000);
                             if (!ped) return;
                             ped.IsRagdoll = false;
                         });
                     }
                     catch (Exception e)
                     {
                         Game.LogTrivial("Deadly Weapons: Unable to remove ragdoll due to player death.");
                     }
                 }

        internal static void VisualSearch(LHandle handler)
        {
            var bad = Functions.GetPulloverSuspect(handler);
            var checking = true;
            var hasWeapon = false;
            var rND = new Random().Next(1,8);
            var checkFiber = new GameFiber(delegate
            {
                while (checking)
                {
                    GameFiber.Yield();
                    
                    if (!Functions.IsPlayerPerformingPullover())
                    {
                        checking = false;
                    }

                    if (Game.LocalPlayer.Character.DistanceTo(bad) < 3f)
                    {
                        Game.LogTrivial("DeadlyWeapons: Pullover detected, using scenario: " + rND);
                        checking = false;
                        bad.Inventory.Weapons.Clear();
                        if (DFunctions.CFunctions.IsWanted(bad))
                        {
                            switch (rND)
                            {
                                case 1:
                                    bad.Inventory.Weapons.Add(WeaponHash.Pistol);
                                    hasWeapon = true;
                                    break;
                                case 2:
                                    bad.Inventory.Weapons.Add(WeaponHash.Pistol);
                                    hasWeapon = true;
                                    bad.Tasks.FireWeaponAt(Game.LocalPlayer.Character, -1,
                                        FiringPattern.BurstFirePistol);
                                    break;
                                case 3:
                                    var pursuit = Functions.CreatePursuit();
                                    Functions.AddPedToPursuit(pursuit, bad);
                                    Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else                                                                               //TODO: ADD WEAPONS TO PED SEARCH
                        {
                            switch (rND)
                            {
                                case 1:
                                    bad.Inventory.Weapons.Add(WeaponHash.Pistol);
                                    hasWeapon = true;
                                    bad.Metadata.hasGunPermit = false;
                                    break;
                                case 2:
                                    bad.Inventory.Weapons.Add(WeaponHash.Pistol);
                                    hasWeapon = true;
                                    bad.Metadata.hasGunPermit = true;
                                    break;
                                case 3:
                                    var pursuit = Functions.CreatePursuit();
                                    Functions.AddPedToPursuit(pursuit, bad);
                                    Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (hasWeapon)
                        {
                            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Traffic Stop", "~r~Weapon Spotted",
                                "You noticed the suspect has a weapon in the vehicle!");
                        }
                    }
                }
            });
            checkFiber.Start();
        }

        internal static void RubberBullets()
        {
            NonLeathal = !NonLeathal;
            Game.DisplayHelp("Using Non Lethal Ammo:~y~ " + NonLeathal);
        }

        internal static void Panic()
        {
            if (_panic) return;
            _panic = true;
            GameFiber.StartNew(delegate
            {
                if (Settings.Code3Backup)
                {
                    LSPD_First_Response.Mod.API.Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.LocalUnit);
                }
                if (Settings.SwatBackup)
                {
                    LSPD_First_Response.Mod.API.Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.SwatTeam);
                }
                if (Settings.NooseBackup)
                {
                    LSPD_First_Response.Mod.API.Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.NooseTeam);
                }

                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Shots Fired", "~y~Panic Activated",
                    "Your weapon has been discharged. Dispatch has been alerted.");
                GameFiber.Wait(Settings.PanicCooldown * 1000);
                _panic = false;
            });
        }

        internal static void PedReact(Ped ped)
        {
            GameFiber.StartNew(delegate
            {
                var rnd = new Random().Next(0, 8);
                switch (rnd)
                {
                    case 0:
                        Game.LogTrivial("Deadly Weapons: " +
                                        LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(ped).FullName +
                                        " is fleeing!");
                        ped.BlockPermanentEvents = true;
                        ped.IsPersistent = true;
                        ped.Tasks.ClearImmediately();
                        ped.Tasks.Flee(Game.LocalPlayer.Character, 120, 20000);
                        GameFiber.Wait(15000);
                        if (ped)
                        {
                            ped.BlockPermanentEvents = false;
                            ped.IsPersistent = false;
                        }

                        break;
                    case 1:
                        Game.LogTrivial("Deadly Weapons: " +
                                        LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(ped).FullName +
                                        " is hiding!");
                        ped.BlockPermanentEvents = true;
                        ped.IsPersistent = true;
                        ped.Tasks.ClearImmediately();
                        ped.Tasks.TakeCoverFrom(Game.LocalPlayer.Character, 20000, false);
                        GameFiber.Wait(15000);
                        if (ped)
                        {
                            ped.BlockPermanentEvents = false;
                            ped.IsPersistent = false;
                        }

                        break;
                    case 2:
                        Game.LogTrivial("Deadly Weapons: " +
                                        LSPD_First_Response.Mod.API.Functions.GetPersonaForPed(ped).FullName +
                                        " is cowering!");
                        ped.BlockPermanentEvents = true;
                        ped.IsPersistent = true;
                        ped.Tasks.ClearImmediately();
                        ped.Tasks.Cower(20000);
                        GameFiber.Wait(15000);
                        if (ped)
                        {
                            ped.BlockPermanentEvents = false;
                            ped.IsPersistent = false;
                        }
                        
                        break;
                }
            });
        }
    }
}