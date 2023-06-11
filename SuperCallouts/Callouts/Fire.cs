#region

using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Fire", CalloutProbability.Medium, "Reports of a vehicle fire", "Code 3")]
internal class Fire : Callout
{
    private readonly UIMenuItem _endCall = new("~y~End Callout", "Ends the callout early.");
    private readonly MenuPool _interaction = new();
    private readonly UIMenu _mainMenu = new("SuperCallouts", "~y~Choose an option.");
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private bool _onScene;
    private Vector3 _spawnPoint;
    private float _spawnPointH;

    public override bool OnBeforeCalloutDisplayed()
    {
        PyroFunctions.FindSideOfRoad(750, 280, out _spawnPoint, out _spawnPointH);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
        CalloutMessage = "~b~Dispatch:~s~ Reports of a car fire";
        CalloutAdvisory = "Caller reports large flames coming from the vehicle.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        //Setup
        Log.Info("fire callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Fire",
            "Reports of a car fire, respond ~r~CODE-3");
        //cVehicle
        PyroFunctions.SpawnAnyCar(out _cVehicle, _spawnPoint);
        _cVehicle.Heading = _spawnPointH;
        //Start UI
        _mainMenu.MouseControlsEnabled = false;
        _mainMenu.AllowCameraMovement = true;
        _interaction.Add(_mainMenu);
        _mainMenu.AddItem(_endCall);
        _mainMenu.RefreshIndex();
        _mainMenu.OnItemSelect += Interactions;
        //cBlip
        _cBlip = _cVehicle.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        try
        {
            //GamePlay
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle) < 25f)
            {
                _onScene = true;
                _cBlip.DisableRoute();
                for (var i = 0; i < 5; i++) PyroFunctions.FireControl(_spawnPoint.Around2D(1f, 5f), 24, true);
                for (var i = 0; i < 10; i++) PyroFunctions.FireControl(_spawnPoint.Around2D(1f, 5f), 24, false);
                Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
            }

            //Keybinds
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
            _interaction.ProcessMenus();
        }
        catch (Exception e)
        {
Log.Error(e.ToString());
            End();
        }

        base.Process();
    }

    public override void End()
    {
        if (_cVehicle.Exists()) _cVehicle.Dismiss();
        if (_cBlip.Exists()) _cBlip.Delete();
        _mainMenu.Visible = false;
        
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        CalloutInterfaceAPI.Functions.SendMessage(this, "Scene clear, Code4");
        base.End();
    }

    private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _endCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            End();
        }
    }
}