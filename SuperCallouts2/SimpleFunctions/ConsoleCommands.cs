namespace SuperCallouts2.SimpleFunctions
{
    internal static class ConsoleCommands
    {
        [Rage.Attributes.ConsoleCommand]
        internal static void Command_SCReloadConfig()
        {
            Settings.LoadSettings();
        }
    }
}