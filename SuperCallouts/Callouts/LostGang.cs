using System.Collections.Generic;
using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using Rage;
using SuperCallouts.CustomScenes;
using Functions = LSPD_First_Response.Mod.API.Functions;


namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Officer Ambush", CalloutProbability.Low)]
internal class LostGang : Callout
{
    private readonly List<Ped> _bikers = [];
    private readonly List<Vehicle> _cVehicles = [];
    private Vehicle _bike1;
    private Vehicle _bike2;
    private Vehicle _bike3;
    private Vehicle _bike4;
    private Vehicle _bike5;
    private Vehicle _bike6;
    private Vehicle _bike7;
    private Ped _biker1;
    private Ped _biker10;
    private Ped _biker2;
    private Ped _biker3;
    private Ped _biker4;
    private Ped _biker5;
    private Ped _biker6;
    private Ped _biker7;
    private Ped _biker8;
    private Ped _biker9;
    private Blip _cBlip;
    private Vehicle _cCar1;
    private Vehicle _cCar2;
    private Ped _cop1;
    private Ped _cop2;
    private Ped _cop3;
    private bool _onScene;
    private Vector3 _searcharea;
    private Vector3 _spawnPoint;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = new Vector3(2350.661f, 4920.378f, 41.7339f);
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 80f);
        CalloutMessage = "~r~Panic Button:~s~ Multiple officers under fire.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_BRANDISHING_WEAPON_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Log.Info("LostMC callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Biker Gang Attack",
            "~r~EMERGENCY~s~ All Units: Multiple officers under fire, 7 plus armed gang members attacking sheriff officers. ~r~Respond CODE-3");
        LostMc.ConstructBikersScene(out _cCar1, out _cCar2, out _cop1, out _cop2, out _cop3, out _bike1, out _bike2,
            out _bike3, out _bike4, out _bike5, out _bike6, out _bike7, out _biker1, out _biker2, out _biker3,
            out _biker4,
            out _biker5, out _biker6, out _biker7, out _biker8, out _biker9, out _biker10);
        _searcharea = _spawnPoint.Around2D(1f, 2f);
        _cBlip = new Blip(_searcharea, 80f) { Color = Color.Yellow, Alpha = .5f };
        _cBlip.EnableRoute(Color.Yellow);
        _cVehicles.Add(_cCar1);
        _cVehicles.Add(_cCar2);
        _cVehicles.Add(_bike1);
        _cVehicles.Add(_bike2);
        _cVehicles.Add(_bike3);
        _cVehicles.Add(_bike4);
        _cVehicles.Add(_bike5);
        _cVehicles.Add(_bike6);
        _cVehicles.Add(_bike7);
        _bikers.Add(_biker1);
        _bikers.Add(_biker2);
        _bikers.Add(_biker3);
        _bikers.Add(_biker4);
        _bikers.Add(_biker5);
        _bikers.Add(_biker6);
        _bikers.Add(_biker7);
        _bikers.Add(_biker8);
        _bikers.Add(_biker9);
        _bikers.Add(_biker10);
        foreach (var vehicless in _cVehicles) vehicless.IsPersistent = true;
        foreach (var bikerss in _bikers) bikerss.IsPersistent = true;
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 100f)
        {
            _onScene = true;
            _cBlip.DisableRoute();
            Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~OutNumbered",
                "~y~Stay in cover until backup arrives!");
            Functions.PlayScannerAudioUsingPosition(
                "DISPATCH_SWAT_UNITS_FROM_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_01", _spawnPoint);
            Game.SetRelationshipBetweenRelationshipGroups("LOSTERS", "COP", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("LOSTERS", "PLAYER", Relationship.Hate);
            foreach (var bikerss in _bikers)
            {
                PyroFunctions.SetWanted(bikerss, true);
                bikerss.Tasks.FightAgainstClosestHatedTarget(50f);
            }
            Backup.Request(Enums.BackupType.Code3);
            Backup.Request(Enums.BackupType.Code3);
            Backup.Request(Enums.BackupType.Code3);
        }

        if (_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) > 90f) End();
        base.Process();
    }

    public override void End()
    {
        foreach (var bikerss in _bikers)
            if (bikerss.Exists())
                bikerss.Dismiss();
        foreach (var vehicless in _cVehicles)
            if (vehicless.Exists())
                vehicless.Dismiss();
        if (_cop1.Exists()) _cop1.Dismiss();
        if (_cop2.Exists()) _cop2.Dismiss();
        if (_cop3.Exists()) _cop3.Dismiss();
        if (_cBlip.Exists()) _cBlip.Delete();

        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        base.End();
    }
}