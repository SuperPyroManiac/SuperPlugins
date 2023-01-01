namespace DeadlyWeaponsLegacy.Modules
{
    internal static class Wrapper
    {
        internal static void CallCode3()
        {
            UltimateBackup.API.Functions.callCode3Backup(false);
        }
        internal static void CallSwat()
        {
            UltimateBackup.API.Functions.callCode3SwatBackup(false, false);
        }
        internal static void CallNoose()
        {
            UltimateBackup.API.Functions.callCode3SwatBackup(false, true);
        }
    }
}