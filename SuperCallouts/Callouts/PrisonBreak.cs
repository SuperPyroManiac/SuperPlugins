using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using SuperCallouts.CustomScenes;
using Functions = LSPD_First_Response.Mod.API.Functions;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Prison Break", CalloutProbability.Low)]
internal class PrisonBreak : Callout
{
    private readonly Vector3 _spawnPoint = new(1970.794f, 2624.078f, 46.00704f);
    private Blip _prisoner1Blip;
    private Blip _prisoner2Blip;
    private Blip _prisoner3Blip;
    private Blip _prisoner4Blip;
    private Blip _prisoner5Blip;
    private Vehicle _prisonBus;
    private bool _onScene;
    private Ped _prisoner1;
    private Ped _prisoner2;
    private Ped _prisoner3;
    private Ped _prisoner4;
    private Ped _prisoner5;

    public override bool OnBeforeCalloutDisplayed()
    {
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 80f);
        CalloutMessage = "~r~EMERGENCY~b~ SADOC Report:~s~ Prisoner(s) have escaped.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_SUSPECT_ON_THE_RUN_02 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        Log.Info("PrisonBreak callout accepted...");
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Prison Break",
            "DOC has reported multiple groups of prisoners have escaped! They are occupied with another group and need local police assistance. ~r~CODE-3"
        );

        SetupPrisonBreakScene();
        CreateBlips();
        SetupRelationships();
        LoadPrisonersIntoBus();

        Game.DisplaySubtitle("Get to the ~r~scene~w~! Proceed with ~r~CAUTION~w~!", 10000);
        return base.OnCalloutAccepted();
    }

    private void SetupPrisonBreakScene()
    {
        PrisonbreakSetup.ConstructPrisonBreakSetupScene(out _prisoner1, out _prisoner2, out _prisoner3, out _prisoner4, out _prisoner5);

        SetPrisonersAsWanted();
        SpawnPrisonBus();
    }

    private void SetPrisonersAsWanted()
    {
        PyroFunctions.SetWanted(_prisoner1, true);
        PyroFunctions.SetWanted(_prisoner2, true);
        PyroFunctions.SetWanted(_prisoner3, true);
        PyroFunctions.SetWanted(_prisoner4, true);
        PyroFunctions.SetWanted(_prisoner5, true);

        _prisoner1.IsPersistent = true;
        _prisoner2.IsPersistent = true;
        _prisoner3.IsPersistent = true;
        _prisoner4.IsPersistent = true;
        _prisoner5.IsPersistent = true;
    }

    private void SpawnPrisonBus()
    {
        _prisonBus = new Vehicle("PBUS", _prisoner1.GetOffsetPositionFront(4)) { IsPersistent = true, IsStolen = true };
    }

    private void CreateBlips()
    {
        _prisoner1Blip = _prisoner1.AttachBlip();
        _prisoner1Blip.Scale = .75f;
        _prisoner1Blip.EnableRoute(Color.Red);
        _prisoner1Blip.Color = Color.Red;

        _prisoner2Blip = _prisoner2.AttachBlip();
        _prisoner2Blip.Scale = .75f;
        _prisoner2Blip.Color = Color.Red;

        _prisoner3Blip = _prisoner3.AttachBlip();
        _prisoner3Blip.Scale = .75f;
        _prisoner3Blip.Color = Color.Red;

        _prisoner4Blip = _prisoner4.AttachBlip();
        _prisoner4Blip.Scale = .75f;
        _prisoner4Blip.Color = Color.Red;

        _prisoner5Blip = _prisoner5.AttachBlip();
        _prisoner5Blip.Scale = .75f;
        _prisoner5Blip.Color = Color.Red;
    }

    private void SetupRelationships()
    {
        Game.LocalPlayer.Character.RelationshipGroup = "COP";
        Game.SetRelationshipBetweenRelationshipGroups("PRISONERS", "COP", Relationship.Hate);
    }

    private void LoadPrisonersIntoBus()
    {
        _prisoner1.WarpIntoVehicle(_prisonBus, -1);
        _prisoner2.WarpIntoVehicle(_prisonBus, 0);
        _prisoner3.WarpIntoVehicle(_prisonBus, 1);
        _prisoner4.WarpIntoVehicle(_prisonBus, 2);
        _prisoner5.WarpIntoVehicle(_prisonBus, 3);
    }

    public override void Process()
    {
        if (Game.IsKeyDown(Settings.EndCall))
            End();

        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 90f)
        {
            _onScene = true;
            HandleOnSceneArrival();
        }

        base.Process();
    }

    private void HandleOnSceneArrival()
    {
        Game.DisplaySubtitle("Suspects spotted, they appear to have stolen a bus!", 5000);
        _prisoner1Blip?.DisableRoute();

        StartPrisonerPursuit();
        RequestSWATBackup();
        Game.DisplayHelp("You can end the pursuit to stop the callout at any time!", 7000);
    }

    private void StartPrisonerPursuit()
    {
        var pursuit = Functions.CreatePursuit();
        Functions.AddPedToPursuit(pursuit, _prisoner1);
        Functions.AddPedToPursuit(pursuit, _prisoner2);
        Functions.AddPedToPursuit(pursuit, _prisoner3);
        Functions.AddPedToPursuit(pursuit, _prisoner4);
        Functions.AddPedToPursuit(pursuit, _prisoner5);
        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
    }

    private void RequestSWATBackup()
    {
        Functions.PlayScannerAudioUsingPosition("DISPATCH_SWAT_UNITS_FROM_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_01", _spawnPoint);
    }

    public override void End()
    {
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        CleanupEntities();
        DeleteBlips();
        base.End();
    }

    private void CleanupEntities()
    {
        _prisoner1?.Dismiss();
        _prisoner2?.Dismiss();
        _prisoner3?.Dismiss();
        _prisoner4?.Dismiss();
        _prisoner5?.Dismiss();
        _prisonBus?.Dismiss();
    }

    private void DeleteBlips()
    {
        _prisoner1Blip?.Delete();
        _prisoner2Blip?.Delete();
        _prisoner3Blip?.Delete();
        _prisoner4Blip?.Delete();
        _prisoner5Blip?.Delete();
    }
}
