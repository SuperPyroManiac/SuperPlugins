using System.Windows.Forms;
using Rage;

namespace DeadlyWeapons
{
    internal static class Settings
    {
        internal static bool EnableDamageSystem = true;
        internal static bool EnablePanic = true;
        internal static int PanicCooldown = 150;

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading Deadly Weapons config.");
            var path = "Plugins/LSPDFR/DeadlyWeapons.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            EnableDamageSystem = ini.ReadBoolean("Features", "EnableDamageSystem", true);
            EnablePanic = ini.ReadBoolean("Features", "EnablePanic", true);
            PanicCooldown = ini.ReadInt32("Features", "TimeBetweenEvents", 150);
            Game.LogTrivial("Deadly Weapons: Config loaded.");
        }
    }
}