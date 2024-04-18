#region

using System;
using System.Drawing;
using CalloutInterfaceAPI;
using LSPD_First_Response;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.API;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using Functions = LSPD_First_Response.Mod.API.Functions;

#endregion

namespace SuperCallouts.Callouts;

[CalloutInterface("[SC] Angry Animal", CalloutProbability.Medium, "Report of animal attacking people", "Code 3")]
internal class AngryAnimal : SuperCallout
{
    private readonly UIMenuItem _callEms = new("~r~ Call EMS", "Calls for a medical team.");
    private Ped _animal;
    private Blip _cBlip;
    private Blip _cBlip2;
    private Ped _victim;
    internal override Location SpawnPoint { get; set; } = new(World.GetNextPositionOnStreet(Player.Position.Around(450f)), 0);
    internal override float OnSceneDistance { get; set; } = 30;
    internal override string CalloutName { get; set; } = "Animal Attack";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~" + Settings.EmergencyNumber +
                         " Report:~s~ Person(s) being attacked by a wild animal.";
        CalloutAdvisory = "Caller says a wild animal is attacking people.";
        Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_11_351_02 UNITS_RESPOND_CODE_03_01",
            SpawnPoint.Position);
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~Help Civilian",
            "Details are unknown, get to the scene as soon as possible! Respond ~r~CODE-3");

        Model[] meanAnimal = { "A_C_MTLION", "A_C_COYOTE" };
        _animal = new Ped(meanAnimal[new Random().Next(meanAnimal.Length)], SpawnPoint.Position, 50);
        _animal.IsPersistent = true;
        _animal.BlockPermanentEvents = true;
        EntitiesToClear.Add(_animal);

        _victim = new Ped(_animal.GetOffsetPosition(new Vector3(0, 1.8f, 0)));
        _victim.IsPersistent = true;
        _victim.BlockPermanentEvents = true;
        _victim.Health = 500;
        EntitiesToClear.Add(_victim);

        MainMenu.RemoveItemAt(1);
        MainMenu.AddItem(_callEms);
        MainMenu.AddItem(EndCall);
        _callEms.LeftBadge = UIMenuItem.BadgeStyle.Alert;
        _callEms.Enabled = false;

        _cBlip = _animal.AttachBlip();
        _cBlip.EnableRoute(Color.Red);
        _cBlip.Color = Color.Red;
        _cBlip2 = _victim.AttachBlip();
        _cBlip2.Color = Color.Blue;
        BlipsToClear.Add(_cBlip);
        BlipsToClear.Add(_cBlip2);
    }

    internal override void CalloutOnScene()
    {
        _cBlip.DisableRoute();
        _animal.Tasks.FightAgainst(_victim, -1);
        _victim.Tasks.ReactAndFlee(_animal);
        _callEms.Enabled = true;
    }

    protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index)
    {
        if (selItem == _callEms)
        {
            Game.DisplaySubtitle(
                "~g~You~s~: Dispatch, we have a person that has been attacked by an animal! We need medical here ASAP!");
            CalloutInterfaceAPI.Functions.SendMessage(this, "Rescue has been notified and is on route. 11-78");
            if (PyroCommon.Main.UsingUb)
            {
                Wrapper.CallEms();
                Wrapper.CallFd();
            }
            else
            {
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                    EBackupUnitType.Ambulance);
                Functions.RequestBackup(Game.LocalPlayer.Character.Position, EBackupResponseType.Code3,
                    EBackupUnitType.Firetruck);
            }

            _callEms.Enabled = false;
            base.Interactions(sender, selItem, index);
        }
    }
}