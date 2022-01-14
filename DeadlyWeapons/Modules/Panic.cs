#region

using System;
using System.Linq;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using Rage;

#endregion

namespace DeadlyWeapons.Modules
{
    internal static class StartPanic
    {
        private static bool _panic;

        private static readonly Func<string, bool> IsLoaded = PlugName =>
            Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(PlugName));

        internal static void PanicHit()
        {
            if (_panic) return;
            _panic = true;
            GameFiber.StartNew(delegate
            {
                if (Settings.Code3Backup)
                {
                    if (IsLoaded("UltimateBackup")) 
                    {
                        UltimateBackup.API.Functions.callCode3Backup(false);
                        return;
                    }
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.LocalUnit);
                }

                if (Settings.SwatBackup)
                {
                    if (IsLoaded("UltimateBackup")) 
                    {
                        UltimateBackup.API.Functions.callCode3SwatBackup(false, false);
                        return;
                    }
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.SwatTeam);
                }

                if (Settings.NooseBackup)
                {
                    if (IsLoaded("UltimateBackup")) 
                    {
                        UltimateBackup.API.Functions.callCode3SwatBackup(false, true);
                        return;
                    }
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.NooseTeam);
                }

                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Shots Fired", "~y~Panic Activated",
                    "Your weapon has been discharged. Dispatch has been alerted.");
                GameFiber.Wait(Settings.PanicCooldown * 1000);
                _panic = false;
            });
        }
    }
}