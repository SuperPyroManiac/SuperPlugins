using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using Rage;

namespace DeadlyWeapons
{
    internal static class Timer
    {
        private static bool _panic;
        internal static void Ragdoll(Ped ped)
        {
            GameFiber.StartNew(delegate
            {
                ped.IsRagdoll = true;
                GameFiber.Wait(2000);
                ped.IsRagdoll = false;
            });
        }

        internal static void Panic()
        {
            if (_panic) return;
            _panic = true;
            GameFiber.StartNew(delegate
            {
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                    EBackupUnitType.LocalUnit);
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Shots Fired", "~y~Panic Activated", "Your weapon has been discharged. Dispatch has been alerted.");

                GameFiber.Wait(150000);
                _panic = false;
            });
        }
    }
}