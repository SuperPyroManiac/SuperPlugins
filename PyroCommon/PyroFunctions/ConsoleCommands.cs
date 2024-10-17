using PyroCommon.UIManager;
using Rage;
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

    //DEBUG RESET
    [ConsoleCommand]
    public static void RLP()
    {
        Game.Console.Print("Reloading LSPDFR");
        World.CleanWorld(true, true, true, true, true, true);
        foreach ( Blip b in World.GetAllBlips() )
        {
            if ( b )
            {
                b.Delete();
            }
        }

        Game.ReloadActivePlugin();
    }

    //DEBUG ERROR
    [ConsoleCommand]
    public static void ErrorTest()
    {
        Log.Error("This is a test error message");
    }
}
