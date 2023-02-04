#region

using System.Reflection;
using Rage;
// ReSharper disable InconsistentNaming

#endregion

namespace DeadlyWeapons
{
    internal static class Settings
    {
        internal static readonly string CalloutVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading Deadly Weapons config.");
            var path = "Plugins/LSPDFR/DeadlyWeapons.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            var EnableDamageSystem = ini.ReadBoolean("Features", "EnableDamageSystem", true);
            var EnablePanic = ini.ReadBoolean("Features", "EnablePanic", true);
            var Code3Backup = ini.ReadBoolean("Backup", "Code3Backup", true);
            var SwatBackup = ini.ReadBoolean("Backup", "SwatBackup");
            var NooseBackup = ini.ReadBoolean("Backup", "NooseBackup");
            var EnableBetterAi = ini.ReadBoolean("Features", "EnableBetterAI", true);
            var EnablePulloverAi = ini.ReadBoolean("Features", "EnablePulloverAI", true);
            var AiAccuracy = ini.ReadInt32("Features", "AIAccuracy", 20);
            var PanicCooldown = ini.ReadInt32("Features", "TimeBetweenEvents", 150);
            var PluginDelay = ini.ReadInt32("Advanced", "PluginDelay", 250);
            Game.LogTrivial("Deadly Weapons: Config loaded.");
        }
    }
}