using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Types.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Knife Attack", CalloutProbability.Medium)]
internal class KnifeAttack : SuperCallout
{
    private readonly int _cScene = new Random(DateTime.Now.Millisecond).Next(1, 4);

    private readonly List<Location> _locations =
    [
        new(new Vector3(98.695f, -1711.661f, 30.11257f), 226f),
        new(new Vector3(128.4992f, -1737.29f, 30.11015f), 240f),
        new(new Vector3(-219.8601f, -1049.929f, 30.13966f), 168f),
        new(new Vector3(-498.5762f, -671.4704f, 11.80903f), 173f),
        new(new Vector3(-1337.652f, -494.7437f, 15.04538f), 21f),
        new(new Vector3(-818.4063f, -128.05f, 28.17534f), 49f),
        new(new Vector3(-290.8545f, -338.4935f, 10.06309f), 0f),
        new(new Vector3(297.8111f, -1202.387f, 38.89421f), 172f),
        new(new Vector3(-549.0919f, -1298.383f, 26.90161f), 187f),
        new(new Vector3(-882.8482f, -2308.612f, -11.7328f), 234f),
        new(new Vector3(-1066.983f, -2700.32f, -7.41007f), 339f),
    ];

    private Blip _cBlip;
    private Ped _cSuspect;
    private Ped _cVictim;
    internal override Location SpawnPoint { get; set; }
    internal override float OnSceneDistance { get; set; } = 25f;
    internal override string CalloutName { get; set; } = "Knife Attack";

    internal override void CalloutPrep()
    {
        SpawnPoint = _locations.OrderBy(x => x.Position.DistanceTo(Game.LocalPlayer.Character.Position)).FirstOrDefault();
        CalloutMessage = "~b~Dispatch:~s~ Reports of a knife attack.";
        CalloutAdvisory = "Caller says attacker has injured others.";
        Functions.PlayScannerAudioUsingPosition(
            "CITIZENS_REPORT_04 CRIME_ROBBERY_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Knife Attack",
            "Reports of a person attacking people with a knife at the train station."
        );

        _cSuspect = new Ped(SpawnPoint.Position);
        _cSuspect.Heading = SpawnPoint.Heading;
        _cSuspect.IsPersistent = true;
        _cSuspect.BlockPermanentEvents = true;
        _cSuspect.Inventory.Weapons.Add(WeaponHash.Knife);
        _cSuspect.Inventory.EquippedWeapon = WeaponHash.Knife;
        EntitiesToClear.Add(_cSuspect);

        _cVictim = new Ped(_cSuspect.FrontPosition);
        _cVictim.IsPersistent = true;
        _cVictim.BlockPermanentEvents = true;
        EntitiesToClear.Add(_cVictim);

        _cBlip = new Blip(SpawnPoint.Position, 8f);
        _cBlip.Color = Color.Red;
        _cBlip.Alpha /= 2;
        _cBlip.Name = "Callout";
        _cBlip.Flash(500, 8000);
        _cBlip.EnableRoute(Color.Red);
        BlipsToClear.Add(_cBlip);
    }

    internal override void CalloutOnScene()
    {
        if (!_cVictim || !_cSuspect)
        {
            CalloutEnd(true);
            return;
        }

        _cVictim.Kill();
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

        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Locate Suspect",
            "Search for the suspect. Last was seen carrying a knife."
        );
        _cBlip?.Delete();
    }
}
