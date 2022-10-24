using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;
using System;
using System.Drawing;

namespace SuperCallouts.Callouts;

[CalloutInfo("DeadBody", CalloutProbability.Medium)]
internal class DeadBody : Callout
{
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private Ped _victim;
    private Ped _witness;
    private UIMenuItem _endCall;
    private UIMenuItem _questioning;
    private float _heading;
    private MenuPool _interaction;
    private UIMenu _mainMenu;
    private UIMenu _convoMenu;
    private bool _onScene;
    private Vector3 _spawnPoint;
    
    public override bool OnBeforeCalloutDisplayed()
    {
        CFunctions.FindSideOfRoad(750, 280, out _spawnPoint, out _heading);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Reports of an injured person.";
        CalloutAdvisory = "Caller says the person is not breathing.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_AMBULANCE_REQUESTED_01 IN_OR_ON_POSITION",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        //Setup
        Game.LogTrivial("SuperCallouts Log: Dead body callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~y~Medical Emergency",
            "Caller reports an injured person that is not breathing, respond ~r~CODE-3");
        if (Main.UsingCi) Wrapper.StartCi(this, "Code 3");
        //Vehicle
        CFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint, _heading);
        //Peds
        _witness = new Ped(_cVehicle.GetOffsetPositionFront(-9f));
        _witness.IsPersistent = true;
        _witness.BlockPermanentEvents = true;
        _witness.Tasks.Cower(-1);
        _victim = new Ped(_witness.GetOffsetPositionFront(-2f));
        _victim.IsPersistent = true;
        _victim.BlockPermanentEvents = true;
        _victim.Tasks.Cower(-1);
        //cBlip
        _cBlip = _cVehicle.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        //UI
        CFunctions.BuildUi(out _interaction, out _mainMenu, out _convoMenu, out _questioning, out _endCall);
        _mainMenu.OnItemSelect += InteractionProcess;
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        try
        {
            if (!_onScene && Game.LocalPlayer.Character.Position.DistanceTo(_spawnPoint) < 60)
            {
                _onScene = true;
                _witness.Kill();
                _cBlip.DisableRoute();
                _questioning.Enabled = true;
                Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
            }

            //Keybinds
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
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
        if(_cVehicle.Exists()) _cVehicle.Dismiss();
        if (_witness.Exists()) _witness.Dismiss();
        if (!_victim.Exists()) _victim.Dismiss();
        if (_cBlip.Exists()) _cBlip.Delete();
        _mainMenu.Visible = false;
        CFunctions.Code4Message();
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        if (Main.UsingCi) Wrapper.CiSendMessage(this, "Scene clear, Code4");
        base.End();
    }

    private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _endCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            End();
        }
    }
}