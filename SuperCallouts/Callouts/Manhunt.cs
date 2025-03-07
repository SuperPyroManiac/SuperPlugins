using System.Drawing;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Manhunt", CalloutProbability.Low)]
internal class Manhunt : SuperCallout
{
    private Ped _suspect;
    private Blip _searchAreaBlip;
    private Blip _suspectBlip;
    private string _suspectName;
    private UIMenuItem _speakSuspect;

    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(650f)));
    internal override float OnSceneDistance { get; set; } = 50;
    internal override string CalloutName { get; set; } = "Manhunt";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Wanted suspect on the run.";
        CalloutAdvisory = "Officers report a suspect evaded them in the area.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 SUSPECTS_LAST_SEEN_02 IN_OR_ON_POSITION", SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Manhunt", "Search for the suspect. High priority, respond ~r~CODE-3");

        SpawnSuspect();
        CreateSearchAreaBlip();
        SetupConversation();
    }

    private void SpawnSuspect()
    {
        _suspect = new Ped(SpawnPoint.Position) { IsPersistent = true };
        PyroFunctions.SetWanted(_suspect, true);
        _suspectName = Functions.GetPersonaForPed(_suspect).FullName;
        _suspect.Tasks.Wander();
        EntitiesToClear.Add(_suspect);
    }

    private void CreateSearchAreaBlip()
    {
        _searchAreaBlip = new Blip(_suspect.Position.Around2D(40f, 75f), 90f) { Color = Color.Yellow, Alpha = 0.5f };
        _searchAreaBlip.EnableRoute(Color.Yellow);
        BlipsToClear.Add(_searchAreaBlip);
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

        StartPursuit();
        UpdateBlips();
    }

    private void StartPursuit()
    {
        var pursuit = Functions.CreatePursuit();
        Functions.AddPedToPursuit(pursuit, _suspect);
        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
    }

    private void UpdateBlips()
    {
        _searchAreaBlip?.Delete();
        _suspectBlip = _suspect.AttachBlip();
        _suspectBlip.Color = Color.Red;
        BlipsToClear.Add(_suspectBlip);
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
        {
            GameFiber.StartNew(
                delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: Why did you run? It makes just makes the situation worse.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: Man I just didn't want to go back to the slammer.'", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: I understand that but evading is a whole new charge that will make going back even worse.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle($"~r~{_suspectName}~s~: I know, too late to go back now though.", 5000);
                    GameFiber.Wait(5000);
                }
            );
        }

        base.Conversations(sender, selItem, index);
    }
}
