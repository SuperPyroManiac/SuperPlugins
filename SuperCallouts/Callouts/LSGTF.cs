#region

using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response;
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

[CalloutInterface("Gang Taskforce", CalloutProbability.Low, "Stakeout has found a wanted cop killer - Speak with FIB", "Code 5", "SWAT")]
internal class Lsgtf : Callout
{
    private readonly Vector3 _raidpoint = new(113.1443f, -1926.435f, 20.8231f);
    private Ped _bad1;
    private Ped _bad2;
    private Ped _bad3;
    private Ped _bad4;
    private Ped _bad5;
    private Ped _bad6;
    private Ped _bad7;
    private Ped _bad8;
    private Blip _cBlip1;
    private Blip _cBlip2;
    private Blip _cBlip3;
    private Blip _cBlip4;
    private Blip _cBlip5;
    private Blip _cBlip6;
    private Blip _cBlip7;
    private Blip _cBlip8;
    private MenuPool _conversation;
    private Vehicle _cVehicle;
    private Ped _fib1;
    private Ped _fib2;
    private UIMenu _mainMenu;
    private bool _meeting;
    private Blip _meetingB;
    private Vector3 _meetingP;
    private bool _okcool;
    private bool _onScene;
    private UIMenuItem _startConv;
    private UIMenuItem _startConv2;
    private UIMenuItem _startConv3;

    public override bool OnBeforeCalloutDisplayed()
    {
        ShowCalloutAreaBlipBeforeAccepting(_raidpoint, 80f);
        //AddMinimumDistanceCheck(20f, Raidpoint);
        //AddMaximumDistanceCheck(1500f, Raidpoint);
        CalloutMessage = "~b~LSPD Report:~s~ Wanted gang members located.";
        CalloutPosition = _raidpoint;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_BRANDISHING_WEAPON_01 IN_OR_ON_POSITION", _raidpoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Game.LogTrivial("SuperCallouts Log: LSGTF callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Meet with FIB",
            "FIB has a gang task force ready. Speak with them to conduct the raid.");
        CalloutInterfaceAPI.Functions.SendMessage(this, "**Dispatch** Go speak with the federal agents.");
        LsgtfSetup.ConstructLspdraidScene(out _bad1, out _bad2, out _bad3, out _bad4, out _bad5, out _bad6,
            out _bad7, out _bad8, out _cVehicle, out _fib1, out _fib2);
        _bad1.IsPersistent = true;
        _bad2.IsPersistent = true;
        _bad3.IsPersistent = true;
        _bad4.IsPersistent = true;
        _bad5.IsPersistent = true;
        _bad6.IsPersistent = true;
        _bad7.IsPersistent = true;
        _bad8.IsPersistent = true;
        _bad1.Inventory.Weapons.Add(WeaponHash.APPistol).Ammo = -1;
        _bad2.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        _bad3.Inventory.Weapons.Add(WeaponHash.APPistol).Ammo = -1;
        _bad4.Inventory.Weapons.Add(WeaponHash.Knife);
        _bad5.Inventory.Weapons.Add(WeaponHash.Molotov).Ammo = -1;
        _bad6.Inventory.Weapons.Add(WeaponHash.Crowbar);
        _bad7.Inventory.Weapons.Add(WeaponHash.MicroSMG).Ammo = -1;
        _bad8.Inventory.Weapons.Add(WeaponHash.Pistol50).Ammo = -1;
        _cVehicle.IsPersistent = true;
        _fib1.IsPersistent = true;
        _fib2.IsPersistent = true;
        _fib1.BlockPermanentEvents = true;
        _fib2.BlockPermanentEvents = true;
        CFunctions.SetWanted(_bad1, true);
        CFunctions.SetWanted(_bad2, true);
        CFunctions.SetWanted(_bad3, true);
        CFunctions.SetWanted(_bad4, true);
        CFunctions.SetWanted(_bad5, true);
        CFunctions.SetWanted(_bad6, true);
        CFunctions.SetWanted(_bad7, true);
        CFunctions.SetWanted(_bad8, true);
        _meetingB = _cVehicle.AttachBlip();
        _meetingB.EnableRoute(Color.Aquamarine);
        _meetingB.Color = Color.Aquamarine;
        _meetingP = _fib1.Position;
        _conversation = new MenuPool();
        _mainMenu = new UIMenu("Meeting", "Choose an option");
        _mainMenu.MouseControlsEnabled = false;
        _mainMenu.AllowCameraMovement = true;
        _conversation.Add(_mainMenu);
        _mainMenu.AddItem(_startConv = new UIMenuItem("What's the plan?"));
        _mainMenu.RefreshIndex();
        _mainMenu.OnItemSelect += LetsChatBois;

