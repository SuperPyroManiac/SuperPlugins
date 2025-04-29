using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Extensions;
using PyroCommon.Models;
using PyroCommon.Utils;
using Rage;
using Rage.Native;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Models.Location;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInfo("[SC] Fight", CalloutProbability.Medium)]
internal class Fight : SuperCallout
{
    private Ped _victim;
    private Ped _suspect;
    private Blip _blip;
    internal override Location SpawnPoint { get; set; } = CommonUtils.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 35;
    internal override string CalloutName { get; set; } = "Fight";

    internal override void CalloutPrep()
    {
        CalloutMessage = $"~r~{Settings.EmergencyNumber} Report:~s~ Reports of a fight.";
        CalloutAdvisory = "Caller said the the people are screaming and yelling before hanging up.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE PYRO_FIGHT IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~y~Fight",
            "Caller reports two people fighting in the street, respond ~r~CODE-3"
        );

        SpawnPeds();
        CreateBlip();
    }

    private void SpawnPeds()
    {
        _victim = CommonUtils.SpawnPed(SpawnPoint);
        _victim.RelationshipGroup = new RelationshipGroup("VICTIM");

        _suspect = CommonUtils.SpawnPed(new Location(SpawnPoint.Position.Around2D(1.5f)));
        _suspect.RelationshipGroup = new RelationshipGroup("SUSPECT");

        _victim.Tasks.FaceEntity(_suspect);
        _suspect.Tasks.FaceEntity(_victim);

        EntitiesToClear.Add(_victim);
        EntitiesToClear.Add(_suspect);
    }

    private void CreateBlip()
    {
        _blip = CommonUtils.CreateSearchBlip(SpawnPoint, Color.Red, true, true);
        BlipsToClear.Add(_blip);
    }

    internal override void CalloutRunning()
    {
        if (!OnScene)
        {
            _suspect?.PlayAmbientSpeech("GENERIC_CURSE_MED");
            _victim?.PlayAmbientSpeech("GENERIC_CURSE_MED");
        }
    }

    internal override void CalloutOnScene()
    {
        if (!_blip || !_suspect || !_victim)
        {
            CalloutEnd(true);
            return;
        }

        UpdateBlip();
        SetupPeds();
        HandleScenario();
    }

    private void UpdateBlip()
    {
        _blip.Position = SpawnPoint.Position;
        _blip.Scale = 20;
        _blip.DisableRoute();
    }

    private void SetupPeds()
    {
        _victim.BlockPermanentEvents = false;
        _suspect.BlockPermanentEvents = false;
    }

    private void HandleScenario()
    {
        switch (new Random(DateTime.Now.Millisecond).Next(1, 4))
        {
            case 1: // Fighting scenario
                LogUtils.Info("Callout Scene 1");
                Game.SetRelationshipBetweenRelationshipGroups("SUSPECT", "VICTIM", Relationship.Hate);
                Game.SetRelationshipBetweenRelationshipGroups("VICTIM", "SUSPECT", Relationship.Hate);
                _suspect.SetResistance(Enums.ResistanceAction.Uncooperative, false, 100);
                NativeFunction.Natives.x19D1B791CB3670FE(_suspect, _victim);
                NativeFunction.Natives.x19D1B791CB3670FE(_victim, _suspect);
                GameFiber.Wait(2000);
                _suspect.Tasks.FightAgainst(_victim, 5);
                break;

            case 2: // Intimidation scenario
                LogUtils.Info("Callout Scene 2");
                _victim.Tasks.Cower(-1);
                _suspect.Tasks.FaceEntity(Player);
                _suspect.SetResistance(Enums.ResistanceAction.Attack, false, 100);
                break;

            case 3: // Drunk intimidation scenario
                LogUtils.Info("Callout Scene 3");
                _victim.Tasks.Cower(-1);
                _suspect.Tasks.FaceEntity(Player);
                _suspect.SetDrunk(Enums.DrunkState.ExtremelyDrunk);
                _suspect.SetResistance(Enums.ResistanceAction.Flee, true, 100);
                break;
        }
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        Game.SetRelationshipBetweenRelationshipGroups("SUSPECT", "VICTIM", Relationship.Neutral);
        Game.SetRelationshipBetweenRelationshipGroups("SUSPECT", "COP", Relationship.Neutral);
        Game.SetRelationshipBetweenRelationshipGroups("VICTIM", "SUSPECT", Relationship.Neutral);
        base.CalloutEnd(forceCleanup);
    }
}
