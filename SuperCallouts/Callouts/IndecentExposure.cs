using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Utils;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Models.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Indecent Exposure", CalloutProbability.Medium)]
internal class IndecentExposure : SuperCallout
{
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(60f, 320f)));
    internal override float OnSceneDistance { get; set; } = 15;
    internal override string CalloutName { get; set; } = "Indecent Exposure";
    private Ped _naked;
    private Blip _blip;
    private readonly int _rNd = new Random(DateTime.Now.Millisecond).Next(1);

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Naked person running around.";
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

        var models = new[] { "a_m_m_acult_01", "a_f_m_fatcult_01" };
        _naked = new Ped(models[new Random(DateTime.Now.Millisecond).Next(models.Length)], SpawnPoint.Position, 0);
        _naked.IsPersistent = true;
        _naked.BlockPermanentEvents = true;
        _naked.Tasks.Wander();
        EntitiesToClear.Add(_naked);

        _blip = _naked.AttachBlip();
        _blip.EnableRoute(Color.Yellow);
        _blip.Color = Color.Yellow;
        BlipsToClear.Add(_blip);
    }

    internal override void CalloutOnScene()
    {
        if (!_naked)
        {
            CalloutEnd(true);
            return;
        }

        _blip?.Delete();
        _naked.BlockPermanentEvents = false;
        switch (_rNd)
        {
            case 0:
                var pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _naked);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;
            case 1:
                CommonUtils.SetDrunkOld(_naked, true);
                _naked.Tasks.FightAgainst(Player, -1);
                break;
        }
    }
}
