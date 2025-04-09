using PyroCommon.UIManager;
using Rage.Attributes;

namespace PyroCommon.PyroFunctions;

public static class ConsoleCommands
{
    //Open PC Menu
    [ConsoleCommand]
    public static void OpenPyroMenu()
    {
        Manager.RefreshMenus();
        Manager.MainMenu.Visible = true;
    }
}
