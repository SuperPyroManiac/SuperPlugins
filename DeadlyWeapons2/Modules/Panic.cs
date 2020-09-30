#region

using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using Rage;

#endregion

namespace DeadlyWeapons2.Modules
{
    internal static class StartPanic
    {
        private static bool _panic;

        internal static void PanicHit()
        {
            if (_panic) return;
            _panic = true;
            GameFiber.StartNew(delegate
            {
                if (Settings.Code3Backup)
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.LocalUnit);
                if (Settings.SwatBackup)
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.SwatTeam);
                if (Settings.NooseBackup)
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.NooseTeam);

                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Shots Fired", "~y~Panic Activated",
                    "Your weapon has been discharged. Dispatch has been alerted.");
                GameFiber.Wait(Settings.PanicCooldown * 1000);
                _panic = false;
            });
        }
    }
}