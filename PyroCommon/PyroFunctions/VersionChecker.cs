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
	private static string _receivedData = string.Empty;

	internal static void IsUpdateAvailable(Dictionary<string, string> pluginDict)
    {
        try
		{
			var updateThread = new Thread(() => CheckVersion(pluginDict));
			updateThread.Start();
            GameFiber.SleepWhile(() => updateThread.IsAlive, 0);
            
			switch (_state)
			{
				case State.Failed:
					Log.Warning("Unable to check for updates! No internet or LSPDFR is down?");
					break;
					
					case State.Update:
						var ingameNotice = string.Empty;
						var logNotice = "Plugin updates available!";
						
						foreach ( var plug in OutdatedPyroPlugins )
						{
                            Log.Info($"ign {plug.Key} {plug.Value}");
                            Log.Info($"{pluginDict[plug.Key]}");
							ingameNotice += $"~w~{plug.Key}: ~r~{pluginDict[plug.Key]} <br>~w~New Version: ~g~{plug.Value}<br>";
							logNotice += $"\r\n{plug.Key}: Current Version: {pluginDict[plug.Key]} New Version: {plug.Value}";
						}
						if (Settings.UpdateNotifications) Game.DisplayNotification("commonmenu", "mp_alerttriangle", 
                            "~r~SuperPlugins Warning", "~y~New updates available!", ingameNotice);
						Log.Warning(logNotice);
						break;
					
				case State.Current:
					Log.Info("Plugins are up to date!");
					break;
			}
		}
		catch (Exception)
		{
			_state = State.Failed;
			Log.Info("VersionChecker failed to run!");
		}
    }

	private static void CheckVersion(Dictionary<string, string> plugDict)
	{
		foreach ( var plug in plugDict )
		{
			try
            {
                var id = plug.Key switch
                {
                    "SuperCallouts" => "23995",
                    "SuperEvents" => "24437",
                    "DeadlyWeapons" => "27453",
                    _ => string.Empty
                };
                Log.Info(id);
                _receivedData = new WebClient().DownloadString($"https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId={id}&textOnly=1").Trim();
            }
			catch (WebException)
			{
				_state = State.Failed;
			}
			
            Log.Info(_receivedData);
            Log.Info(plug.Value);
			if (_receivedData == plug.Value) continue;
			OutdatedPyroPlugins.Add(plug.Key, _receivedData);
			_state = State.Update;
		}
	}
}