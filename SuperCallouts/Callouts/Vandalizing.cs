using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts;

[CalloutInfo("Vandalizing", CalloutProbability.Medium)]
internal class Vandalizing : Callout
{
    private CState _state = CState.CheckDistance;
    private UIMenu _convoMenu;
    private UIMenuItem _endCall;
    private MenuPool _interaction;
    private UIMenu _mainMenu;
    private Vector3 _spawnPoint;
    private Vehicle _cVehicle;
    private Ped _bad;
    private Blip _cBlip;
    
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
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Vandalizing",
            "A suspect has been reported damaging a vehicle. Respond ~r~CODE-3");
        if (Main.UsingCi)
        {
            Wrapper.StartCi(this, "Code 3");
            Wrapper.CiSendMessage(this, "A dump truck has been stolen from a construction site. This vehicle is very large and driving on public streets.");
        }
        //cVehicle
        _cVehicle = new Vehicle("dump", _spawnPoint)
            { IsPersistent = true};
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