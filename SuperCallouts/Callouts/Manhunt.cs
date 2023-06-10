#region

using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Manhunt", CalloutProbability.Low, "Suspect evading police", "Code 3")]
internal class Manhunt : Callout
{
    private readonly UIMenuItem _callAr = new("~r~ Call Air Unit", "Calls for an air unit.");
    private readonly UIMenu _convoMenu = new("SuperCallouts", "~y~Choose a subject to speak with.");
    private readonly UIMenuItem _endCall = new("~y~End Callout", "Ends the callout.");
    private readonly MenuPool _interaction = new();
    private readonly UIMenu _mainMenu = new("SuperCallouts", "~y~Choose an option.");
    private readonly UIMenuItem _questioning = new("Speak With Subject");
    private Ped _bad;
    private Blip _cBlip;
    private Blip _cBlip2;
    private string _name1;
    private bool _onScene;
    private LHandle _pursuit;
    private Vector3 _searcharea;
    private Vector3 _spawnPoint;
    private UIMenuItem _speakSuspect;

    public override bool OnBeforeCalloutDisplayed()
    {
        _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(650f));
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
        CalloutMessage = "~b~Dispatch:~s~ Wanted suspect on the run.";
        CalloutAdvisory = "Officers report a suspect evaded them in the area.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition("ATTENTION_ALL_UNITS_05 SUSPECTS_LAST_SEEN_02 IN_OR_ON_POSITION",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        //Setup
        Game.Console.Print("SuperCallouts Log: Manhunt callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Manhunt",
            "Search for the suspect. High priority, respond ~r~CODE-3");
        //Bad
        _bad = new Ped(_spawnPoint) { IsPersistent = true };
        CFunctions.SetWanted(_bad, true);
        _name1 = Functions.GetPersonaForPed(_bad).FullName;
        _bad.Tasks.Wander();
        //AreaBlip
        var position = _bad.Position;
        _searcharea = position.Around2D(40f, 75f);
        _cBlip = new Blip(_searcharea, 90f) { Color = Color.Yellow, Alpha = .5f };
        _cBlip.EnableRoute(Color.Yellow);
        //UI
        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        _mainMenu.MouseControlsEnabled = false;
        _mainMenu.AllowCameraMovement = true;
        _interaction.Add(_mainMenu);
        _interaction.Add(_convoMenu);
        _mainMenu.AddItem(_callAr);
        _mainMenu.AddItem(_questioning);
        _mainMenu.AddItem(_endCall);
        _convoMenu.AddItem(_speakSuspect);
        _mainMenu.RefreshIndex();
        _convoMenu.RefreshIndex();
        _mainMenu.BindMenuToItem(_convoMenu, _questioning);
        _mainMenu.OnItemSelect += Interactions;
        _convoMenu.OnItemSelect += Conversations;
        _callAr.LeftBadge = UIMenuItem.BadgeStyle.Alert;
        _convoMenu.ParentMenu = _mainMenu;
        _callAr.Enabled = false;
        _questioning.Enabled = false;
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        try
        {
            //GamePlay
            if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 55)
            {
                _onScene = true;
                _pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(_pursuit, _bad);
                Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                if (_cBlip.Exists()) _cBlip.Delete();
                _cBlip2 = _bad.AttachBlip();
                _cBlip2.Color = Color.Red;
                _callAr.Enabled = true;
                Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
            }

            if (_onScene && !Functions.IsPursuitStillRunning(_pursuit))
            {
                Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                _questioning.Enabled = true;
            }

            //Keybinds
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
            _interaction.ProcessMenus();
        }
        catch (Exception e)
        {
            Game.Console.Print("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print("SuperCallouts Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print(e.ToString());
            Game.Console.Print("======================================================");
            Game.Console.Print("SuperCallouts Error Report End");
            End();
        }

        base.Process();
    }

    public override void End()
    {
        _interaction.CloseAllMenus();
        if (_bad.Exists()) _bad.Dismiss();
        if (_cBlip.Exists()) _cBlip.Delete();
        if (_cBlip2.Exists()) _cBlip2.Delete();
        _mainMenu.Visible = false;
        CFunctions.Code4Message();
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        CalloutInterfaceAPI.Functions.SendMessage(this, "Scene clear, Code4");
        base.End();
    }

    private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _callAr)
        {
            Game.DisplaySubtitle("~g~You~s~: Dispatch, can I get air support.");
            try
            {
                Functions.RequestBackup(_bad.Position, EBackupResponseType.Pursuit, EBackupUnitType.AirUnit);
            }
            catch (Exception)
            {
                Game.Console.Print(
                    "SuperEvents Warning: Ultimate Backup is not installed! Backup was not automatically called!");
                Game.DisplayHelp("~r~Ultimate Backup is not installed! Backup was not automatically called!", 8000);
            }

            _callAr.Enabled = false;
        }
        else if (selItem == _endCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            End();
        }
    }

    private void Conversations(UIMenu sender, UIMenuItem selItem, int index)
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
    }
}