#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CalloutInterfaceAPI;
using LSPD_First_Response;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.CustomScenes;
using SuperCallouts.SimpleFunctions;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("Mafia Raid", CalloutProbability.Low, "Stakeout has found a meeting point for mob bosses - Raid in progress", "Code 5", "SWAT")]
internal class Mafia3 : Callout
{
    private readonly Vector3 _callPos = new(949.3857f, -3129.14f, 5.900989f);
    private readonly List<Ped> _peds = new();
    private readonly List<Vehicle> _vehicles = new();
    private Ped _bad1;
    private Ped _bad10;
    private Ped _bad11;
    private Ped _bad12;
    private Ped _bad2;
    private Ped _bad3;
    private Ped _bad4;
    private Ped _bad5;
    private Ped _bad6;
    private Ped _bad7;
    private Ped _bad8;
    private Ped _bad9;
    private Blip _cBlip;
    private Vehicle _defender;
    private UIMenuItem _endCall;
    private MenuPool _interaction;
    private Vehicle _limo;
    private UIMenu _mainMenu;
    private RunState _state = RunState.CheckDistance;
    private Vehicle _truck1;
    private Vehicle _truck2;
    private Vehicle _truck3;
    private static Ped Player => Game.LocalPlayer.Character;


    public override bool OnBeforeCalloutDisplayed()
    {
        ShowCalloutAreaBlipBeforeAccepting(_callPos, 80f);
        CalloutMessage = "~b~FIB Report:~s~ Organized crime members spotted.";
        CalloutPosition = _callPos;
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_BRANDISHING_WEAPON_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            _callPos);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        //Setup
        Mafia3Setup.ConstructMafia3Scene(out _limo, out _defender, out _truck1, out _truck2, out _truck3, out _bad1,
            out _bad2, out _bad3, out _bad4, out _bad5, out _bad6, out _bad7, out _bad8, out _bad9, out _bad10,
            out _bad11, out _bad12);
        Game.LogTrivial("SuperCallouts Log: Mafia3 callout accepted...");
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~The Mafia",
            "FIB and IAA began a raid on a drug scene at the harbor. Suspects are heavily armed and backup is required. Get to the scene.");
        Game.LocalPlayer.Character.RelationshipGroup = "COP";
        Game.DisplaySubtitle("Get to the ~r~scene~w~! Proceed with ~r~CAUTION~w~!", 10000);
        //World
        _cBlip = _truck2.AttachBlip();
        _cBlip.EnableRoute(Color.Red);
        _cBlip.Color = Color.Red;
        //Entities
        _vehicles.Add(_limo);
        _vehicles.Add(_defender);
        _vehicles.Add(_truck1);
        _vehicles.Add(_truck2);
        _vehicles.Add(_truck3);
        _peds.Add(_bad1);
        _peds.Add(_bad2);
        _peds.Add(_bad3);
        _peds.Add(_bad4);
        _peds.Add(_bad5);
        _peds.Add(_bad6);
        _peds.Add(_bad7);
        _peds.Add(_bad8);
        _peds.Add(_bad9);
        _peds.Add(_bad10);
        _peds.Add(_bad11);
        _peds.Add(_bad12);
        foreach (var mafiaCars in _vehicles) mafiaCars.IsPersistent = true;
        foreach (var mafiaDudes in _peds)
        {
            mafiaDudes.IsPersistent = true;
            mafiaDudes.BlockPermanentEvents = true;
            mafiaDudes.Inventory.Weapons.Add(WeaponHash.AssaultRifle).Ammo = -1;
            mafiaDudes.RelationshipGroup = new RelationshipGroup("MAFIA");
            CFunctions.SetWanted(mafiaDudes, true);
            Functions.AddPedContraband(mafiaDudes, ContrabandType.Narcotics, "Cocaine");
        }

        //Add Items
        _truck1.Metadata.searchTrunk =
            "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~y~empty body bags~s~";
        _truck2.Metadata.searchTrunk =
            "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~y~bags of cash~s~";
        _truck3.Metadata.searchTrunk =
            "~r~multiple pallets of cocaine~s~, ~r~hazmat suits~s~, ~r~multiple weapons~s~, ~r~explosives~s~";
        //UI Items
        CFunctions.BuildUi(out _interaction, out _mainMenu, out _, out _, out _endCall);
        _mainMenu.OnItemSelect += InteractionProcess;
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        try
        {
            switch (_state)
            {
                case RunState.CheckDistance:
                    if (Player.DistanceTo(_callPos) < 90f)
                    {
                        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Hate);
                        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "PLAYER", Relationship.Hate);
                        Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Hate);
                        CalloutInterfaceAPI.Functions.SendMessage(this, "NOOSE Units on-route to scene.");
                        if (Main.UsingUb)
                        {
                            Wrapper.CallSwat(true);
                            Wrapper.CallSwat(true);
                            Wrapper.CallCode3();
                        }
                        else
                        {
                            Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                EBackupUnitType.SwatTeam);
                            Functions.RequestBackup(_callPos, EBackupResponseType.Code3,
                                EBackupUnitType.SwatTeam);
                            Functions.RequestBackup(_callPos, EBackupResponseType.Code3, EBackupUnitType.LocalUnit);
                        }

                        _state = RunState.RaidScene;
                    }

                    break;
                case RunState.RaidScene:
                    GameFiber.StartNew(delegate
                    {
                        GameFiber.Wait(5000);
                        foreach (var mafiaDudes in _peds.Where(mafiaDudes => mafiaDudes.Exists()))
                        {
                            mafiaDudes.BlockPermanentEvents = false;
                            mafiaDudes.Tasks.FightAgainstClosestHatedTarget(100, -1);
                        }

                        _bad1.Tasks.FightAgainst(Player);

                        _cBlip.DisableRoute();
                    });
                    _state = RunState.End;
                    break;
                case RunState.End:
                    break;
            }
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

        //Keybinds
        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
        _interaction.ProcessMenus();
        base.Process();
    }

    public override void End()
    {
        if (_cBlip.Exists()) _cBlip.Delete();
        foreach (var mafiaCars in _vehicles.Where(mafiaCars => mafiaCars.Exists())) mafiaCars.Dismiss();
        foreach (var mafiaDudes in _peds.Where(mafiaDudes => mafiaDudes.Exists())) mafiaDudes.Dismiss();
        Game.SetRelationshipBetweenRelationshipGroups("COP", "MAFIA", Relationship.Dislike);
        Game.SetRelationshipBetweenRelationshipGroups("MAFIA", "COP", Relationship.Dislike);

        _interaction.CloseAllMenus();
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        CFunctions.Code4Message();
        CalloutInterfaceAPI.Functions.SendMessage(this, "Scene clear, Code4");

        base.End();
    }

    //UI Functions
    private void InteractionProcess(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _endCall)
        {
            Game.DisplaySubtitle("~y~Callout Ended.");
            End();
        }
    }

    private enum RunState
    {
        CheckDistance,
        RaidScene,
        End
    }
}