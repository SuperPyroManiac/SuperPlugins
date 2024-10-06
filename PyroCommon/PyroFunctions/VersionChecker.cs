using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Rage;

namespace PyroCommon.PyroFunctions;

internal static class VersionChecker
{
    internal static readonly Dictionary<string, string> OutdatedPyroPlugins = new();
    
    private enum State
    {
        Failed,
        Update,
        Current
    }
    private static State _state = State.Current;

    internal static void IsUpdateAvailable(Dictionary<string, string> pluginDict)
    {
        try
        {
            OutdatedPyroPlugins.Clear();
            var updateThread = new Thread(() => CheckVersion(pluginDict));
            updateThread.Start();
            GameFiber.SleepWhile(() => updateThread.IsAlive, 0);
            HandleUpdateResult(pluginDict);
        }
        catch (Exception)
        {
            _state = State.Failed;
            Log.Warning("VersionChecker failed to run!");
        }
    }

    private static void HandleUpdateResult(Dictionary<string, string> pluginDict)
    {
        switch (_state)
        {
            case State.Failed:
                Log.Warning("Unable to check for updates! No internet or LSPDFR is down?");
                break;
            case State.Update:
                NotifyOutdatedPlugins(pluginDict);
                break;
            case State.Current:
                Log.Info("Plugins are up to date!");
                break;
        }
    }

    private static void NotifyOutdatedPlugins(Dictionary<string, string> pluginDict)
    {
        var ingameNotice = string.Empty;
        var logNotice = "Plugin updates available!";

        foreach (var plug in OutdatedPyroPlugins)
        {
            ingameNotice += $"~w~{plug.Key}: ~r~{pluginDict[plug.Key]} <br>~w~New Version: ~g~{plug.Value}<br>";
            logNotice += $"\r\n{plug.Key}: Current Version: {pluginDict[plug.Key]} New Version: {plug.Value}";
        }

        if (Settings.UpdateNotifications)
            Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~r~SuperPlugins Warning", "~y~New updates available!", ingameNotice);
        Log.Warning(logNotice);
    }

    private static void CheckVersion(Dictionary<string, string> plugDict)
    {
        foreach (var plug in plugDict)
        {
            var id = GetPluginId(plug.Key);
            if (string.IsNullOrEmpty(id)) continue;
            try
            {
                var receivedData = new WebClient().DownloadString($"https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId={id}&textOnly=1").Trim();
                ProcessReceivedData(plug, receivedData);
            }
            catch (WebException)
            {
                _state = State.Failed;
            }
        }
    }

    private static string GetPluginId(string pluginName)
    {
        return pluginName switch
        {
            "SuperCallouts" => "23995",
            "SuperEvents" => "24437",
            "DeadlyWeapons" => "27453",
            _ => string.Empty
        };
    }

    private static void ProcessReceivedData(KeyValuePair<string, string> plug, string receivedData)
    {
        if ( receivedData == plug.Value ) return;
        OutdatedPyroPlugins[plug.Key] = receivedData;
        _state = State.Update;
    }
}