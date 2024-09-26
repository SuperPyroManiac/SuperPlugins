using System;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using PyroCommon.Objects;
using Rage;

namespace PyroCommon.PyroFunctions;

public static class Backup
{
    private static readonly Vector3 PlayerPos = Game.LocalPlayer.Character.Position;
    
    public static void Request(Enums.BackupType bType)
    {
        switch (bType)
        {
            case Enums.BackupType.Code2:
                if ( Main.UsingUb ) Wrappers.Backup.UbCode2();
                else Functions.RequestBackup(PlayerPos, EBackupResponseType.Code2, EBackupUnitType.LocalUnit);
                break;
            case Enums.BackupType.Code3:
                if (Main.UsingUb) Wrappers.Backup.UbCode3();
                else Functions.RequestBackup(PlayerPos, EBackupResponseType.Code3, EBackupUnitType.LocalUnit);
                break;
            case Enums.BackupType.Swat:
                if (Main.UsingUb) Wrappers.Backup.UbSwat(false);
                else Functions.RequestBackup(PlayerPos, EBackupResponseType.Code3, EBackupUnitType.SwatTeam);
                break;
            case Enums.BackupType.Noose:
                if (Main.UsingUb) Wrappers.Backup.UbSwat(true);
                else Functions.RequestBackup(PlayerPos, EBackupResponseType.Code3, EBackupUnitType.NooseTeam);
                break;
            case Enums.BackupType.Fire:
                if (Main.UsingUb) Wrappers.Backup.UbFd();
                else Functions.RequestBackup(PlayerPos, EBackupResponseType.Code3, EBackupUnitType.Firetruck);
                break;
            case Enums.BackupType.Medical:
                if (Main.UsingUb) Wrappers.Backup.UbEms();
                else Functions.RequestBackup(PlayerPos, EBackupResponseType.Code3, EBackupUnitType.Ambulance);
                break;
            case Enums.BackupType.Pursuit:
                if (Main.UsingUb) Wrappers.Backup.UbPursuit();
                else Functions.RequestBackup(PlayerPos, EBackupResponseType.Pursuit, EBackupUnitType.LocalUnit);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bType), bType, null);
        }
    }
}