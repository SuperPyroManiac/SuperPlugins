using System.Reflection;
using System.Windows.Forms;
using Rage;

namespace TurnOffThatEngine
{
    internal static class Settings
    {
        internal static Keys Turnoffenginekey = Keys.C;
        internal static ControllerButtons Turnoffenginebutton = ControllerButtons.None;
        internal static readonly string CalloutVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading TurnOffThatEngine config.");
            var path = "Plugins/TurnOffThatEngine/TurnOffThatEngine.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            Turnoffenginekey = ini.ReadEnum("Keys", "TurnOffEngine", Keys.C);
            Turnoffenginebutton = ini.ReadEnum<ControllerButtons>("Controller", "TurnOffEngine", ControllerButtons.None);
            Game.LogTrivial("TurnOffThatEngine: Config loaded.");
        }
    }
}