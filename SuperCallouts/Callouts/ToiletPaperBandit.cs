using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Toilet Paper Bandit", CalloutProbability.Low)]
internal class ToiletPaperBandit : SuperCallout
{
    private Ped _suspect;
    private Blip _suspectBlip;
    private Vehicle _vehicle;
    private string _suspectName;
    private UIMenuItem _speakSuspect;

    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Toilet Paper Bandit";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a person stealing toilet paper.";
        CalloutAdvisory = "Caller reports a person is stealing toilet paper from a store.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_ROBBERY_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Toilet Paper Bandit",
            "Reports of a person stealing toilet paper from a store. Respond ~r~CODE-3"
        );

        SpawnVehicle();
        SpawnSuspect();
        CreateBlip();
        SetupConversation();
    }

    private void SpawnVehicle()
    {
        PyroFunctions.SpawnNormalCar(out _vehicle, SpawnPoint.Position);
        _vehicle.Heading = SpawnPoint.Heading;
        EntitiesToClear.Add(_vehicle);
    }

    private void SpawnSuspect()
    {
        _suspect = _vehicle.CreateRandomDriver();
        _suspect.IsPersistent = true;
        _suspect.BlockPermanentEvents = true;
        _suspectName = Functions.GetPersonaForPed(_suspect).FullName;
        _suspect.Metadata.searchPed = "~r~Toilet Paper~s~, ~g~wallet~s~";
        EntitiesToClear.Add(_suspect);
    }

    private void CreateBlip()
    {
        _suspectBlip = _suspect.AttachBlip();
        _suspectBlip.Color = Color.Red;
        _suspectBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_suspectBlip);
    }

    private void SetupConversation()
    {
        _speakSuspect = new UIMenuItem($"Speak with ~y~{_suspectName}");
        ConvoMenu.AddItem(_speakSuspect);
    }

    internal override void CalloutOnScene()
    {
        if (!_suspect)
        {
            CalloutEnd(true);
            return;
        }

        _suspectBlip?.DisableRoute();
        Questioning.Enabled = true;

        // Determine suspect behavior
        DetermineSuspectBehavior();
    }

    private void DetermineSuspectBehavior()
    {
        var random = new Random(DateTime.Now.Millisecond);
        var choice = random.Next(1, 4);

        switch (choice)
        {
            case 1: // Suspect flees
                var pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _suspect);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;

            case 2: // Suspect fights
                _suspect.Tasks.FightAgainst(Player);
                break;

            case 3: // Suspect complies
                _suspect.Tasks.PutHandsUp(-1, Player);
                break;
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (!_suspect)
        {
            CalloutEnd(true);
            return;
        }

        if (selItem == _speakSuspect)
        {
            GameFiber.StartNew(
                delegate
                {
                    _speakSuspect.Enabled = false;
                    Game.DisplaySubtitle("~g~You~s~: I've received reports that you stole toilet paper from a store, is that true?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: I didn't steal anything officer, I bought it fair and square!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: Do you have a receipt for it?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: No, I threw it away already.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: The store owner says you didn't pay for it, and we have you on camera.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: Fine, I took it. But I needed it! There's none left anywhere!", 5000);
                }
            );
        }

        base.Conversations(sender, selItem, index);
    }
}
