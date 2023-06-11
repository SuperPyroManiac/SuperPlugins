using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CalloutInterfaceAPI;
using LSPD_First_Response;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;

namespace SuperCallouts.Callouts;

[CalloutInterface("Trespassing", CalloutProbability.Medium, "Aggressive person damaging property", "Code 3")]
public class Trespassing : Callout
{
    private readonly int _cScene = new Random().Next(0, 4);

    private readonly List<Tuple<Vector3, float>> _locations = new()
    {
        Tuple.Create(new Vector3(1323.59f, -1652.35f, 52.27f), 99f),
        Tuple.Create(new Vector3(77.46f, -1390.56f, 29.3761f), 86f),
        Tuple.Create(new Vector3(423.18f, -808.52f, 29.49f), 268f),
        Tuple.Create(new Vector3(-711.29f, -156.18f, 37.41f), 344f),
        Tuple.Create(new Vector3(1392.59f, 3602.87f, 34.98f), 24f),
        Tuple.Create(new Vector3(1699.71f, 4926.49f, 42.06f), 167f),
        Tuple.Create(new Vector3(1733.94f, 6420.61f, 35.03f), 245f),
        Tuple.Create(new Vector3(2679.04f, 3281.72f, 55.24f), 128f),
        Tuple.Create(new Vector3(-49.33f, -1756.87f, 29.42f), 255f),
        Tuple.Create(new Vector3(-1224.55f, -906.21f, 12.32f), 229f)
    };

    private Blip _cBlip;
    private bool _checkDead;
    private Tuple<Vector3, float> _chosenLocation;
    private UIMenu _convoMenu;
    private UIMenuItem _endCall;
    private float _heading;
    private MenuPool _interaction;
    private UIMenu _mainMenu;
    private string _name;
    private bool _onScene;
    private LHandle _pursuit;
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
        Log.Info("trespassing callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~y~Trespassing",
            "Caller reports an individual trespassing and causing a disturbance. ~r~CODE-2");
        CalloutInterfaceAPI.Functions.SendMessage(this, "Dispatch: Caller reports suspect is drunk and may be armed.");
        //Ped
        _suspect = new Ped(_spawnPoint, _heading)
        {
            IsPersistent = true,
            BlockPermanentEvents = true
        };
        PyroFunctions.SetDrunk(_suspect, true);
        _suspect.Metadata.stpAlcoholDetected = true;
        Functions.SetPersonaForPed(_suspect, new Persona("Benzo", "Smith", Gender.Male));
        _name = Functions.GetPersonaForPed(_suspect).FullName;
        _suspect.Tasks.Cower(-1);
        //cBlip
        _cBlip = _suspect.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        //UI
        PyroFunctions.BuildUi(out _interaction, out _mainMenu, out _convoMenu, out _questioning, out _endCall);
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
                CalloutInterfaceAPI.Functions.SendMessage(this,
                    "Dispatch: We ran the name given to us by the caller and can confirm this individual has been trespassed in the past from this location.");
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
            Log.Error(e.ToString());
            End();
        }

        base.Process();
    }

    public override void End()
    {
        if (_suspect.Exists()) _suspect.Dismiss();
        if (_cBlip.Exists()) _cBlip.Delete();
        _mainMenu.Visible = false;

        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        CalloutInterfaceAPI.Functions.SendMessage(this, "Scene clear, Code4");
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
                    Game.DisplaySubtitle(
                        "~g~You~s~: Well we have records showing you have been trespassed from this business.", 4000);
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
                            _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                            _suspect.BlockPermanentEvents = false;
                            _pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(_pursuit, _suspect);
                            Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
                            break;
                        case 1:
                            _suspect.Tasks.FightAgainst(Game.LocalPlayer.Character, -1);
                            break;
                        case 2:
                            _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                            _suspect.Inventory.Weapons.Clear();
                            _suspect.Inventory.Weapons.Add(WeaponHash.Pistol).Ammo = -1;
                            _suspect.Tasks.ClearImmediately();
                            _suspect.Tasks.AimWeaponAt(Game.LocalPlayer.Character, -1);
                            Game.DisplaySubtitle(
                                "~r~" + _name + "~s~: Don't make me do this!", 4000);
                            break;
                        case 3:
                            _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                            Game.DisplaySubtitle("~r~" + _name + "~s~: Ok, ok, I am leaving. Now leave me alone.",
                                4000);
                            _suspect.Dismiss();
                            break;
                    }
                });
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            End();
        }
    }
}