using System.Reflection;
using System.Windows.Forms;
using Rage;

namespace TurnOffThatEngine
{
    internal static class Settings
    {
        internal static Keys turnoffengine = Keys.C;
        internal static readonly string CalloutVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading TurnOffThatEngine config.");
            var path = "Plugins/LSPDFR/TurnOffThatEngine.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            turnoffengine = ini.ReadEnum("Keys", "TurnOffEngine", Keys.C);
            Game.LogTrivial("TurnOffThatEngine: Config loaded.");
        }
    }
}