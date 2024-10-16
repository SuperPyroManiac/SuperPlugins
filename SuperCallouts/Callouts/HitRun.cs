using System.Drawing;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Hit and Run", CalloutProbability.Medium)]
internal class HitRun : SuperCallout
{
    private Ped _bad1;
    private Ped _bad2;
    private Blip _cBlip1;
    private Blip _cBlip2;
    private Blip _cBlip3;
    private Vehicle _cVehicle1;
    private Vehicle _cVehicle2;
    private string _name1;
    private string _name2;
    private string _name3;
    private bool _onScene;
    private bool _onScene2;
    private LHandle _pursuit = Functions.CreatePursuit();
    private Vector3 _spawnPointOffset;
    private UIMenuItem _speakSuspect1;
    private UIMenuItem _speakSuspect2;
    private UIMenuItem _speakVictim;
    private bool _startPursuit;
    private Ped _victim;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 20;
    internal override string CalloutName { get; set; } = "Hit and Run";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Vehicle hit and run.";
        CalloutAdvisory = "Caller reports other driver has left the scene.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_HIT_AND_RUN_01 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Car Accident",
            "Victim reports the other driver has left the scene. Get to the victim as soon as possible.");

        PyroFunctions.SpawnNormalCar(out _cVehicle1, SpawnPoint.Position);
        _cVehicle1.Heading = SpawnPoint.Heading;
        PyroFunctions.DamageVehicle(_cVehicle1, 50, 50);
        _spawnPointOffset = World.GetNextPositionOnStreet(_cVehicle1.Position.Around(100f));
        EntitiesToClear.Add(_cVehicle1);

        PyroFunctions.SpawnNormalCar(out _cVehicle2, _spawnPointOffset);
        PyroFunctions.DamageVehicle(_cVehicle2, 200, 200);
        EntitiesToClear.Add(_cVehicle2);

        _victim = _cVehicle1.CreateRandomDriver();
        _victim.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
        Functions.SetVehicleOwnerName(_cVehicle1, Functions.GetPersonaForPed(_victim).FullName);
        _name1 = Functions.GetPersonaForPed(_victim).FullName;
        EntitiesToClear.Add(_victim);

        _bad1 = _cVehicle2.CreateRandomDriver();
        _bad1.Tasks.CruiseWithVehicle(8f);
        _bad1.IsPersistent = true;
        Functions.SetVehicleOwnerName(_cVehicle2, Functions.GetPersonaForPed(_bad1).FullName);
        _name2 = Functions.GetPersonaForPed(_bad1).FullName;
        EntitiesToClear.Add(_bad1);

        _bad2 = new Ped(SpawnPoint.Position);
        _bad2.WarpIntoVehicle(_cVehicle2, 0);
        _bad2.IsPersistent = true;
        _name3 = Functions.GetPersonaForPed(_bad2).FullName;
        EntitiesToClear.Add(_bad2);

        _speakVictim = new UIMenuItem("Speak with ~y~" + _name1);
        _speakSuspect1 = new UIMenuItem("Speak with ~y~" + _name2);
        _speakSuspect2 = new UIMenuItem("Speak with ~y~" + _name3);
        ConvoMenu.AddItem(_speakVictim);
        ConvoMenu.AddItem(_speakSuspect1);
        ConvoMenu.AddItem(_speakSuspect2);
        _speakVictim.Enabled = false;
        _speakSuspect1.Enabled = false;
        _speakSuspect2.Enabled = false;

        _cBlip1 = _victim.AttachBlip();
        _cBlip1.EnableRoute(Color.Yellow);
        _cBlip1.Color = Color.Yellow;
        BlipsToClear.Add(_cBlip1);
    }

    internal override void CalloutRunning()
    {
        if ( !_onScene && Game.LocalPlayer.Character.DistanceTo(_cVehicle1) < 20f )
        {
            _onScene = true;
            Questioning.Enabled = true;
            _speakVictim!.Enabled = true;
            Game.DisplayNotification($"Speak with the victim to continue! Press: ~{Settings.Interact.GetInstructionalId()}~");
        }

        if ( _startPursuit && !_onScene2 && Game.LocalPlayer.Character.DistanceTo(_cVehicle2) < 50f )
        {
            _startPursuit = false;
            _onScene2 = true;
            Functions.AddPedToPursuit(_pursuit, _bad1);
            Functions.AddPedToPursuit(_pursuit, _bad2);
            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
            _cBlip2?.Delete();
            _cBlip3?.Delete();
        }

        if ( _onScene2 && Game.LocalPlayer.Character.DistanceTo(_cVehicle2) < 50f &&
            !Functions.IsPursuitStillRunning(_pursuit) )
        {
            _onScene2 = false;
            Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
            _speakSuspect1!.Enabled = true;
            _speakSuspect2!.Enabled = true;
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if ( !_bad1 || !_bad2 )
        {
            CalloutEnd(true);
            return;
        }

        if ( selItem == _speakVictim )
            GameFiber.StartNew(delegate
            {
                _speakVictim.Enabled = false;
                Game.DisplaySubtitle("~g~You~s~: What's going on here? Are you ok?", 5000);
                NativeFunction.Natives.x5AD23D40115353AC(_victim, Game.LocalPlayer.Character, -1);
                GameFiber.Wait(5000);
                _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                Game.DisplaySubtitle(
                    "~r~" + _name1 + "~s~: I'm ok, someone hit my car and when I got out they drove off!", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~g~You~s~: Alright, well did you get any information? What did they look like or a vehicle description?",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~r~" + _name1 +
                    "~s~: I gave the dispatch lady the license number, but it was so fast I don't recall any details. Im sorry, can I leave?",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~y~Dispatch~s~: ANPR has located a vehicle matching the license given to us. Dismiss victim and respond ~r~CODE-3",
                    5000);
                GameFiber.Wait(3000);
                Game.DisplaySubtitle(
                    "~g~You~s~: You are good to go, we will be in contact once we get more information on the suspect.");
                GameFiber.Wait(1000);
                _victim?.Dismiss();
                _cVehicle1?.Dismiss();
                _cBlip1?.Delete();
                _startPursuit = true;
                _cBlip2 = _bad1.AttachBlip();
                _cBlip2.Color = Color.Red;
                _cBlip2.EnableRoute(Color.Red);
                BlipsToClear.Add(_cBlip2);
                _cBlip3 = _bad2.AttachBlip();
                _cBlip3.Color = Color.Red;
                BlipsToClear.Add(_cBlip3);
            });
        if ( selItem == _speakSuspect1 )
            GameFiber.StartNew(delegate
            {
                _speakSuspect1.Enabled = false;
                Game.DisplaySubtitle(
                    "~g~You~s~: Why are you running? This could have been a simple ticket and court date for the accident, now you're facing serious charges!",
                    5000);
                NativeFunction.Natives.x5AD23D40115353AC(_bad1, Game.LocalPlayer.Character, -1);
                GameFiber.Wait(5000);
                _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                Game.DisplaySubtitle("~r~" + _name2 + "~s~: Screw you pig, I aint talkin to you!", 5000);
            });
        if ( selItem == _speakSuspect2 )
            GameFiber.StartNew(delegate
            {
                _speakSuspect2.Enabled = false;
                Game.DisplaySubtitle("~g~You~s~: What's going on? Why were you guys running?", 5000);
                GameFiber.Wait(5000);
                NativeFunction.Natives.x5AD23D40115353AC(_bad2, Game.LocalPlayer.Character, -1);
                _bad1.PlayAmbientSpeech("GENERIC_CURSE_MED");
                Game.DisplaySubtitle(
                    "~r~" + _name3 +
                    "~s~: I didnt do nothing at all, I was just chilling and they hit someone and started running, I was like bro, and they were like bruh, so we dipped.",
                    5000);
            });
    }
}