#region

using System;
using System.Linq;
using DeadlyWeapons.DFunctions;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using Rage;

#endregion

namespace DeadlyWeapons.Modules
{
    internal static class Panic
    {
        private static bool _panic;

        private static readonly Func<string, bool> IsLoaded = plugName =>
            Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(plugName));
        private static readonly bool UsingUb = IsLoaded("UltimateBackup");
        private static Ped Player => Game.LocalPlayer.Character;

        internal static void StartPanicWatch()
        {
            Game.LogTrivial("DeadlyWeapons: Starting PanicFiber.");
            while (true)
            {
                GameFiber.Yield();
                if (Player.IsInAnyVehicle(true)) continue;
                if (Player.IsShooting && (DamageTrackerLib.DamageInfo.WeaponHash)Player.Inventory.EquippedWeapon.Hash != DamageTrackerLib.DamageInfo.WeaponHash.Stun_Gun &&
                    Player.Inventory.EquippedWeapon.Hash != WeaponHash.FireExtinguisher &&
                    Player.Inventory.EquippedWeapon.Hash != WeaponHash.Flare && Settings.EnablePanic)
                    PanicHit();
            }
            // ReSharper disable once FunctionNeverReturns
        }
        private static void PanicHit()
        {
            if (_panic) return;
            if (Settings.EnableDebug) Game.LogTrivial("DeadlyWeapons: [DEBUG]: Panic has been activated! Waiting cooldown to activate again: " + Settings.PanicCooldown * 1000 + " seconds.");
            _panic = true;
            if (UsingUb) Game.LogTrivial("DeadlyWeapons: Using Ultimate Backup for panic.");
            GameFiber.StartNew(delegate
            {
                if (Settings.Code3Backup)
                {
                    if (UsingUb)
                        Wrapper.CallCode3();
                    else
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                            EBackupResponseType.Code3,
                            EBackupUnitType.LocalUnit);
                }

                if (Settings.SwatBackup)
                {
                    if (UsingUb)
                        Wrapper.CallSwat();
                    else
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                            EBackupResponseType.Code3,
                            EBackupUnitType.SwatTeam);
                }

                if (Settings.NooseBackup)
                {
                    if (UsingUb)
                        Wrapper.CallNoose();
                    else
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                            EBackupResponseType.Code3,
                            EBackupUnitType.NooseTeam);
                }

                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Shots Fired", "~y~Panic Activated",
                    "Your weapon has been discharged. Dispatch has been alerted.");
                GameFiber.Wait(Settings.PanicCooldown * 1000);
                _panic = false;
                Game.LogTrivial("DeadlyWeapons: [DEBUG]: Panic cooldown complete. Ready to run again!");
            });
        }
    }
}