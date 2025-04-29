using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Utils;
using Rage;
using SuperCallouts.CustomScenes;
using Functions = LSPD_First_Response.Mod.API.Functions;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Prison Break", CalloutProbability.Low)]
internal class PrisonBreak : Callout
{
    private readonly Vector3 _spawnPoint = new(1970.794f, 2624.078f, 46.00704f);
    private Blip _cBlip1;
    private Blip _cBlip2;
    private Blip _cBlip3;
    private Blip _cBlip4;
    private Blip _cBlip5;
    private Vehicle _cVehicle;
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
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_SUSPECT_ON_THE_RUN_02 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            _spawnPoint
        );
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        LogUtils.Info("PrisonBreak callout accepted...");
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Prison Break",
            "DOC has reported multiple groups of prisoners have escaped! They are occupied with another group and need local police assistance. ~r~CODE-3"
        );
        PrisonbreakSetup.ConstructPrisonBreakSetupScene(out _prisoner1, out _prisoner2, out _prisoner3, out _prisoner4, out _prisoner5);
        CommonUtils.SetWanted(_prisoner1, true);
        CommonUtils.SetWanted(_prisoner2, true);
        CommonUtils.SetWanted(_prisoner3, true);
        CommonUtils.SetWanted(_prisoner4, true);
        CommonUtils.SetWanted(_prisoner5, true);
        _cVehicle = new Vehicle("PBUS", _prisoner1.GetOffsetPositionFront(4));
        _cVehicle.IsPersistent = true;
        _cVehicle.IsStolen = true;
        _prisoner1.IsPersistent = true;
        _prisoner2.IsPersistent = true;
        _prisoner3.IsPersistent = true;
        _prisoner4.IsPersistent = true;
        _prisoner5.IsPersistent = true;
        _cBlip1 = _prisoner1.AttachBlip();
        _cBlip1.Scale = .75f;
        _cBlip1.EnableRoute(Color.Red);
        _cBlip1.Color = Color.Red;
        _cBlip2 = _prisoner2.AttachBlip();
        _cBlip2.Scale = .75f;
        _cBlip2.Color = Color.Red;
        _cBlip3 = _prisoner3.AttachBlip();
        _cBlip3.Scale = .75f;
        _cBlip3.Color = Color.Red;
        _cBlip4 = _prisoner4.AttachBlip();
        _cBlip4.Scale = .75f;
        _cBlip4.Color = Color.Red;
        _cBlip5 = _prisoner5.AttachBlip();
        _cBlip5.Scale = .75f;
        _cBlip5.Color = Color.Red;
        Game.LocalPlayer.Character.RelationshipGroup = "COP";
        Game.SetRelationshipBetweenRelationshipGroups("PRISONERS", "COP", Relationship.Hate);
        _prisoner1.WarpIntoVehicle(_cVehicle, -1);
        _prisoner2.WarpIntoVehicle(_cVehicle, 0);
        _prisoner3.WarpIntoVehicle(_cVehicle, 1);
        _prisoner4.WarpIntoVehicle(_cVehicle, 2);
        _prisoner5.WarpIntoVehicle(_cVehicle, 3);
        Game.DisplaySubtitle("Get to the ~r~scene~w~! Proceed with ~r~CAUTION~w~!", 10000);
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        if (Game.IsKeyDown(Settings.EndCall))
            End();
        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 90f)
        {
            _onScene = true;
            Game.DisplaySubtitle("Suspects spotted, they appear to have stolen a bus!", 5000);
            _cBlip1?.DisableRoute();
            var pursuit = Functions.CreatePursuit();
            Functions.AddPedToPursuit(pursuit, _prisoner1);
            Functions.AddPedToPursuit(pursuit, _prisoner2);
            Functions.AddPedToPursuit(pursuit, _prisoner3);
            Functions.AddPedToPursuit(pursuit, _prisoner4);
            Functions.AddPedToPursuit(pursuit, _prisoner5);
            Functions.SetPursuitIsActiveForPlayer(pursuit, true);
            Functions.PlayScannerAudioUsingPosition("DISPATCH_SWAT_UNITS_FROM_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_01", _spawnPoint);
            Game.DisplayHelp("You can end the pursuit to stop the callout at any time!", 7000);
        }

        base.Process();
    }

    public override void End()
    {
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        if (_prisoner1)
            _prisoner1.Dismiss();
        if (_prisoner2)
            _prisoner2.Dismiss();
        if (_prisoner3)
            _prisoner3.Dismiss();
        if (_prisoner4)
            _prisoner4.Dismiss();
        if (_prisoner5)
            _prisoner5.Dismiss();
        if (_cVehicle)
            _cVehicle.Dismiss();
        _cBlip1?.Delete();
        _cBlip2?.Delete();
        _cBlip3?.Delete();
        _cBlip4?.Delete();
        _cBlip5?.Delete();
        base.End();
    }
}
