using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts;

[CalloutInfo("Trespassing", CalloutProbability.Medium)]
public class Trespassing : Callout
{
    private readonly int _cScene = new Random().Next(0, 4);
    private readonly List<Tuple<Vector3, float>> _locations = new()
    {
        Tuple.Create(new Vector3(98.695f, -1711.661f, 30.11257f), 226f),
        Tuple.Create(new Vector3(128.4992f, -1737.29f, 30.11015f), 240f),
        Tuple.Create(new Vector3(-219.8601f, -1049.929f, 30.13966f), 168f),
        Tuple.Create(new Vector3(-498.5762f, -671.4704f, 11.80903f), 173f),
        Tuple.Create(new Vector3(-1337.652f, -494.7437f, 15.04538f), 21f),
        Tuple.Create(new Vector3(-818.4063f, -128.05f, 28.17534f), 49f),
        Tuple.Create(new Vector3(-290.8545f, -338.4935f, 10.06309f), 0f),
        Tuple.Create(new Vector3(297.8111f, -1202.387f, 38.89421f), 172f),
        Tuple.Create(new Vector3(-549.0919f, -1298.383f, 26.90161f), 187f),
        Tuple.Create(new Vector3(-882.8482f, -2308.612f, -11.7328f), 234f),
        Tuple.Create(new Vector3(-1066.983f, -2700.32f, -7.41007f), 339f)
    };
    private Blip _cBlip;
    private UIMenu _convoMenu;
    private UIMenuItem _endCall;
    private float _heading;
    private Tuple<Vector3, float> _chosenLocation;
    private MenuPool _interaction;
    private UIMenu _mainMenu;
    private string _name;
    private bool _onScene;
    private bool _checkDead;
    private UIMenuItem _questioning;
    private Vector3 _spawnPoint;
    private UIMenuItem _speakSuspect;
    private Ped _suspect;

    public override bool OnBeforeCalloutDisplayed()
    {
        foreach (var unused in _locations)
            _chosenLocation = _locations.OrderBy(x =>
                x.Item1.DistanceTo(Game.LocalPlayer.Character.Position)).FirstOrDefault();
        _spawnPoint = _chosenLocation!.Item1;
        _heading = _chosenLocation.Item2;
        ShowCalloutAreaBlipBeforeAccepting(_spawnPoint, 10f);
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Reports of a person trespassing.";
        CalloutAdvisory = "Caller says the person is being aggressive and damaging property.";
        CalloutPosition = _spawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION",
            _spawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        //Setup
        Game.LogTrivial("SuperCallouts Log: trespassing callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~y~Trespassing",
            "Caller reports an individual trespassing and causing a disturbance. ~r~CODE-2");
        if (Main.UsingCi) Wrapper.StartCi(this, "Code 2");
        //Ped
        _suspect = new Ped(_spawnPoint, _heading)
        {
            IsPersistent = true,
            BlockPermanentEvents = true
        };
        CFunctions.SetDrunk(_suspect, true);
        _suspect.Metadata.stpAlcoholDetected = true;
        Functions.SetPersonaForPed(_suspect, new Persona("Benzo", "Smith", Gender.Male));
        _name = Functions.GetPersonaForPed(_suspect).FullName;
        _suspect.Tasks.Cower(-1);
        //cBlip
        _cBlip = _suspect.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        //UI
        CFunctions.BuildUi(out _interaction, out _mainMenu, out _convoMenu, out _questioning, out _endCall);
        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name);
        _convoMenu.AddItem(_speakSuspect);
        _mainMenu.OnItemSelect += InteractionProcess;
        _convoMenu.OnItemSelect += Conversations;
        _mainMenu.RefreshIndex();
        _convoMenu.RefreshIndex();
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        try
        {
            if (!_onScene) _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
            if (!_onScene && Game.LocalPlayer.Character.Position.DistanceTo(_spawnPoint) < 10)
            {
                _onScene = true;
                _cBlip.DisableRoute();
                _questioning.Enabled = true;
                if (Main.UsingCi) Wrapper.CiSendMessage(this, "Dispatch: We ran the name given to us by the caller and can confirm this individual has been trespassed in the past from this location.");
                NativeFunction.Natives.x5AD23D40115353AC(_suspect, Game.LocalPlayer.Character, -1);
                Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.", 5000);
                Game.DisplaySubtitle("~r~" + _name + "~s~: What do you want?", 3000);
            }

            //Keybinds
            if (Game.IsKeyDown(Settings.EndCall)) End();
            if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
            //UI
            if (_suspect.IsDead && !_checkDead)
            {
                _speakSuspect.Enabled = false;
                _speakSuspect.RightLabel = "~r~Dead";
                _checkDead = true;
            }
            _interaction.ProcessMenus();
        }
        catch (Exception e)
        {
            Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.LogTrivial("SuperCallouts Error Report Start");
            Game.LogTrivial("======================================================");
            Game.LogTrivial(e.ToString());
            Game.LogTrivial("======================================================");
            Game.LogTrivial("SuperCallouts Error Report End");
            End();
        }

        base.Process();
    }

    public override void End()
    {
        if (_suspect.Exists()) _suspect.Dismiss();
        if (_cBlip.Exists()) _cBlip.Delete();
        _mainMenu.Visible = false;
        CFunctions.Code4Message();
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        if (Main.UsingCi) Wrapper.CiSendMessage(this, "Scene clear, Code4");
        base.End();
    }

    private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _endCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            End();
        }
    }

    private void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        try
        {
            if (selItem == _speakSuspect)
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: What's going on?", 4000);
                    GameFiber.Wait(4000);
                    _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                    Game.DisplaySubtitle(
                        "~r~" + _name + "~s~: Nothing, beat it im not doing anything wrong.",
                        4000);
                    GameFiber.Wait(4000);
                    Game.DisplaySubtitle("~g~You~s~: Well we have records showing you have been trespassed from this area.", 4000);
                    GameFiber.Wait(4000);
                    Game.DisplaySubtitle(
                        "~r~" + _name + "~s~: I don't know anything about that, this is a free country!", 4000);
                    GameFiber.Wait(4000);
                    Game.DisplaySubtitle(
                        "~g~You~s~: Alright, well let's step outside and have a talk about this.",
                        4000);
                    switch (_cScene)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                    }
                    if (Main.UsingCi)
                        Wrapper.CiSendMessage(this, "Witness has been questioned, no useful information.");
                });
        }
        catch (Exception e)
        {
            Game.LogTrivial("Oops there was an error here. Please send this log to https://dsc.gg/ulss");
            Game.LogTrivial("SuperCallouts Error Report Start");
            Game.LogTrivial("======================================================");
            Game.LogTrivial(e.ToString());
            Game.LogTrivial("======================================================");
            Game.LogTrivial("SuperCallouts Error Report End");
            End();
        }
    }
}