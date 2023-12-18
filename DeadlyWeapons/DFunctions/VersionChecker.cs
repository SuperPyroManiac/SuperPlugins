using System;
using System.Net;
using System.Threading;
using PyroCommon.API;
using Rage;

namespace DeadlyWeapons.DFunctions;

internal static class VersionChecker
{
	private enum State
	{
		Failed,
		Update,
		Current
	}

	private static State _state = State.Current;
	private static string _receivedData = string.Empty;
	internal static readonly Thread UpdateThread = new Thread(CheckVersion);

	internal static void IsUpdateAvailable()
	{
		try
		{
			UpdateThread.Start();
			GameFiber.Sleep(5000);

			while (UpdateThread.IsAlive) GameFiber.Wait(1000);

			switch (_state)
			{
				case State.Failed:
					Log.Warning("Unable to check for updates! No internet or LSPDFR is down?");
					break;
				case State.Update:
					Game.DisplayNotification(
						"commonmenu",
						"mp_alerttriangle",
						"~r~DeadlyWeapons Warning",
						"~y~A new update is available!",
						$"Current Version: ~r~{Settings.DWVersion}~w~<br>New Version: ~g~{_receivedData}");
					Log.Warning(
						$"A new version is available!\r\nCurrent Version: {Settings.DWVersion}\r\nNew Version: {_receivedData}");
					break;
				case State.Current:
					Log.Info("Version is up to date!");
					break;
			}
		}
		catch (Exception e)
		{
			_state = State.Failed;
			Log.Info("VersionChecker failed due to rapid reloads!");
		}
	}

	private static void CheckVersion()
	{
		try
		{
			_receivedData = new WebClient()
				.DownloadString(
					"https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=27453&textOnly=1")
				.Trim();
		}
		catch (WebException)
		{
			_state = State.Failed;
		}

		if (_receivedData == Settings.DWVersion) return;
		_state = State.Update;
	}
}