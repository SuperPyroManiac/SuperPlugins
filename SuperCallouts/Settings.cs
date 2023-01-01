using System.Reflection;
using System.Windows.Forms;
using Rage;

namespace SuperCallouts
{
    internal static class Settings
    {
        internal static bool Animals = true;
        internal static Keys Interact = Keys.Y;
        internal static Keys EndCall = Keys.End;
        internal static string EmergencyNumber = "911";
        internal static readonly string CalloutVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading SuperCallouts config.");
            var path = "Plugins/LSPDFR/SuperCallouts.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            //SwatCallouts
            //BasicCallouts
            Animals = ini.ReadBoolean("Settings", "AttackingAnimal", true);
            //Other
            Interact = ini.ReadEnum("Keys", "Interact", Keys.Y);
            EndCall = ini.ReadEnum("Keys", "EndCall", Keys.End);
            EmergencyNumber = ini.ReadString("Msc", "EmergencyNumber", "911");
            Game.LogTrivial("SuperCallouts: Config loaded.");
        }
    }
}