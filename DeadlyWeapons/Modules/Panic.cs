using System;
using DeadlyWeapons.PyroFunctions;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
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
            if (Settings.Code3Backup) Backup.Request(Enums.BackupType.Code3);
            if (Settings.SwatBackup) Backup.Request(Enums.BackupType.Swat);
            if (Settings.NooseBackup) Backup.Request(Enums.BackupType.Noose);

            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Shots Fired", "~y~Panic Activated", "Your weapon has been discharged. Dispatch has been alerted.");
            GameFiber.Wait(Settings.PanicCooldown * 1000);
            _panic = false;
        });
    }
}