namespace SuperCallouts.SimpleFunctions
{
    internal static class Wrapper
    {
        //ULTIMATE BACKUP
        internal static void CallCode3()
        {
            UltimateBackup.API.Functions.callCode3Backup(false);
        }

        internal static void CallCode2()
        {
            UltimateBackup.API.Functions.callCode2Backup(false);
        }

        internal static void CallSwat(bool noose)
        {
            UltimateBackup.API.Functions.callCode3SwatBackup(false, noose);
        }

        internal static void CallPursuit()
        {
            UltimateBackup.API.Functions.callPursuitBackup(false);
        }

        internal static void CallFd()
        {
            UltimateBackup.API.Functions.callFireDepartment();
        }

        internal static void CallEms()
        {
            UltimateBackup.API.Functions.callAmbulance();
        }
    }
}