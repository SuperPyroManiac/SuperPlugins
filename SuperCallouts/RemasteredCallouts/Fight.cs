using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Rage.Native;
using Functions = LSPD_First_Response.Mod.API.Functions;

namespace SuperCallouts.RemasteredCallouts;

[CalloutInterface("[SC] Fight", CalloutProbability.Medium, "Reports of a fight on the street, limited details", "Code 3")]
internal class Fight : SuperCallout
{
    private Ped _victim;
    private Ped _suspect;
    private Blip _blip;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 35;
    internal override string CalloutName { get; set; } = "Fight";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Reports of a fight.";
        CalloutAdvisory = "Caller said the the people are screaming and yelling before hanging up.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_ASSAULT_01 IN_OR_ON_POSITION",
            SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~y~Fight",
            "Caller reports two people fighting in the street, respond ~r~CODE-3");

        _victim = PyroFunctions.SpawnPed(SpawnPoint);
        _suspect = PyroFunctions.SpawnPed(new Location(SpawnPoint.Position.Around2D(1.5f)));
        EntitiesToClear.Add(_victim);
        EntitiesToClear.Add(_suspect);

        _blip = PyroFunctions.CreateSearchBlip(SpawnPoint, Color.Red, true);
        BlipsToClear.Add(_blip);
        
        //TASK_TURN_PED_TO_FACE_ENTITY(Ped ped, Entity entity, int duration) // 0x5AD23D40115353AC 0x3C37C767 b323
        NativeFunction.Natives.x5AD23D40115353AC(_victim, _suspect, -1);
        NativeFunction.Natives.x5AD23D40115353AC(_suspect, _victim, -1);
    }

    internal override void CalloutRunning()
    {
        if (!OnScene)
        {
            _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
            _victim.PlayAmbientSpeech("GENERIC_CURSE_MED");
        }
    }

    internal override void CalloutOnScene()
    {
        _blip.Position = SpawnPoint.Position;
        _blip.Scale = 20;
        _blip.DisableRoute();

        _victim.BlockPermanentEvents = false;
        _suspect.BlockPermanentEvents = false;

        switch (new Random().Next(1, 4))
        {
            case 1:
                //TASK_AGITATED_ACTION_CONFRONT_RESPONSE(Ped ped, Ped ped2) // 0x19D1B791CB3670FE b877
                NativeFunction.Natives.x19D1B791CB3670FE(_suspect, _victim);
                GameFiber.Wait(2000);
                _suspect.Tasks.FightAgainst(_victim, 5);
                break;
            case 2:
                _victim.Tasks.Cower(-1);
                //TASK_AGITATED_ACTION_CONFRONT_RESPONSE(Ped ped, Ped ped2) // 0x19D1B791CB3670FE b877
                NativeFunction.Natives.x19D1B791CB3670FE(_suspect, Player);
                GameFiber.Wait(2000);
                _suspect.Tasks.FightAgainst(Player, 5);
                break;
            case 3:
                PyroFunctions.SetDrunk(_suspect, true);
                _victim.Tasks.Cower(-1);
                var pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _suspect);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;
        }
    }
}