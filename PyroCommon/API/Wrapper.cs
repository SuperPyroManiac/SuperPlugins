using UltimateBackup.API;

namespace PyroCommon.API;

internal static class Wrapper
{
    //ULTIMATE BACKUP
    internal static void CallCode3()
    {
        Functions.callCode3Backup(false);
    }

    internal static void CallCode2()
    {
        Functions.callCode2Backup(false);
    }

    internal static void CallSwat(bool noose)
    {
        Functions.callCode3SwatBackup(false, noose);
    }

    internal static void CallPursuit()
    {
        Functions.callPursuitBackup(false);
    }

    internal static void CallFd()
    {
        Functions.callFireDepartment();
    }

    internal static void CallEms()
    {
        Functions.callAmbulance();
    }
}