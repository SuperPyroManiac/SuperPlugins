using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Indecent Exposure", CalloutProbability.Medium)]
internal class IndecentExposure : SuperCallout
{
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(60f, 320f)));
    internal override float OnSceneDistance { get; set; } = 15;
    internal override string CalloutName { get; set; } = "Indecent Exposure";

    private Ped _suspect;
    private Blip _suspectBlip;
    private readonly int _scenarioType = new Random(DateTime.Now.Millisecond).Next(1);

    internal override void CalloutPrep()
    {
        CalloutMessage = $"~r~{Settings.EmergencyNumber} Report:~s~ Naked person running around.";
        CalloutAdvisory = "Reports of a person running around naked.";
        Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_11_351_02 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_nakeds",
            "~b~Dispatch",
            "~r~Indecent Exposure",
            "Reports of a person running around outside naked. Respond ~y~CODE-2"
        );

        SpawnSuspect();
        CreateBlip();
    }

    private void SpawnSuspect()
    {
        var models = new[] { "a_m_m_acult_01", "a_f_m_fatcult_01" };
        _suspect = new Ped(models[new Random(DateTime.Now.Millisecond).Next(models.Length)], SpawnPoint.Position, 0);
        _suspect.IsPersistent = true;
        _suspect.BlockPermanentEvents = true;
        _suspect.Tasks.Wander();
        EntitiesToClear.Add(_suspect);
    }

    private void CreateBlip()
    {
        _suspectBlip = _suspect.AttachBlip();
        _suspectBlip.EnableRoute(Color.Yellow);
        _suspectBlip.Color = Color.Yellow;
        BlipsToClear.Add(_suspectBlip);
    }

    internal override void CalloutOnScene()
    {
        if (!_suspect)
        {
            CalloutEnd(true);
            return;
        }

        _suspectBlip?.Delete();
        _suspect.BlockPermanentEvents = false;

        ExecuteScenario();
    }

    private void ExecuteScenario()
    {
        switch (_scenarioType)
        {
            case 0: // Fleeing suspect
                var pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _suspect);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;

            case 1: // Aggressive drunk suspect
                PyroFunctions.SetDrunkOld(_suspect, true);
                _suspect.Tasks.FightAgainst(Player, -1);
                break;
        }
    }
}
