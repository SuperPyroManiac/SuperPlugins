using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;
using Functions = LSPD_First_Response.Mod.API.Functions;

namespace SuperCallouts.Callouts;

[CalloutInterface("Stolen Construction Vehicle", CalloutProbability.Low, "Very large construction vehicle reported stolen", "Code 3")]
internal class StolenDumptruck : Callout
{
    private Ped _bad;
    private Blip _cBlip;
    private UIMenu _convoMenu;
    private Vehicle _cVehicle;
    private UIMenuItem _endCall;
    private MenuPool _interaction;
    private UIMenu _mainMenu;
    private LHandle _pursuit;
    private Vector3 _spawnPoint;
    private CState _state = CState.CheckDistance;
    
    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(350f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
        CalloutMessage = "~b~Dispatch:~s~ Stolen construction vehicle.";
        CalloutAdvisory = "A very large vehicle was stolen from a construction site.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "WE_HAVE CRIME_BRANDISHING_WEAPON_01 CRIME_ROBBERY_01 IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }
    
    public override bool OnCalloutAccepted()
    {
        //Setup
        Game.LogTrivial("SuperCallouts Log: StolenDumptruck callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Stolen Construction Vehicle",
            "A suspect has stolen a very large construction vehicle. Respond ~r~CODE-3");
        CalloutInterfaceAPI.Functions.SendMessage(this, "A dump truck has been stolen from a construction site. This vehicle is very large and driving on public streets.");
            //cVehicle
        _cVehicle = new Vehicle("dump", _spawnPoint)
            { IsPersistent = true, IsStolen = true};
        //Bad
        _bad = new Ped(_spawnPoint.Around(15f));
        _bad.WarpIntoVehicle(_cVehicle, -1);
        _bad.IsPersistent = true;
        _bad.BlockPermanentEvents = true;
        _bad.Metadata.stpDrugsDetected = true;
        _bad.Metadata.stpAlcoholDetected = true;
        CFunctions.SetDrunk(_bad, true);
        //Blip
        _cBlip = _bad.AttachBlip();
        _cBlip.EnableRoute(Color.Red);
        _cBlip.Color = Color.Red;
        _cBlip.Scale = .5f;
        //Task
        _bad.Tasks.CruiseWithVehicle(_cVehicle, 100f, VehicleDrivingFlags.Emergency);
        //UI
        CFunctions.BuildUi(out _interaction, out _mainMenu, out _convoMenu, out _, out _endCall);
        _mainMenu.OnItemSelect += Interactions;
        return base.OnCalloutAccepted();
    }
    
        public override void Process()
    {
        try
        {
            switch (_state)
            {
                case CState.CheckDistance:
                    if (Game.LocalPlayer.Character.DistanceTo(_bad) < 30f)
                    {
                        _cBlip.Delete();
                        _pursuit = Functions.CreatePursuit();
                        Game.DisplayHelp(
                            $"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                        _state = CState.OnScene;
                    }

                    break;
                case CState.OnScene:
                    Functions.AddPedToPursuit(_pursuit, _bad);
                    Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                    if (Main.UsingUb)
                        Wrapper.CallPursuit();
                    else
                        Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit,
                            EBackupUnitType.LocalUnit);
                    _state = CState.End;
                    break;
            }

            //Keybinds
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (Game.IsKeyDown(Settings.Interact))
            {
                _mainMenu.Visible = !_mainMenu.Visible;
                _convoMenu.Visible = false;
            }

            _interaction.ProcessMenus();
        }
        catch (Exception e)
        {
            Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.LogTrivial("SuperCallouts Error Report Start");
            Game.LogTrivial("======================================================");
            Game.LogTrivial(e.ToString());
            Game.LogTrivial("======================================================");
            Game.LogTrivial("SuperCallouts Error Report End");
            End();
        }

        base.Process();
    }

    public override void End()
    {
        CFunctions.Code4Message();
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        if (_cBlip) _cBlip.Delete();
        if (_cVehicle) _cVehicle.Dismiss();
        if (_bad) _bad.Dismiss();
        _interaction.CloseAllMenus();
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
    
    private enum CState
    {
        CheckDistance,
        OnScene,
        End
    }
}