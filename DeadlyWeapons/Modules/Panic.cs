using System;
using DeadlyWeapons.PyroFunctions;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using PyroCommon.API;
using Rage;

namespace DeadlyWeapons.Modules;

internal static class Panic
{
    private static bool _panic;

    private static Ped Player => Game.LocalPlayer.Character;

    internal static void StartPanicFiber()
    {
        Log.Info("Starting PanicFiber.");
        while (Main.Running)
        {
            try
            {
                GameFiber.Yield();
                if (Player.IsInAnyVehicle(true)) continue;
                if ( Player.IsShooting && Settings.Panic && !Utils.GetWeaponByHash(Player.Inventory.EquippedWeapon.Hash).PanicIgnore) PanicHit();
                if ( Settings.Debug && Player.IsShooting && Settings.Panic ) Log.Info($"[DEBUG] Weapon fired: ({Player.Inventory.EquippedWeapon.Hash.ToString()}) Best DW Match: ({Utils.GetWeaponByHash(Player.Inventory.EquippedWeapon.Hash).Name}: {Utils.GetWeaponByHash(Player.Inventory.EquippedWeapon.Hash).WeaponHash})");
            }
            catch ( Exception e )
            {
                Log.Error(e.Message);
            }
        }
    }
    private static void PanicHit()
    {
        if (_panic) return;
        _panic = true;
        if (PyroCommon.Main.UsingUb) Log.Info("Using Ultimate Backup for panic.");
        GameFiber.StartNew(delegate
        {
            if (Settings.Code3Backup)
            {
                if (PyroCommon.Main.UsingUb)
                    Wrapper.CallCode3();
                else
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.LocalUnit);
            }

            if (Settings.SwatBackup)
            {
                if (PyroCommon.Main.UsingUb)
                    Wrapper.CallSwat(false);
                else
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.SwatTeam);
            }

            if (Settings.NooseBackup)
            {
                if (PyroCommon.Main.UsingUb)
                    Wrapper.CallSwat(true);
                else
                    Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                        EBackupResponseType.Code3,
                        EBackupUnitType.NooseTeam);
            }

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Shots Fired", "~y~Panic Activated", "Your weapon has been discharged. Dispatch has been alerted.");
            GameFiber.Wait(Settings.PanicCooldown * 1000);
            _panic = false;
        });
    }
}