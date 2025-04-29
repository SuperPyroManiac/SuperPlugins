using DeadlyWeapons.PyroFunctions;
using PyroCommon.Models;
using PyroCommon.Utils;
using Rage;

namespace DeadlyWeapons.Modules;

internal static class Panic
{
    private static bool _panic;

    private static Ped Player => Game.LocalPlayer.Character;

    internal static void StartPanicFiber()
    {
        LogUtils.Info("Starting PanicFiber.");
        while (Main.Running)
        {
            GameFiber.Yield();
            if (Player.IsInAnyVehicle(true))
                continue;
            if (Player.IsShooting && Settings.Panic && !Utils.GetWeaponByHash(Player.Inventory.EquippedWeapon.Hash).PanicIgnore)
                PanicHit();
            if (Settings.Debug && Player.IsShooting && Settings.Panic)
                LogUtils.Info(
                    $"[DEBUG] Weapon fired: ({Player.Inventory.EquippedWeapon.Hash.ToString()}) "
                        + $"Best DW Match: ({Utils.GetWeaponByHash(Player.Inventory.EquippedWeapon.Hash).Name}: {Utils.GetWeaponByHash(Player.Inventory.EquippedWeapon.Hash).WeaponHash})"
                );
        }
    }

    private static void PanicHit()
    {
        if (_panic)
            return;
        _panic = true;
        if (PyroCommon.Main.UsingUb)
            LogUtils.Info("Using Ultimate Backup for panic.");
        GameFiber.StartNew(
            delegate
            {
                if (Settings.Code3Backup)
                    CommonUtils.RequestBackup(Enums.BackupType.Code3);
                if (Settings.SwatBackup)
                    CommonUtils.RequestBackup(Enums.BackupType.Swat);
                if (Settings.NooseBackup)
                    CommonUtils.RequestBackup(Enums.BackupType.Noose);

                Game.DisplayNotification(
                    "3dtextures",
                    "mpgroundlogo_cops",
                    "~r~Shots Fired",
                    "~y~Panic Activated",
                    "Your weapon has been discharged. Dispatch has been alerted."
                );
                GameFiber.Wait(Settings.PanicCooldown * 1000);
                _panic = false;
            }
        );
    }
}
