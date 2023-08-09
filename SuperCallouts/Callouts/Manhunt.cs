#region

using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Manhunt", CalloutProbability.Low, "Suspect evading police", "Code 3")]
internal class Manhunt : SuperCallout
{
    private Ped _bad;
    private Blip _cBlip;
    private Blip _cBlip2;
    private string _name1;
    private UIMenuItem _speakSuspect;
    internal override Vector3 SpawnPoint { get; set; } = World.GetNextPositionOnStreet(Player.Position.Around(650f));
    internal override float OnSceneDistance { get; set; } = 50;
    internal override string CalloutName { get; set; } = "Manhunt";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Wanted suspect on the run.";
        CalloutAdvisory = "Officers report a suspect evaded them in the area.";
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 SUSPECTS_LAST_SEEN_02 IN_OR_ON_POSITION",
            SpawnPoint);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Manhunt",
            "Search for the suspect. High priority, respond ~r~CODE-3");

        _bad = new Ped(SpawnPoint) { IsPersistent = true };
        PyroFunctions.SetWanted(_bad, true);
        _name1 = Functions.GetPersonaForPed(_bad).FullName;
        _bad.Tasks.Wander();
        EntitiesToClear.Add(_bad);

        _cBlip = new Blip(_bad.Position.Around2D(40f, 75f), 90f) { Color = Color.Yellow, Alpha = .5f };
        _cBlip.EnableRoute(Color.Yellow);
        BlipsToClear.Add(_cBlip);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        ConvoMenu.AddItem(_speakSuspect);
    }

    internal override void CalloutOnScene()
    {
        var pursuit = Functions.CreatePursuit();
        Functions.AddPedToPursuit(pursuit, _bad);
        Functions.SetPursuitIsActiveForPlayer(pursuit, true);
        if (_cBlip.Exists()) _cBlip.Delete();
        _cBlip2 = _bad.AttachBlip();
        _cBlip2.Color = Color.Red;
        BlipsToClear.Add(_cBlip2);
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(delegate
            {
                Game.DisplaySubtitle("~g~You~s~: Why did you run? It makes just makes the situation worse.", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~r~" + _name1 + "~s~: Man I just didn't want to go back to the slammer.'", 5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~g~You~s~: I understand that but evading is a whole new charge that will make going back even worse.",
                    5000);
                GameFiber.Wait(5000);
                Game.DisplaySubtitle(
                    "~r~" + _name1 + "~s~: I know, too late to go back now though.", 5000);
                GameFiber.Wait(5000);
            });
        base.Conversations(sender, selItem, index);
    }
}