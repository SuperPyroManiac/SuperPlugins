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

[CalloutInfo("[SC] Vandalizing", CalloutProbability.Medium)]
internal class Vandalizing : SuperCallout
{
    private Ped _suspect;
    private Blip _suspectBlip;
    private string _suspectName;
    private UIMenuItem _speakSuspect;

    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 25;
    internal override string CalloutName { get; set; } = "Vandalizing";

    internal override void CalloutPrep()
    {
        CalloutMessage = $"~r~{Settings.EmergencyNumber} Report:~s~ Reports of a person vandalizing property.";
        CalloutAdvisory = "Caller reports a person is spray painting a wall.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_02_01", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Vandalizing", "Reports of a person vandalizing property. Respond ~y~CODE-2");

        SpawnSuspect();
        CreateBlip();
        SetupConversation();
    }

    private void SpawnSuspect()
    {
        _suspect = new Ped(SpawnPoint.Position);
        _suspect.IsPersistent = true;
        _suspect.BlockPermanentEvents = true;
        _suspectName = Functions.GetPersonaForPed(_suspect).FullName;
        _suspect.Metadata.searchPed = "~r~Spray paint can~s~, ~g~wallet~s~";
        EntitiesToClear.Add(_suspect);
    }

    private void CreateBlip()
    {
        _suspectBlip = _suspect.AttachBlip();
        _suspectBlip.Color = Color.Yellow;
        _suspectBlip.EnableRoute(Color.Yellow);
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

        DetermineSuspectBehavior();
    }

    private void DetermineSuspectBehavior()
    {
        var random = new Random(DateTime.Now.Millisecond);
        var behavior = random.Next(1, 4);

        switch (behavior)
        {
            case 1: // Suspect flees
                var pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, _suspect);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                break;

            case 2: // Suspect is aggressive
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
                    Game.DisplaySubtitle("~g~You~s~: Excuse me, I've received reports of someone vandalizing property in this area.", 5000);
                    _suspect.Tasks.FaceEntity(Player);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: It wasn't me officer, I was just walking by.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: Do you mind if I search you for spray paint?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: I don't consent to searches.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: I have reasonable suspicion that you were involved in vandalism, which gives me grounds to search you.", 5000);
                }
            );
        }

        base.Conversations(sender, selItem, index);
    }
}
