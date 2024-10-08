using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PyroCommon.Objects;
using Rage;
using Task = System.Threading.Tasks.Task;

namespace PyroCommon.PyroFunctions;

internal static class VersionChecker
{
    internal static readonly Dictionary<string, string> OutdatedPyroPlugins = new();
    private static Enums.UpdateState _updateState = Enums.UpdateState.Current;

    internal static void Validate(Dictionary<string, string> pluginDict)
    {
        try
        {
            OutdatedPyroPlugins.Clear();
            var updateTask = Task.Run(() => CheckVersion(pluginDict));
            GameFiber.WaitUntil(() => updateTask.IsCompleted);
            HandleUpdateResult(pluginDict);
        }
        catch (Exception)
        {
            _updateState = Enums.UpdateState.Failed;
            Log.Warning("VersionChecker failed to run!");
        }
    }

    private static async Task CheckVersion(Dictionary<string, string> plugDict)
    {
        var httpClient = new HttpClient();
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(15));
        
        foreach (var plug in plugDict)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.PyrosFun.com/ver/{plug.Key}");
                var response = await httpClient.SendAsync(request, cts.Token);
                response.EnsureSuccessStatusCode();
                var receivedData = await response.Content.ReadAsStringAsync();

                if (receivedData == plug.Value) return;
                OutdatedPyroPlugins[plug.Key] = receivedData;
                _updateState = Enums.UpdateState.Update;
            }
            catch (TaskCanceledException) when (cts.Token.IsCancellationRequested)
            {
                Log.Warning($"Version request for {plug.Key} timed out.");
                _updateState = Enums.UpdateState.Failed;
            }
            catch (WebException ex)
            {
                Log.Warning($"An error occurred in the updater:\r\n{ex.Message}");
                _updateState = Enums.UpdateState.Failed;
            }
            catch (Exception ex)
            {
                Log.Warning($"An error occurred in the updater:\r\n{ex.Message}");
                _updateState = Enums.UpdateState.Failed;
            }
        }
    }
    
    private static void HandleUpdateResult(Dictionary<string, string> pluginDict)
    {
        switch (_updateState)
        {
            case Enums.UpdateState.Failed:
                Log.Warning("Unable to check for updates!");
                break;
            case Enums.UpdateState.Update:
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
            case Enums.UpdateState.Current:
                Log.Info("Plugins are up to date!");
                break;
        }
    }
}