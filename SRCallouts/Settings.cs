using System.Windows.Forms;
using Rage;

namespace SRCallouts
{
    internal static class Settings
    {
        internal static bool Mafia1 = true;
        internal static Keys Interact = Keys.Y;
        internal static Keys EndCall = Keys.End;

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading SR Callouts config.");
            var path = "Plugins/LSPDFR/SRCallouts.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            Mafia1 = ini.ReadBoolean("Settings", "CarAccident", true);
            Interact = ini.ReadEnum("Keys", "Interact", Keys.Y);
            EndCall = ini.ReadEnum("Keys", "EndCall", Keys.End);
            Game.LogTrivial("SR Callouts: Config loaded.");
        }
    }
}