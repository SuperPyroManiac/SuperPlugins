using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using SuperCallouts.SimpleFunctions;

namespace SuperCallouts.Callouts;

[CalloutInfo("KnifeAttack", CalloutProbability.Medium)]
internal class KnifeAttack : Callout
{
    private Blip _cBlip;
    private float _cHeading;
    private Tuple<Vector3, float> _chosenLocation;
    private UIMenu _convoMenu;
    private readonly int _cScene = new Random().Next(1, 4);
    private Vector3 _cSpawnPoint;
    private Ped _cSuspect;
    private Tasks _cTasks = Tasks.CheckDistance;
    private Ped _cVictim;

    private UIMenuItem _endCall;

    //UI Items
    private MenuPool _interaction;

    //Locations
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

    private UIMenu _mainMenu;
    private UIMenuItem _questioning;

    public override bool OnBeforeCalloutDisplayed()
    {
        foreach (var tupe in _locations)
            _chosenLocation = _locations.OrderBy(x => x.Item1.DistanceTo(Game.LocalPlayer.Character.Position))
                .FirstOrDefault();
        _cSpawnPoint = _chosenLocation.Item1;
        _cHeading = _chosenLocation.Item2;
        ShowCalloutAreaBlipBeforeAccepting(_cSpawnPoint, 10f);
        CalloutMessage = "~b~Dispatch:~s~ Reports of a knife attack.";
        CalloutAdvisory = "Caller says attacker has injured others.";
        CalloutPosition = _cSpawnPoint;
        Functions.PlayScannerAudioUsingPosition(
            "CITIZENS_REPORT_04 CRIME_ROBBERY_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            _cSpawnPoint);
        return base.OnBeforeCalloutDisplayed();
    }

    public override bool OnCalloutAccepted()
    {
        //Setup
        Game.LogTrivial("SuperCallouts Log: knife attack callout accepted. Using scenario #:" + _cScene);
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Knife Attack",
            "Reports of a person attacking people with a knife at the train station.");
        //Suspect
        _cSuspect = new Ped(_cSpawnPoint);
        _cSuspect.Heading = _cHeading;
        _cSuspect.IsPersistent = true;
        _cSuspect.BlockPermanentEvents = true;
        _cSuspect.Inventory.Weapons.Add(WeaponHash.Knife);
        _cSuspect.Inventory.EquippedWeapon = WeaponHash.Knife;
        //Victim
        _cVictim = new Ped(_cSuspect.FrontPosition);
        _cVictim.IsPersistent = true;
        _cVictim.BlockPermanentEvents = true;
        //Blip
        _cBlip = new Blip(_cSpawnPoint, 8f);
        _cBlip.Color = Color.Red;
        _cBlip.Alpha /= 2;
        _cBlip.Name = "Callout";
        _cBlip.Flash(500, 8000);
        _cBlip.EnableRoute(Color.Red);
        //UI Items
        CFunctions.BuildUi(out _interaction, out _mainMenu, out _convoMenu, out _questioning, out _endCall);
        _mainMenu.OnItemSelect += InteractionProcess;
        return base.OnCalloutAccepted();
    }

    public override void Process()
    {
        switch (_cTasks)
        {
            case Tasks.CheckDistance:
                if (Game.LocalPlayer.Character.DistanceTo(_cSuspect) < 25f)
                {
                    _cVictim.Kill();
                    _cTasks = Tasks.OnScene;
                }

                break;
            case Tasks.OnScene:
                Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                switch (_cScene)
                {
                    case 1:
                        _cSuspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        break;
                    case 2:
                        var flee = Functions.CreatePursuit();
                        Functions.AddPedToPursuit(flee, _cSuspect);
                        Functions.SetPursuitIsActiveForPlayer(flee, true);
                        break;
                    case 3:
                        _cSuspect.Tasks.Wander();
                        break;
                }

                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Locate Suspect",
                    "Search for the suspect. Last was seen carrying a knife.");
                _cBlip.Delete();
                _cTasks = Tasks.End;
                break;
            case Tasks.End:
                break;
        }

        if (Game.IsKeyDown(Settings.EndCall)) End();
        if (Game.IsKeyDown(Settings.Interact)) _mainMenu.Visible = !_mainMenu.Visible;
        _interaction.ProcessMenus();
        base.Process();
    }

    public override void End()
    {
        if (_cBlip) _cBlip.Delete();
        if (_cVictim) _cVictim.Dismiss();
        if (_cSuspect) _cSuspect.Dismiss();
        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        CFunctions.Code4Message();
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

    private enum Tasks
    {
        CheckDistance,
        OnScene,
        End
    }
}