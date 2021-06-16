using System.Reflection;
using Rage;

namespace TurnOffThatEngine
{
    public class EntryPoint
    {
        public string Snooper = "Why are you doing in here friend?";
        public static void Main()
        {
            Settings.LoadSettings();
            TurnOffThatEngine.Main.MainFiber();
            Game.LogTrivial("TurnOffThatEngine " + Assembly.GetExecutingAssembly().GetName().Version + " by SuperPyroManiac has been initialised.");
        }
    }
}