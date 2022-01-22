using System;
using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts;

[CalloutInfo("Robbery", CalloutProbability.Medium)]
internal class Robbery : Callout
{
    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(450f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 30f);
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Person(s) being held at gunpoint.";
        CalloutAdvisory = "Caller reports people holding someone at gunpoint.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_03 CRIME_ROBBERY_01 IN_OR_ON_POSITION",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        //Setup
        Game.LogTrivial("SuperCallouts Log: Robery callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Possible Robbery",
            "A " + Settings.EmergencyNumber +
            " report claims 2 armed people are holding 1 person at gunpoint. Respond ~r~CODE-3");
        //cVehicle1
        CFunctions.SpawnNormalCar(out _cVehicle, _spawnPoint);
        _cVehicle.IsPersistent = true;
        _cVehicle.EngineHealth = 0;
        CFunctions.Damage(_cVehicle, 200, 200);
        Functions.SetVehicleOwnerName(_cVehicle, _rude1Name);
        //cVehicle2
        CFunctions.SpawnNormalCar(out _cVehicle2, _cVehicle.GetOffsetPositionFront(6f));
        _cVehicle2.IsPersistent = true;
        _cVehicle2.EngineHealth = 0;
        _cVehicle2.Rotation = new Rotator(0f, 0f, 180f);
        CFunctions.Damage(_cVehicle2, 200, 200);
        Functions.SetVehicleOwnerName(_cVehicle2, _victimName);
        //rude1
        _rude1 = _cVehicle.CreateRandomDriver();
        _rude1.IsPersistent = true;
        _rude1.BlockPermanentEvents = true;
        _rude1.Inventory.GiveNewWeapon("WEAPON_PISTOL", 500, true);
        _rude1.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        _rude1Name = Functions.GetPersonaForPed(_rude1).FullName;
        //rude2
        _rude2 = new Ped();
        _rude2.IsPersistent = true;
        _rude2.BlockPermanentEvents = true;
        _rude2.WarpIntoVehicle(_cVehicle, 0);
        _rude2.Inventory.GiveNewWeapon("WEAPON_PUMPSHOTGUN", 500, true);
        _rude2.Tasks.LeaveVehicle(_cVehicle, LeaveVehicleFlags.LeaveDoorOpen);
        //victim
        _victim = _cVehicle2.CreateRandomDriver();
        _victim.IsPersistent = true;
        _victim.BlockPermanentEvents = true;
        _victim.Tasks.LeaveVehicle(_cVehicle2, LeaveVehicleFlags.LeaveDoorOpen);
        _victim.Health = 200;
        _victimName = Functions.GetPersonaForPed(_victim).FullName;
        //Blips
        _blip1 = _victim.AttachBlip();
        _blip1.EnableRoute(Color.Yellow);
        _blip1.Scale = .75f;
        _blip1.Color = Color.Yellow;
        _blip2 = _rude1.AttachBlip();
        _blip2.Scale = .75f;
        _blip2.Color = Color.Red;
        _blip3 = _rude2.AttachBlip();
        _blip3.Scale = .75f;
        _blip3.Color = Color.Red;
        //Start UI
        _mainMenu.MouseControlsEnabled = false;
        _mainMenu.AllowCameraMovement = true;
        _interaction.Add(_mainMenu);
        _mainMenu.AddItem(_endCall);
        _mainMenu.RefreshIndex();
        _mainMenu.OnItemSelect += Interactions;
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        try
        {
            //Gameplay
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_victim.Position) < 40f)
            {
                _blip1.Delete();
                _blip2.Delete();
                _blip3.Delete();
                _pursuit = Functions.CreatePursuit();
                _onScene = true;
                var choices = _rNd.Next(1, 5);
                Game.DisplaySubtitle("~r~Suspect: ~w~What are the cops doing here?!", 5000);
                switch (choices)
                {
                    case 1:
                        GameFiber.StartNew(delegate
                        {
                            NativeFunction.Natives.x9B53BB6E8943AF53(_rude1, _victim, -1, true);
                            NativeFunction.Natives.x9B53BB6E8943AF53(_rude2, _victim, -1, true);
                            _victim.Tasks.PutHandsUp(-1, _rude1);
                            GameFiber.Wait(2000);
                            NativeFunction.Natives.xF166E48407BAC484(_rude1, _victim, 0, 1);
                            NativeFunction.Natives.xF166E48407BAC484(_rude2, _victim, 0, 1);
                            _victim.Tasks.Cower(-1);
                            GameFiber.Wait(3000);
                            NativeFunction.Natives.x72C896464915D1B1(_rude1,
                                Game.LocalPlayer.Character);
                            NativeFunction.Natives.xF166E48407BAC484(_rude2, Game.LocalPlayer.Character, 0, 1);
                            Functions.AddPedToPursuit(_pursuit, _rude1);
                            GameFiber.Wait(10000);
                            Functions.AddPedToPursuit(_pursuit, _rude2);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        });
                        break;
                    case 2:
                        GameFiber.StartNew(delegate
                        {
                            NativeFunction.Natives.x9B53BB6E8943AF53(_rude1, _victim, -1, true);
                            NativeFunction.Natives.x9B53BB6E8943AF53(_rude2, _victim, -1, true);
                            _victim.Tasks.PutHandsUp(-1, _rude1);
                            GameFiber.Wait(4000);
                            NativeFunction.Natives.x72C896464915D1B1(_rude1,
                                Game.LocalPlayer.Character);
                            NativeFunction.Natives.x72C896464915D1B1(_rude2,
                                Game.LocalPlayer.Character);
                            _victim.Tasks.Cower(-1);
                            Functions.AddPedToPursuit(_pursuit, _rude1);
                            Functions.AddPedToPursuit(_pursuit, _rude2);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        });
                        break;
                    case 3:
                        GameFiber.StartNew(delegate
                        {
                            NativeFunction.Natives.x9B53BB6E8943AF53(_rude1, _victim, -1, true);
                            NativeFunction.Natives.x9B53BB6E8943AF53(_rude2, _victim, -1, true);
                            _victim.Tasks.PutHandsUp(-1, _rude1);
                            GameFiber.Wait(4000);
                            NativeFunction.Natives.xF166E48407BAC484(_rude1, Game.LocalPlayer.Character, 0, 1);
                            NativeFunction.Natives.xF166E48407BAC484(_rude2, Game.LocalPlayer.Character, 0, 1);
                            CFunctions.SetWanted(_victim, true);
                            NativeFunction.Natives.x72C896464915D1B1(_victim, _rude1);
                            GameFiber.Wait(5000);
                            Functions.AddPedToPursuit(_pursuit, _rude1);
                            Functions.AddPedToPursuit(_pursuit, _rude2);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        });
                        break;
                    case 4:
                        GameFiber.StartNew(delegate
                        {
                            NativeFunction.Natives.x9B53BB6E8943AF53(_rude1, _victim, -1, true);
                            NativeFunction.Natives.x9B53BB6E8943AF53(_rude2, _victim, -1, true);
                            _victim.Tasks.PutHandsUp(-1, _rude1);
                            GameFiber.Wait(4000);
                            NativeFunction.Natives.x9B53BB6E8943AF53(_rude1, Game.LocalPlayer.Character,
                                -1, true);
                            NativeFunction.Natives.x9B53BB6E8943AF53(_rude2, Game.LocalPlayer.Character,
                                -1, true);
                            _victim.Tasks.Cower(-1);
                            GameFiber.Wait(2000);
                            _rude1.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                            _rude2.Tasks.PutHandsUp(-1, Game.LocalPlayer.Character);
                            GameFiber.Wait(4000);
                            Functions.AddPedToPursuit(_pursuit, _rude1);
                            Functions.AddPedToPursuit(_pursuit, _rude2);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                        });
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
        if (_victim.Exists()) _victim.Dismiss();
        if (_rude1.Exists()) _rude1.Dismiss();
        if (_rude2.Exists()) _rude2.Dismiss();
        if (_cVehicle.Exists()) _cVehicle.Dismiss();
        if (_cVehicle2.Exists()) _cVehicle2.Dismiss();
        if (_blip1.Exists()) _blip1.Delete();
        if (_blip2.Exists()) _blip2.Delete();
        if (_blip3.Exists()) _blip3.Delete();
        _mainMenu.Visible = false;
        CFunctions.Code4Message();
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
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

    #region Variables

    private Blip _blip1;
    private Blip _blip2;
    private Blip _blip3;
    private Vehicle _cVehicle;
    private Vehicle _cVehicle2;
    private bool _onScene;
    private LHandle _pursuit;
    private readonly Random _rNd = new();
    private Ped _rude1;
    private string _rude1Name;
    private Ped _rude2;
    private Vector3 _spawnPoint;
    private Ped _victim;

    private string _victimName;

    //UI Items
    private readonly MenuPool _interaction = new();
    private readonly UIMenu _mainMenu = new("SuperCallouts", "~y~Choose an option.");
    private readonly UIMenuItem _endCall = new("~y~End Callout", "Ends the callout early.");

    #endregion
}