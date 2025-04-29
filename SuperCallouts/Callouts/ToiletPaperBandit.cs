using System.Drawing;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.PyroFunctions;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Types.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Stolen Cleaning Truck", CalloutProbability.Low)]
internal class ToiletPaperBandit : SuperCallout
{
    private Ped _bad;
    private Blip _cBlip;
    private Vehicle _cVehicle;
    private string _name1;
    private readonly LHandle _pursuit = Functions.CreatePursuit();
    private UIMenuItem _speakSuspect;
    internal override Location SpawnPoint { get; set; } = PyroFunctions.GetSideOfRoad(750, 180);
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Stolen Cleaning Truck";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~b~Dispatch:~s~ Reports of a sanitization transport robbery.";
        CalloutAdvisory = "Caller reports the vehicle of full of cleaning supplies. Possible fire hazard.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_UNITS_05 WE_HAVE CRIME_GRAND_THEFT_AUTO_03 IN_OR_ON_POSITION",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Robbery",
            "Reports of someone robbing a truck full of cleaning supplies, respond ~r~CODE-3"
        );

        _cVehicle = new Vehicle("pounder", SpawnPoint.Position)
        {
            IsPersistent = true,
            IsStolen = true,
            Heading = SpawnPoint.Heading,
        };
        _cVehicle.Metadata.searchDriver =
            "~y~50 travel hand sanitizers~s~, ~y~48 toilet paper rolls~s~, ~g~lighters~s~, ~g~cigarettes~s~";
        _cVehicle.Metadata.searchPassenger = "~r~multiple packs of cleaning wipes~s~, ~r~box full of medical masks~s~";
        _cVehicle.Metadata.searchTrunk =
            "~r~multiple pallets of toilet paper~s~, ~r~hazmat suits~s~, ~r~12 molotov explosives~s~, ~y~22 packs of cigarettes~s~";
        EntitiesToClear.Add(_cVehicle);

        _bad = new Ped("s_m_m_movspace_01", SpawnPoint.Position.Around2D(20f), 0f)
        {
            BlockPermanentEvents = true,
            IsPersistent = true,
        };
        _bad.WarpIntoVehicle(_cVehicle, -1);
        _bad.Inventory.Weapons.Add(WeaponHash.Molotov);
        _bad.Metadata.searchPed = "~r~Molotov's~s~, ~g~multiple hand sanitizers~s~, ~g~cleaning wipes~s~";
        _bad.Metadata.stpDrugsDetected = true;
        _bad.Tasks.CruiseWithVehicle(_cVehicle, 10f, VehicleDrivingFlags.Normal);
        _name1 = Functions.GetPersonaForPed(_bad).FullName;
        EntitiesToClear.Add(_bad);

        _cBlip = _bad.AttachBlip();
        _cBlip.Color = Color.Red;
        _cBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip);

        _speakSuspect = new UIMenuItem("Speak with ~y~" + _name1);
        ConvoMenu.AddItem(_speakSuspect);
    }

    internal override void CalloutRunning()
    {
        if (OnScene)
        {
            if (!_bad)
            {
                CalloutEnd(true);
                return;
            }

            if (!Functions.IsPursuitStillRunning(_pursuit) || _bad.IsCuffed)
            {
                if (!OnScene)
                    return;
                Game.DisplaySubtitle("~r~" + _name1 + "~s~: I surrender!", 5000);
                Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
                Questioning.Enabled = true;
            }
        }
    }

    internal override void CalloutOnScene()
    {
        _cBlip?.DisableRoute();
        Game.DisplayHelp($"Press ~{Settings.Interact.GetInstructionalId()}~ to open interaction menu.");
        Game.DisplayHelp("Suspect is fleeing!");
        Functions.AddPedToPursuit(_pursuit, _bad);
        Functions.SetPursuitIsActiveForPlayer(_pursuit, true);
        Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.AirUnit);
        Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.SwatTeam);
        Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Pursuit, EBackupUnitType.LocalUnit);
    }

    protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _speakSuspect)
            GameFiber.StartNew(
                delegate
                {
                    Game.DisplaySubtitle("~g~You~s~: What are you doing with this truck?", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~r~" + _name1 + "~s~: Get away from me! You might have that virus!!!", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~g~You~s~: You need to calm down, is that why you stole a truck full of cleaning supplies?",
                        5000
                    );
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: Everyone is infected.. EVERYONE! Let me go, give me my sanitizer!!",
                        5000
                    );
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle("~g~You~s~: I understand your fears but you need to calm down.", 5000);
                    GameFiber.Wait(5000);
                    Game.DisplaySubtitle(
                        "~r~" + _name1 + "~s~: Its everywhere.. EVERYWHERE! I need my sanitizer, I NEED IT! I NEED IT!",
                        5000
                    );
                }
            );
        base.Conversations(sender, selItem, index);
    }
}
