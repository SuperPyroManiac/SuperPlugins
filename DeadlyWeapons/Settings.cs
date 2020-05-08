#region

using Rage;

#endregion

namespace DeadlyWeapons
{
    internal static class Settings
    {
        internal static bool EnableDamageSystem = true;
        internal static bool EnablePanic = true;
        internal static bool Code3Backup = true;
        internal static bool SwatBackup;
        internal static bool NooseBackup;
        internal static bool EnableBetterAi = true;
        internal static int AiAccuracy = 20;
        internal static int PanicCooldown = 150;

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading Deadly Weapons config.");
            var path = "Plugins/LSPDFR/DeadlyWeapons.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            EnableDamageSystem = ini.ReadBoolean("Features", "EnableDamageSystem", true);
            EnablePanic = ini.ReadBoolean("Features", "EnablePanic", true);
            Code3Backup = ini.ReadBoolean("Backup", "Code3Backup", true);
            SwatBackup = ini.ReadBoolean("Backup", "SwatBackup");
            NooseBackup = ini.ReadBoolean("Backup", "NooseBackup");
            EnableBetterAi = ini.ReadBoolean("Features", "EnableBetterAI", true);
            AiAccuracy = ini.ReadInt32("Features", "AIAccuracy", 20);
            PanicCooldown = ini.ReadInt32("Features", "TimeBetweenEvents", 150);
            Game.LogTrivial("Deadly Weapons: Config loaded.");
        }
    }
}