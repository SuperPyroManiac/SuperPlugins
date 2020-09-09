using LSPD_First_Response;
using Rage;

namespace DeadlyWeapons2.Modules
{
    internal static class StartPanic
    {
        internal static bool Panic;
        
        internal static void PanicHit()
        {
            if (Panic) return;
            Panic = true;
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
                Panic = false;
            });
        }
    }
}