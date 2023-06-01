#region

using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.CustomScenes;
using SuperCallouts.SimpleFunctions;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Truck Crash", CalloutProbability.Low, "A large truck has tipped over blocking entire freeway", "Code 3")]
internal class TruckCrash : Callout
{
    private readonly Vector3 _spawnPoint = new(2455.644f, -186.7955f, 87.83904f);
    private Vehicle _car1;
    private Vehicle _car2;
    private Blip _cBlip;
    private MenuPool _conversation;
    private UIMenu _mainMenu;
    private bool _nIce;
    private bool _onScene;
    private UIMenuItem _startConv;
    private Vehicle _truck;
    private Ped _victim;
    private Ped _victim2;
    private Ped _victim3;

    public override bool OnBeforeCalloutDisplayed()
    {
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 40f);
        //AddMinimumDistanceCheck(20f, SpawnPoint);
        //AddMaximumDistanceCheck(1500f, SpawnPoint);
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Large truck tipped over.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_AMBULANCE_REQUESTED_02 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    private void LetsChatBois(UIMenu unUn, UIMenuItem selItem, int nanana)
    {
        if (selItem == _startConv)
            GameFiber.StartNew(delegate
            {
                _startConv.Enabled = false;
                NativeFunction.Natives.x5AD23D40115353AC(_victim, Game.LocalPlayer.Character,
                    -1);
                NativeFunction.Natives.x5AD23D40115353AC(_victim2, Game.LocalPlayer.Character,
                    -1);
                Game.DisplaySubtitle("~g~Me: ~w~What happened? Are you all ok?", 4000);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle(
                    "~y~Victims: ~w~We're ok but the truck driver needs help! We were just going home and he flipped over!",
                    4000);
                _mainMenu.Visible = false;
                _nIce = true;
            });
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("SuperCallouts Log: TruckCash callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Truck Accident",
            "Reports of a truck tipped over on the highway. Respond ~r~CODE-3");
        TruckCrashSetup.ConstructTrucksScene(out _victim, out _victim2, out _victim3, out _truck, out _car1,
            out _car2);
        _victim.IsPersistent = true;
        _victim2.IsPersistent = true;
        _victim3.IsPersistent = true;
        _truck.IsPersistent = true;
        _car1.IsPersistent = true;
        _car2.IsPersistent = true;
        _cBlip = _truck.AttachBlip();
        _cBlip.EnableRoute(Color.Yellow);
        _cBlip.Color = Color.Yellow;
        Game.DisplaySubtitle("Get to the ~r~scene~w~!", 10000);
        _conversation = new MenuPool();
        _mainMenu = new UIMenu("Conversation", "Choose an option");
        _mainMenu.MouseControlsEnabled = false;
        _mainMenu.AllowCameraMovement = true;
        _conversation.Add(_mainMenu);
        _mainMenu.AddItem(_startConv = new UIMenuItem("Are you all ok?"));
        _mainMenu.RefreshIndex();
        _mainMenu.OnItemSelect += LetsChatBois;
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        _conversation.ProcessMenus();
        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_truck) < 30f)
        {
            _onScene = true;
            Game.DisplaySubtitle("~r~Speak with the Victims.", 5000);
            Game.DisplayHelp("When close, Press: " + Settings.Interact + " to speak.", 6000);
            _cBlip.Delete();
        }

        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (!_nIce)
            if (Game.IsKeyDown(Settings.Interact) && _onScene)
            {
                if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 10f)
                    _mainMenu.Visible = !_mainMenu.Visible;
                else
                    Game.DisplayHelp("Get closer to speak.");
            }

        if (_onScene && _nIce && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) > 80f) End();
        base.Process();
    }

    public override void End()
    {
        if (_victim.Exists()) _victim.Dismiss();
        if (_victim2.Exists()) _victim2.Dismiss();
        if (_victim3.Exists()) _victim3.Dismiss();
        if (_truck.Exists()) _truck.Dismiss();
        if (_car1.Exists()) _car1.Dismiss();
        if (_car2.Exists()) _car2.Dismiss();
        if (_cBlip.Exists()) _cBlip.Delete();
        _mainMenu.Visible = false;
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        CFunctions.Code4Message();
        base.End();
    }
}