using PyroCommon.UIManager;
using Rage.Attributes;

namespace PyroCommon.Services;

public static class CommandService
{
    //Open PC Menu
    [ConsoleCommand]
    public static void OpenPyroMenu()
    {
        Manager.RefreshMenus();
        Manager.MainMenu.Visible = true;
    }
}
