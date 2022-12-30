#region

using System.Net;
using Rage;

#endregion

namespace SuperCalloutsLegacy.SimpleFunctions;

internal class VersionChecker
{
    internal static bool IsUpdateAvailable()
    {
        var curVersion = Settings.CalloutVersion;
        var webClient = new WebClient();
        var receivedData = string.Empty;

        try
        {
            receivedData = webClient
                .DownloadString(
                    "https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=23995&textOnly=1")
                .Trim();
        }
        catch (WebException)
        {
            Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~y~SuperCallouts Warning",
                "~y~Failed to check for an update",
                "Please check if you are ~o~online~w~, or try to reload the plugin.");

            Game.Console.Print();
            Game.Console.Print(
                "================================================ SuperCallouts WARNING =====================================================");
            Game.Console.Print();
            Game.Console.Print("Failed to check for a update.");
            Game.Console.Print("Please check if you are online, or try to reload the plugin.");
            Game.Console.Print();
            Game.Console.Print(
                "================================================ SuperCallouts WARNING =====================================================");
            Game.Console.Print();
            // server or connection is having issues
        }

        if (receivedData != Settings.CalloutVersion)
        {
            Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~r~SuperCallouts Warning",
                "~y~A new Update is available!",
                "Current Version: ~r~" + curVersion + "~w~<br>New Version: ~g~" + receivedData);

            Game.Console.Print();
            Game.Console.Print(
                "================================================ SuperCallouts WARNING =====================================================");
            Game.Console.Print();
            Game.Console.Print(
                "A new version of SuperCallouts is available! Update the Version, or play on your own risk.");
            Game.Console.Print("Current Version:  " + curVersion);
            Game.Console.Print("New Version:  " + receivedData);
            Game.Console.Print(
                "It's reccomended you update to prevent any issues that may have been fixed in the new version!");
            Game.Console.Print();
            Game.Console.Print(
                "================================================ SuperCallouts WARNING =====================================================");
            Game.Console.Print();
            return true;
        }

        return false;
    }
}