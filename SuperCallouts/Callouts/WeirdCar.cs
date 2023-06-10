#region

using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Suspicious Vehicle", CalloutProbability.Medium, "Reports of a suspicious vehicle, limited details", "Code 2")]
internal class WeirdCar : Callout
{
    private readonly UIMenu _convoMenu = new("SuperCallouts", "~y~Choose a subject to speak with.");
    private readonly UIMenuItem _endCall = new("~y~End Call", "Ends the callout.");
    private readonly MenuPool _interaction = new();
    private readonly UIMenu _mainMenu = new("SuperCallouts", "~y~Choose an option.");
    private readonly UIMenuItem _questioning = new("Speak With Subjects");
    private readonly Random _rNd = new();
    private Ped _bad1;
    private Blip _cBlip1;
    private Vehicle _cVehicle1;
    private string _name1;
    private bool _onScene;
    private Vector3 _spawnPoint;
    private float _spawnPointH;
    private UIMenuItem _speakSuspect;

    public override bool OnBeforeCalloutDisplayed()
    {
        CFunctions.FindSideOfRoad(750, 280, out _spawnPoint, out _spawnPointH);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
        CalloutMessage = "~b~Dispatch:~s~ Suspicious vehicle.";
        CalloutAdvisory = "Suspicious vehicle was found on the side of the road. Approach with caution.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        //Setup
        Game.Console.Print("SuperCallouts Log: Wierd Car callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Suspicious Vehicle",
            "Report of a suspicious vehicle on the side of the road. Respond ~y~CODE-2");
        //cVehicle1
        CFunctions.SpawnNormalCar(out _cVehicle1, _spawnPoint);
        _cVehicle1.Heading = _spawnPointH;
        _cVehicle1.IsPersistent = true;
        //Start UI
        _mainMenu.MouseControlsEnabled = false;
        _mainMenu.AllowCameraMovement = true;
        _interaction.Add(_mainMenu);
        _interaction.Add(_convoMenu);
        _mainMenu.AddItem(_questioning);
        _mainMenu.AddItem(_endCall);
        _mainMenu.RefreshIndex();
        _convoMenu.RefreshIndex();
        _mainMenu.BindMenuToItem(_convoMenu, _questioning);
        _mainMenu.OnItemSelect += Interactions;
        _convoMenu.OnItemSelect += Conversations;
        _convoMenu.ParentMenu = _mainMenu;
        _questioning.Enabled = false;
        //cBlip1
        _cBlip1 = _cVehicle1.AttachBlip();
        _cBlip1.EnableRoute(Color.Yellow);
        _cBlip1.Color = Color.Yellow;
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        try
        {
            //GamePlay
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle1) < 30f)
            {
                _onScene = true;
                _cBlip1.DisableRoute();
                Game.DisplayHelp("Investigate the vehicle.");
                var choices = _rNd.Next(1, 4);
                switch (choices)
                {
                    case 1:
                        CFunctions.Damage(_cVehicle1, 500, 500);
                        _cVehicle1.IsStolen = true;
                        CalloutInterfaceAPI.Functions.SendMessage(this, "Officer on scene.");
                        break;
                    case 2:
                        GameFiber.StartNew(delegate
                        {
                            _cVehicle1.IsStolen = true;
                            _bad1 = _cVehicle1.CreateRandomDriver();
                            _bad1.IsPersistent = true;
                            _bad1.BlockPermanentEvents = true;
                            _bad1.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                            Game.DisplaySubtitle("~r~Driver:~s~ The world will end with fire!");
                            GameFiber.Wait(3000);
                            _cVehicle1.Explode();
                            CalloutInterfaceAPI.Functions.SendMessage(this, "Officer on scene.");
                            CalloutInterfaceAPI.Functions.SendMessage(this, "Vehicle explosion.");
                        });
                        break;
                    case 3:
                        _bad1 = _cVehicle1.CreateRandomDriver();
                        _bad1.IsPersistent = true;
                        _bad1.BlockPermanentEvents = true;
                        _name1 = Functions.GetPersonaForPed(_bad1).FullName;
                        CFunctions.SetWanted(_bad1, true);
                        _cVehicle1.IsStolen = true;
                        CalloutInterfaceAPI.Functions.SendMessage(this, "Officer on scene.");
                        //UI Setup
                        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
                        _convoMenu.AddItem(_speakSuspect);
                        _convoMenu.RefreshIndex();
                        _questioning.Enabled = true;
                        break;
                    default:
                        Game.DisplayNotification(
                            "An error has been detected! Ending callout early to prevent LSPDFR crash!");
                        End();
                        break;
                }
            }

            //Keybinds
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
            _interaction.ProcessMenus();
        }
        catch (Exception e)
        {
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print("SuperCallouts Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print(e.ToString());
            Game.Console.Print("======================================================");
            Game.Console.Print("SuperCallouts Error Report End");
            End();
        }

        base.Process();
    }

    public override void End()
    {
        if (_bad1.Exists()) _bad1.Dismiss();
        if (_cVehicle1.Exists()) _cVehicle1.Dismiss();
        if (_cBlip1.Exists()) _cBlip1.Delete();
        _mainMenu.Visible = false;
        CFunctions.Code4Message();
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        CalloutInterfaceAPI.Functions.SendMessage(this, "Scene clear, Code4");
        base.End();
    }

    //UI Items
    private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _endCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            End();
        }
    }

    private void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(delegate
            {
                Game.DisplaySubtitle("~g~You~s~: We have reports of suspicious activity here, what's going on?",
                    5000);
                _bad1.Tasks.LeaveVehicle(_cVehicle1, LeaveVehicleFlags.LeaveDoorOpen);
                GameFiber.Wait(5000);
                NativeFunction.Natives.x5AD23D40115353AC(_bad1, Game.LocalPlayer.Character, -1);
                _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                Game.DisplaySubtitle(
                    "~r~" + _name1 + "~s~: Nothing is wrong sir, I don't know why you got that idea.", 5000);
            });
    }
}