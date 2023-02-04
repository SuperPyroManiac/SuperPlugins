#region

using System.Reflection;
using Rage;

#endregion

namespace DeadlyWeaponsLegacy
{
    internal static class Settings
    {
        internal static bool EnableDamageSystem = true;
        internal static bool EnablePanic = true;
        internal static bool Code3Backup = true;
        internal static bool SwatBackup;
        internal static bool NooseBackup;
        internal static bool EnableBetterAi = true;
        internal static bool EnablePulloverAi = true;
        internal static int AiAccuracy = 20;
        internal static int PanicCooldown = 150;
        internal static int PluginDelay = 250;
        internal static readonly string CalloutVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

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
            EnablePulloverAi = ini.ReadBoolean("Features", "EnablePulloverAI", true);
            AiAccuracy = ini.ReadInt32("Features", "AIAccuracy", 20);
            PanicCooldown = ini.ReadInt32("Features", "TimeBetweenEvents", 150);
            PluginDelay = ini.ReadInt32("Advanced", "PluginDelay", 250);
            Game.LogTrivial("Deadly Weapons: Config loaded.");
        }
    }
}