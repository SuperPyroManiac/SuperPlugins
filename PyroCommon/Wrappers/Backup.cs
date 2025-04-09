using UltimateBackup.API;

namespace PyroCommon.Wrappers;

internal static class Backup
{
    //ULTIMATE BACKUP
    internal static void UbCode3()
    {
        Functions.callCode3Backup(false);
    }

    internal static void UbCode2()
    {
        Functions.callCode2Backup(false);
    }

    internal static void UbSwat(bool noose)
    {
        Functions.callCode3SwatBackup(false, noose);
    }

    internal static void UbPursuit()
    {
        Functions.callPursuitBackup(false);
    }

    internal static void UbFd()
    {
        Functions.callFireDepartment();
    }

    internal static void UbEms()
    {
        Functions.callAmbulance();
    }
}