        return base.OnCalloutAccepted();
    }

    private void LetsChatBois(UIMenu unUn, UIMenuItem selItem, int nanana)
    {
        if (selItem == _startConv)
            GameFiber.StartNew(delegate
            {
                _startConv.Enabled = false;
                NativeFunction.Natives.x5AD23D40115353AC(_fib1, Game.LocalPlayer.Character,
                    -1);
                NativeFunction.Natives.x5AD23D40115353AC(_fib2, Game.LocalPlayer.Character,
                    -1);
                Game.DisplaySubtitle(
                    "~g~FIB: ~w~Thanks for coming officer, im sure you are aware how aggressive the gangs around here have become.",
                    6000);
                GameFiber.Wait(6000);
                Game.DisplaySubtitle(
                    "~g~FIB: ~w~Earlier in the week we had some gang members torture and murder a police officer. We tracked them to here.",
                    6000);
                GameFiber.Wait(6000);
                Game.DisplaySubtitle(
                    "~g~FIB: ~w~This entire group is known and wanted wanted for multiple murders. We have setup a large scale raid to bring them in.",
                    6000);
                GameFiber.Wait(6000);
                Game.DisplaySubtitle(
                    "~g~FIB: ~w~You will be the officer in charge in this raid. Let us know when to begin.", 6000);
                _mainMenu.AddItem(_startConv2 = new UIMenuItem("Yes, lets start."));
                _mainMenu.AddItem(_startConv3 = new UIMenuItem("No, I need a minute."));
            });
        if (selItem == _startConv2)
            GameFiber.StartNew(delegate
            {
                _startConv2.Enabled = false;
                _startConv3.Enabled = false;
                _mainMenu.Visible = false;
                _meeting = true;
                Game.DisplaySubtitle(
                    "~g~FIB: ~w~We will call in the raid team. They will be awaiting your arrival.", 5000);
                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Preparation", "Get to the scene",
                    "Get to the site to start the raid.");
                _meetingB.Delete();
                _cBlip1 = _bad1.AttachBlip();
                _cBlip1.Color = Color.Red;
                _cBlip1.EnableRoute(Color.Red);
                _fib1.Tasks.EnterVehicle(_cVehicle, -1);
                _fib2.Tasks.EnterVehicle(_cVehicle, 0);
                _bad1.Tasks.Wander();
                _bad2.Tasks.Wander();
                _bad3.Tasks.Wander();
                _bad4.Tasks.Wander();
                _bad5.Tasks.Wander();
                _bad6.Tasks.Wander();
                _bad7.Tasks.Wander();
                _bad8.Tasks.Wander();
                CalloutInterfaceAPI.Functions.SendMessage(this, "Proceed to raid location.");
            });
        if (selItem == _startConv3)
            GameFiber.StartNew(delegate
            {
                _mainMenu.Visible = false;
                Game.DisplaySubtitle("~g~FIB: ~w~No problem, let us know when you are.", 5000);
            });
    }

    public override void Process()
    {
        _conversation.ProcessMenus();
        if (!_meeting && !_okcool && Game.LocalPlayer.Character.DistanceTo(_meetingP) < 15f)
        {
            Game.DisplayHelp("Speak with the FIB agents. Press " + Settings.Interact + " When close.", 12000);
            _okcool = true;
        }

        if (Game.IsKeyDown(Settings.Interact) && !_meeting) _mainMenu.Visible = !_mainMenu.Visible;
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (!_onScene && _meeting && Game.LocalPlayer.Character.DistanceTo(_raidpoint) < 50f)
        {
            _fib1.Tasks.DriveToPosition(_raidpoint, 10f, VehicleDrivingFlags.Emergency, 10f);
            _cVehicle.IsSirenOn = true;
            _cVehicle.IsSirenSilent = true;
            _onScene = true;
            CalloutInterfaceAPI.Functions.SendMessage(this, "Arriving on scene, shots fired!");
CalloutInterfaceAPI.Functions.SendMessage(this, "**Dispatch** Code-33 all units respond. Station is 10-6.");
            Functions.PlayScannerAudioUsingPosition(
                "DISPATCH_SWAT_UNITS_FROM_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_01", _raidpoint);
            if (Main.UsingUb)
            {
                Wrapper.CallSwat(true);
                Wrapper.CallSwat(false);
            }
            else
            {
                Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                    EBackupResponseType.Code3, EBackupUnitType.NooseTeam);
                Functions.RequestBackup(Game.LocalPlayer.Character.Position,
                    EBackupResponseType.Code3, EBackupUnitType.SwatTeam);
            }

            _cBlip1.DisableRoute();
            _cBlip2 = _bad2.AttachBlip();
            _cBlip2.Color = Color.Red;
            _cBlip3 = _bad3.AttachBlip();
            _cBlip3.Color = Color.Red;
            _cBlip4 = _bad4.AttachBlip();
            _cBlip4.Color = Color.Red;
            _cBlip5 = _bad5.AttachBlip();
            _cBlip5.Color = Color.Red;
            _cBlip6 = _bad6.AttachBlip();
            _cBlip6.Color = Color.Red;
            _cBlip7 = _bad7.AttachBlip();
            _cBlip7.Color = Color.Red;
            _cBlip8 = _bad8.AttachBlip();
            _cBlip8.Color = Color.Red;
            _fib1.Dismiss();
            _fib2.Dismiss();
            _cVehicle.Dismiss();
            Game.LocalPlayer.Character.RelationshipGroup = "COP";
            Game.SetRelationshipBetweenRelationshipGroups("BADGANG", "COP", Relationship.Hate);
            GameFiber.StartNew(delegate
            {
                Game.DisplaySubtitle("~r~Radio: ~s~They know our plan somehow! Take cover!", 7000);
                GameFiber.Wait(8000);
                _bad1.Tasks.FightAgainst(Game.LocalPlayer.Character);
            });
        }

        if (_onScene && _bad1.IsDead && _bad2.IsDead && _bad3.IsDead && _bad4.IsDead && _bad5.IsDead &&
            _bad6.IsDead &&
            _bad7.IsDead && _bad8.IsDead)
            Game.DisplaySubtitle(
                "~r~Radio: ~s~Well that was to be expected. Clear the scene or leave and we will take care of it.",
                7000);
        if (_onScene && Game.LocalPlayer.Character.DistanceTo(_bad1.Position) > 100f &&
            Game.LocalPlayer.Character.DistanceTo(_bad2.Position) > 100f &&
            Game.LocalPlayer.Character.DistanceTo(_bad3.Position) > 100f &&
            Game.LocalPlayer.Character.DistanceTo(_bad4.Position) > 100f &&
            Game.LocalPlayer.Character.DistanceTo(_bad5.Position) > 100f &&
            Game.LocalPlayer.Character.DistanceTo(_bad6.Position) > 100f &&
            Game.LocalPlayer.Character.DistanceTo(_bad7.Position) > 100f &&
            Game.LocalPlayer.Character.DistanceTo(_bad8.Position) > 100f) End();
        base.Process();
    }

    public override void End()
    {
        if (_fib1.Exists()) _fib1.Dismiss();
        if (_fib2.Exists()) _fib2.Dismiss();
        if (_cVehicle.Exists()) _cVehicle.Dismiss();
        if (_bad1.Exists()) _bad1.Dismiss();
        if (_bad2.Exists()) _bad2.Dismiss();
        if (_bad3.Exists()) _bad3.Dismiss();
        if (_bad4.Exists()) _bad4.Dismiss();
        if (_bad5.Exists()) _bad5.Dismiss();
        if (_bad6.Exists()) _bad6.Dismiss();
        if (_bad7.Exists()) _bad7.Dismiss();
        if (_bad8.Exists()) _bad8.Dismiss();
        if (_cBlip1.Exists()) _cBlip1.Delete();
        if (_cBlip2.Exists()) _cBlip2.Delete();
        if (_cBlip3.Exists()) _cBlip3.Delete();
        if (_cBlip4.Exists()) _cBlip4.Delete();
        if (_cBlip5.Exists()) _cBlip5.Delete();
        if (_cBlip6.Exists()) _cBlip6.Delete();
        if (_cBlip7.Exists()) _cBlip7.Delete();
        if (_cBlip8.Exists()) _cBlip8.Delete();
        if (_meetingB.Exists()) _meetingB.Delete();
        CFunctions.Code4Message();
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        CalloutInterfaceAPI.Functions.SendMessage(this, "Scene clear, Code4");
        base.End();
    }
}