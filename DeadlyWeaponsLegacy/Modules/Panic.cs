#region

using System;
using System.Linq;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using Rage;

#endregion

namespace DeadlyWeaponsLegacy.Modules
{
    internal static class StartPanic
    {
        private static bool _panic;
        private static readonly Func<string, bool> IsLoaded = plugName =>
            Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(plugName));

        private static readonly bool UsingUb = IsLoaded("UltimateBackup");

        internal static void PanicHit()
        {
            if (_panic) return;
            _panic = true;
            if (UsingUb) Game.LogTrivial("DeadlyWeapons: UB DETECTED. Using Ultimate Backup for panic.");
            GameFiber.StartNew(delegate
            {
                if (Settings.Code3Backup)
                {
                    if (UsingUb) 
                    {
                        Wrapper.CallCode3();
                    }
                    else
                    {
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                            EBackupResponseType.Code3,
                            EBackupUnitType.LocalUnit);
                    }
                }

                if (Settings.SwatBackup)
                {
                    if (UsingUb) 
                    {
                        Wrapper.CallSwat();
                    }
                    else
                    {
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                            EBackupResponseType.Code3,
                            EBackupUnitType.SwatTeam);
                    }
                }

                if (Settings.NooseBackup)
                {
                    if (UsingUb) 
                    {
                        Wrapper.CallNoose();
                    }
                    else
                    {
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                            EBackupResponseType.Code3,
                            EBackupUnitType.NooseTeam);
                    }
                }

                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Shots Fired", "~y~Panic Activated",
                    "Your weapon has been discharged. Dispatch has been alerted.");
                GameFiber.Wait(Settings.PanicCooldown * 1000);
                _panic = false;
            });
        }
    }
}