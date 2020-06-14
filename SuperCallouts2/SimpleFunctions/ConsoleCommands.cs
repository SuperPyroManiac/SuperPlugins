namespace SuperCallouts2.SimpleFunctions
{
    public static class ConsoleCommands
    {
        [Rage.Attributes.ConsoleCommand]
        public static void Command_SCReloadConfig()
        {
            Settings.LoadSettings();
        }
    }
}