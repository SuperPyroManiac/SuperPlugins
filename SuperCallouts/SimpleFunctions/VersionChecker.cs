#region

using System.Net;
using System.Threading;
using PyroCommon.API;
using Rage;

#endregion

namespace SuperCallouts.SimpleFunctions;

internal static class VersionChecker
{
    private static readonly Thread UpdateThread = new Thread(CheckVersion);
    internal static void IsUpdateAvailable()
    {
        UpdateThread.Start();
    }

    private static void CheckVersion()
    {
        var curVersion = Settings.SCVersion;
        var receivedData = string.Empty;

        try
        {
            receivedData = new WebClient().DownloadString("https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=23995&textOnly=1").Trim();
        }
        catch (WebException)
        {
            Log.Warning("Unable to check for updates! No internet or LSPDFR is down?");
        }
        if (receivedData == Settings.SCVersion) return;
        
        Game.DisplayNotification(
            "commonmenu", 
            "mp_alerttriangle", 
            "~r~SuperEvents Warning",
            "~y~A new update is available!",
            $"Current Version: ~r~{curVersion}~w~<br>New Version: ~g~{receivedData}");
        Log.Warning($"A new version is available!\r\nCurrent Version: {curVersion}\r\nNew Version: {receivedData}");
    }
}