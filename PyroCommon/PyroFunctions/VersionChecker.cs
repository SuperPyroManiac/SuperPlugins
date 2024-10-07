using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using Rage;
using Task = System.Threading.Tasks.Task;

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
            HandleUpdateResult(pluginDict);
        }
        catch (Exception)
        {
            _state = State.Failed;
            Log.Warning("VersionChecker failed to run!");
        }
    }

    private static async Task CheckVersion(Dictionary<string, string> plugDict)
    {
        var client = new HttpClient();
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(15));
        foreach (var plug in plugDict)
        {
            try
            {
                var receivedData = await client.GetStringAsync($"https://api.PyrosFun.com/ver/{plug.Key}", cts.Token);
                if ( receivedData == plug.Value ) return;
                OutdatedPyroPlugins[plug.Key] = receivedData;
                _state = State.Update;
            }
            catch (WebException)
            {
                _state = State.Failed;
            }
        }
    }
    
    private static void HandleUpdateResult(Dictionary<string, string> pluginDict)
    {
        switch (_state)
        {
            case State.Failed:
                Log.Warning("Unable to check for updates!");
                break;
            case State.Update:
                var ingameNotice = string.Empty;
                var logNotice = "Plugin updates available!";

                foreach (var plug in OutdatedPyroPlugins)
                {
                    ingameNotice += $"~w~{plug.Key}: ~r~{pluginDict[plug.Key]} <br>~w~New Version: ~g~{plug.Value}<br>";
                    logNotice += $"\r\n{plug.Key}: Current Version: {pluginDict[plug.Key]} New Version: {plug.Value}";
                }
                if (Settings.UpdateNotifications) Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~r~SuperPlugins Warning", "~y~New updates available!", ingameNotice);
                Log.Warning(logNotice);
                break;
            case State.Current:
                Log.Info("Plugins are up to date!");
                break;
        }
    }
}