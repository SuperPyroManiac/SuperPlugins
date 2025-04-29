﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using PyroCommon.Types;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Types.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Trespassing", CalloutProbability.Medium)]
internal class Trespassing : SuperCallout
{
    private readonly int _cScene = new Random(DateTime.Now.Millisecond).Next(0, 4);

    private readonly List<Location> _locations =
    [
        new(new Vector3(1323.59f, -1652.35f, 52.27f), 99f),
        new(new Vector3(77.46f, -1390.56f, 29.3761f), 86f),
        new(new Vector3(423.18f, -808.52f, 29.49f), 268f),
        new(new Vector3(-711.29f, -156.18f, 37.41f), 344f),
        new(new Vector3(1392.59f, 3602.87f, 34.98f), 24f),
        new(new Vector3(1699.71f, 4926.49f, 42.06f), 167f),
        new(new Vector3(1733.94f, 6420.61f, 35.03f), 245f),
        new(new Vector3(2679.04f, 3281.72f, 55.24f), 128f),
        new(new Vector3(-49.33f, -1756.87f, 29.42f), 255f),
        new(new Vector3(-1224.55f, -906.21f, 12.32f), 229f),
    ];

    private Blip _cBlip;
    private string _name;
    private UIMenuItem _speakSuspect;
    private Ped _suspect;
    internal override Location SpawnPoint { get; set; }
    internal override float OnSceneDistance { get; set; } = 10;
    internal override string CalloutName { get; set; } = "Trespassing";

    internal override void CalloutPrep()
    {
        SpawnPoint = _locations.OrderBy(x => x.Position.DistanceTo(Game.LocalPlayer.Character.Position)).FirstOrDefault();
        CalloutMessage = "~r~" + Settings.EmergencyNumber + " Report:~s~ Reports of a person trespassing.";
        CalloutAdvisory = "Caller says the person is being aggressive and damaging property.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~y~Trespassing",
            "Caller reports an individual trespassing and causing a disturbance. ~r~CODE-2"
        );

        _suspect = new Ped(SpawnPoint.Position, SpawnPoint.Heading) { IsPersistent = true, BlockPermanentEvents = true };
        _suspect.SetDrunk(Enums.DrunkState.Sloshed);
        _suspect.Metadata.stpAlcoholDetected = true;
        _name = Functions.GetPersonaForPed(_suspect).FullName;
        _suspect.Tasks.Cower(-1);
        EntitiesToClear.Add(_suspect);

        _cBlip = _suspect.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name);
        ConvoMenu.AddItem(_speakSuspect);
    }

    internal override void CalloutRunning()
    {
        if (!_suspect)
        {
            CalloutEnd(true);
            return;
        }

        if (!OnScene)
            _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
        if (_suspect.IsDead)
        {
            _speakSuspect!.Enabled = false;
            _speakSuspect.RightLabel = "~r~Dead";
        }
    }

    internal override void CalloutOnScene()
    {
        if (!_suspect)
        {
            CalloutEnd(true);
            return;
        }

        _cBlip?.DisableRoute();
        Questioning.Enabled = true;
        _suspect.Face(Game.LocalPlayer.Character);
        Game.DisplaySubtitle("~r~" + _name + "~s~: What do you want?", 3000);
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        try
        {
            if (!_suspect)
            {
                CalloutEnd(true);
                return;
            }

            if (selItem == _speakSuspect)
                GameFiber.StartNew(
                    delegate
                    {
                        _speakSuspect.Enabled = false;
                        Game.DisplaySubtitle("~g~You~s~: What's going on?", 4000);
                        GameFiber.Wait(4000);
                        _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                        Game.DisplaySubtitle("~r~" + _name + "~s~: Nothing, beat it im not doing anything wrong.", 4000);
                        GameFiber.Wait(4000);
                        Game.DisplaySubtitle(
                            "~g~You~s~: Well we have records showing you have been trespassed from this business.",
                            4000
                        );
                        GameFiber.Wait(4000);
                        Game.DisplaySubtitle(
                            "~r~" + _name + "~s~: I don't know anything about that, this is a free country!",
                            4000
                        );
                        GameFiber.Wait(4000);
                        Game.DisplaySubtitle("~g~You~s~: Alright, well let's step outside and have a talk about this.", 4000);
                        switch (_cScene)
                        {
                            case 0:
                                _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                                _suspect.BlockPermanentEvents = false;
                                LHandle pursuit;
                                pursuit = Functions.CreatePursuit();
                                Functions.AddPedToPursuit(pursuit, _suspect);
                                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
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
                                Game.DisplaySubtitle("~r~" + _name + "~s~: Don't make me do this!", 4000);
                                break;
                            case 3:
                                _suspect.PlayAmbientSpeech("GENERIC_CURSE_MED");
                                Game.DisplaySubtitle("~r~" + _name + "~s~: Ok, ok, I am leaving. Now leave me alone.", 4000);
                                _suspect.Dismiss();
                                break;
                        }
                    }
                );
            base.Conversations(sender, selItem, index);
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            End();
        }
    }
}
