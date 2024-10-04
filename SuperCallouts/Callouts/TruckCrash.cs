using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.CustomScenes;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.API.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Truck Crash", CalloutProbability.Low)]
internal class TruckCrash : SuperCallout
{
    private Vehicle _car1;
    private Vehicle _car2;
    private Blip _cBlip;
    private UIMenuItem _speakSuspect;
    private Vehicle _truck;
    private Ped _victim;
    private Ped _victim2;
    private Ped _victim3;
    internal override Location SpawnPoint { get; set; } = new(2455.644f, -186.7955f, 87.83904f);
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Truck Accident";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Large truck tipped over.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_AMBULANCE_REQUESTED_02 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Truck Accident",
            "Reports of a truck tipped over on the highway. Respond ~r~CODE-3");

        TruckCrashSetup.ConstructTrucksScene(out _victim, out _victim2, out _victim3, out _truck, out _car1,
            out _car2);
        _victim.IsPersistent = true;
        EntitiesToClear.Add(_victim);
        _victim2.IsPersistent = true;
        EntitiesToClear.Add(_victim2);
        _victim3.IsPersistent = true;
        EntitiesToClear.Add(_victim3);
        _truck.IsPersistent = true;
        EntitiesToClear.Add(_truck);
        _car1.IsPersistent = true;
        EntitiesToClear.Add(_car1);
        _car2.IsPersistent = true;
        EntitiesToClear.Add(_car2);

        _cBlip = _truck.AttachBlip();
        _cBlip.EnableRoute(Color.Yellow);
        _cBlip.Color = Color.Yellow;
        BlipsToClear.Add(_cBlip);

        _speakSuspect = new UIMenuItem("Speak with ~y~victims");
        ConvoMenu.AddItem(_speakSuspect);
    }

    internal override void CalloutOnScene()
    {
        Questioning.Enabled = true;
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(delegate
            {
                _speakSuspect.Enabled = false;
                NativeFunction.Natives.x5AD23D40115353AC(_victim, Game.LocalPlayer.Character,
                    -1);
                NativeFunction.Natives.x5AD23D40115353AC(_victim2, Game.LocalPlayer.Character,
                    -1);
                Game.DisplaySubtitle("~g~Me: ~w~What happened? Are you all ok?", 4000);
                GameFiber.Wait(4000);
                Game.DisplaySubtitle(
                    "~y~Victims: ~w~We're ok but the truck driver needs help! We were just going home and he flipped over!",
                    4000);
            });
        base.Conversations(sender, selItem, index);
    }
}