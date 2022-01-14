namespace SuperCallouts.SimpleFunctions
{
    internal static class Wrapper
    {
        //ULTIMATE BACKUP
        internal static void callCode3()
        {
            UltimateBackup.API.Functions.callCode3Backup(false);
        }

        internal static void callCode2()
        {
            UltimateBackup.API.Functions.callCode2Backup(false);
        }

        internal static void callSwat(bool noose)
        {
            UltimateBackup.API.Functions.callCode3SwatBackup(false, noose);
        }

        internal static void callPursuit()
        {
            UltimateBackup.API.Functions.callPursuitBackup(false);
        }

        internal static void callFD()
        {
            UltimateBackup.API.Functions.callFireDepartment();
        }

        internal static void callEMS()
        {
            UltimateBackup.API.Functions.callAmbulance();
        }
    }
}