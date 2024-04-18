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
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 35;
    internal override string CalloutName { get; set; } = "Fight";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Reports of a fight.";
        CalloutAdvisory = "Caller said the the people are screaming and yelling before hanging up.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_AMBULANCE_REQUESTED_01 IN_OR_ON_POSITION",
            SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~y~Fight",
            "Caller reports two people fighting in the street, respond ~r~CODE-3");

        _victim = new Ped();
        _victim.IsPersistent = true;
        _victim.BlockPermanentEvents = true;
        
        _suspect = new Ped();
        _suspect.IsPersistent = true;
        _suspect.BlockPermanentEvents = true;
        
        //TASK_TURN_PED_TO_FACE_ENTITY(Ped ped, Entity entity, int duration) // 0x5AD23D40115353AC 0x3C37C767 b323
        NativeFunction.Natives.x5AD23D40115353AC(_victim, _suspect, -1);
    }

    internal override void CalloutOnScene()
    {
        //TASK_AGITATED_ACTION_CONFRONT_RESPONSE(Ped ped, Ped ped2) // 0x19D1B791CB3670FE b877
        NativeFunction.Natives.x19D1B791CB3670FE(_suspect, _victim);
    }
}