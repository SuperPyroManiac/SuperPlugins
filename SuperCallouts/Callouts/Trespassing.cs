using System;
using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Trespassing", CalloutProbability.Medium)]
internal class Trespassing : SuperCallout
{
    private Ped _suspect;
    private Blip _suspectBlip;
    private Ped _homeowner;
    private string _suspectName;
    private string _homeownerName;
    private UIMenuItem _speakSuspect;
    private UIMenuItem _speakHomeowner;

    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Trespassing";

    internal override void CalloutPrep()
    {
        CalloutMessage = $"~r~{Settings.EmergencyNumber} Report:~s~ Reports of a trespasser.";
        CalloutAdvisory = "Caller reports a person is on their property and refusing to leave.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_02_01", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Trespassing", "Reports of a person trespassing on private property. Respond ~y~CODE-2");

        SpawnSuspect();
        SpawnHomeowner();
        CreateBlip();
        SetupConversations();
    }

    private void SpawnSuspect()
    {
        _suspect = new Ped(SpawnPoint.Position);
        _suspect.IsPersistent = true;
        _suspect.BlockPermanentEvents = true;
        _suspectName = Functions.GetPersonaForPed(_suspect).FullName;
        EntitiesToClear.Add(_suspect);
    }

    private void SpawnHomeowner()
    {
        _homeowner = new Ped(_suspect.GetOffsetPositionFront(2f));
        _homeowner.IsPersistent = true;
        _homeowner.BlockPermanentEvents = true;
        _homeownerName = Functions.GetPersonaForPed(_homeowner).FullName;
        EntitiesToClear.Add(_homeowner);
    }

    private void CreateBlip()
    {
        _suspectBlip = _suspect.AttachBlip();
        _suspectBlip.Color = Color.Yellow;
        _suspectBlip.EnableRoute(Color.Yellow);
        BlipsToClear.Add(_suspectBlip);
    }

    private void SetupConversations()
    {
        _speakSuspect = new UIMenuItem($"Speak with ~y~{_suspectName}");
        _speakHomeowner = new UIMenuItem($"Speak with ~b~{_homeownerName}");
        ConvoMenu.AddItem(_speakSuspect);
        ConvoMenu.AddItem(_speakHomeowner);
    }

    internal override void CalloutOnScene()
    {
        if (!_suspect || !_homeowner)
        {
            CalloutEnd(true);
            return;
        }

        _suspectBlip?.DisableRoute();
        Questioning.Enabled = true;

        SetupInitialInteraction();
    }

    private void SetupInitialInteraction()
    {
        _suspect.Tasks.FaceEntity(_homeowner);
        _homeowner.Tasks.FaceEntity(_suspect);

        // Determine scenario type
        var random = new Random(DateTime.Now.Millisecond);
        var scenario = random.Next(1, 4);

        switch (scenario)
        {
            case 1: // Argument
                _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                _homeowner.PlayAmbientSpeech("GENERIC_CURSE_HIGH");
                break;

            case 2: // Suspect is drunk
                PyroFunctions.SetDrunkOld(_suspect, true);
                _suspect.Metadata.stpAlcoholDetected = true;
                break;

            case 3: // Suspect is aggressive
                _suspect.Tasks.FightAgainst(_homeowner);
                break;
        }
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (!_suspect || !_homeowner)
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
                    Game.DisplaySubtitle("~g~You~s~: Hello, I'm Officer " + Functions.GetPersonaForPed(Player).FullName + ". What's going on here?", 5000);
                    _suspect.Tasks.FaceEntity(Player);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: I'm not doing anything wrong officer, I'm just trying to talk to this person.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: They've asked you to leave their property, and you need to respect that.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: But I need to talk to them! It's important!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~g~You~s~: I understand, but if they don't want to talk to you, you need to leave. Otherwise, I'll have to arrest you for trespassing.",
                        5000
                    );
                }
            );
        }

        if (selItem == _speakHomeowner)
        {
            GameFiber.StartNew(
                delegate
                {
                    _speakHomeowner.Enabled = false;
                    Game.DisplaySubtitle("~g~You~s~: Hello, I'm Officer " + Functions.GetPersonaForPed(Player).FullName + ". Can you tell me what's happening?", 5000);
                    _homeowner.Tasks.FaceEntity(Player);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~b~{_homeownerName}~s~: This person won't leave my property! I've asked them multiple times but they refuse to go.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: Do you know this person?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~b~{_homeownerName}~s~: Yes, they're my ex and they keep showing up uninvited. I want them gone!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: I understand. I'll handle it and make sure they leave.", 5000);
                }
            );
        }

        base.Conversations(sender, selItem, index);
    }
}
