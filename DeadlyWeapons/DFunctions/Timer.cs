#region

using System;
using LSPD_First_Response;
using Rage;

#endregion

namespace DeadlyWeapons.DFunctions
{
    internal static class Timer
    {
        private static bool _panic;

        internal static void Ragdoll(Ped ped)
        {
            try
            {
                GameFiber.StartNew(delegate
                {
                    ped.IsRagdoll = true;
                    GameFiber.Wait(2000);
                    ped.IsRagdoll = false;
                });
            }
            catch (Exception e)
            {
                Game.LogTrivial("Deadly Weapons: Unable to remove ragdoll due to player death.");
            }
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