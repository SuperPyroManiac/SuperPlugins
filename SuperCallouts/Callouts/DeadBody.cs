using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Types.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Dead Body", CalloutProbability.Medium)]
internal class DeadBody : SuperCallout
{
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private string _name;
    private UIMenuItem _speakSuspect;
    private Ped _victim;
    private Ped _witness;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 90;
    internal override string CalloutName { get; set; } = "Dead Body";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Reports of an injured person.";
        CalloutAdvisory = "Caller says the person is not breathing.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_AMBULANCE_REQUESTED_01 IN_OR_ON_POSITION",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~y~Medical Emergency",
            "Caller reports an injured person that is not breathing, respond ~r~CODE-3"
        );

        PyroFunctions.SpawnNormalCar(out _cVehicle, SpawnPoint.Position, SpawnPoint.Heading);
        EntitiesToClear.Add(_cVehicle);

        _witness = new Ped(_cVehicle.GetOffsetPositionFront(-9f));
        _witness.IsPersistent = true;
        _witness.BlockPermanentEvents = true;
        _name = Functions.GetPersonaForPed(_witness).FullName;
        EntitiesToClear.Add(_witness);

        _victim = new Ped(_witness.GetOffsetPositionFront(-2f));
        _victim.IsPersistent = true;
        _victim.BlockPermanentEvents = true;
        EntitiesToClear.Add(_victim);

        _victim.Tasks.Cower(-1);

        NativeFunction.Natives.x5AD23D40115353AC(_witness, _victim, -1);
        _witness.Tasks.Cower(-1);

        _cBlip = _cVehicle.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name);
        ConvoMenu.AddItem(_speakSuspect);
    }

    internal override void CalloutRunning()
    {
        if (!_witness)
        {
            CalloutEnd(true);
            return;
        }

        if (_witness.IsDead)
        {
            _speakSuspect!.Enabled = false;
            _speakSuspect.RightLabel = "~r~Dead";
        }
    }

    internal override void CalloutOnScene()
    {
        if (!_witness || !_victim)
        {
            CalloutEnd(true);
            return;
        }

        NativeFunction.Natives.x5AD23D40115353AC(_witness, _victim, -1);
        _witness.Tasks.Cower(-1);
        _victim.Kill();
        _cBlip?.DisableRoute();
        Questioning.Enabled = true;
        Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        try
        {
            if (!_witness)
            {
                CalloutEnd(true);
                return;
            }

            if (selItem == _speakSuspect)
                GameFiber.StartNew(
                    delegate
                    {
                        _speakSuspect.Enabled = false;
                        Game.DisplaySubtitle("~g~You~s~: Do you know what happened to this person?", 4000);
                        NativeFunction.Natives.x5AD23D40115353AC(_witness, Game.LocalPlayer.Character, -1);
                        GameFiber.Wait(4000);
                        _witness.PlayAmbientSpeech("GENERIC_CURSE_MED");
                        Game.DisplaySubtitle(
                            "~r~" + _name + "~s~: I don't know, I just found them here and called you guys right away!",
                            4000
                        );
                        GameFiber.Wait(4000);
                        Game.DisplaySubtitle("~g~You~s~: Do you know who this is?", 4000);
                        GameFiber.Wait(4000);
                        Game.DisplaySubtitle(
                            "~r~" + _name + "~s~: I don't know anything about them, sorry I wish I could help more.",
                            4000
                        );
                        GameFiber.Wait(4000);
                        Game.DisplaySubtitle(
                            "~g~You~s~: It's alright, thank you for your time and the call. You are free to go home.",
                            4000
                        );
                        if (_witness.Exists())
                            _witness.Dismiss();
                    }
                );
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            CalloutEnd(true);
        }

        base.Conversations(sender, selItem, index);
    }
}
