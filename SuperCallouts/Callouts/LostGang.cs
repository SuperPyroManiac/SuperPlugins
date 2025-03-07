using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.PyroFunctions;
using PyroCommon.PyroFunctions.Extensions;
using Rage;
using SuperCallouts.CustomScenes;
using Functions = LSPD_First_Response.Mod.API.Functions;
using Location = PyroCommon.Objects.Location;

namespace SuperCallouts.Callouts;

[CalloutInfo("[SC] Officer Ambush", CalloutProbability.Low)]
internal class LostGang : SuperCallout
{
    private readonly List<Ped> _gangMembers = [];
    private readonly List<Vehicle> _gangVehicles = [];
    private readonly List<Vehicle> _policeVehicles = [];
    private readonly List<Ped> _policeOfficers = [];
    private Blip _areaBlip;
    internal override Location SpawnPoint { get; set; } = new(new Vector3(2350.661f, 4920.378f, 41.7339f));
    internal override float OnSceneDistance { get; set; } = 100f;
    internal override string CalloutName { get; set; } = "Officer Ambush";

    internal override void CalloutPrep()
    {
        CalloutMessage = "~r~Panic Button:~s~ Multiple officers under fire.";
        CalloutAdvisory = "Biker gang attacking sheriff officers. Multiple suspects armed.";
        Functions.PlayScannerAudioUsingPosition(
            "ATTENTION_ALL_SWAT_UNITS_01 WE_HAVE CRIME_BRANDISHING_WEAPON_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_01",
            SpawnPoint.Position
        );
    }

    internal override void CalloutAccepted()
    {
        Game.DisplayNotification(
            "3dtextures",
            "mpgroundlogo_cops",
            "~b~Dispatch",
            "~r~Biker Gang Attack",
            "~r~EMERGENCY~s~ All Units: Multiple officers under fire, 7 plus armed gang members attacking sheriff officers. ~r~Respond CODE-3"
        );

        // Construct the scene using the custom scene builder
        LostMc.ConstructBikersScene(
            out Vehicle policeCar1,
            out Vehicle policeCar2,
            out Ped officer1,
            out Ped officer2,
            out Ped officer3,
            out Vehicle motorcycle1,
            out Vehicle motorcycle2,
            out Vehicle motorcycle3,
            out Vehicle motorcycle4,
            out Vehicle motorcycle5,
            out Vehicle motorcycle6,
            out Vehicle motorcycle7,
            out Ped biker1,
            out Ped biker2,
            out Ped biker3,
            out Ped biker4,
            out Ped biker5,
            out Ped biker6,
            out Ped biker7,
            out Ped biker8,
            out Ped biker9,
            out Ped biker10
        );

        // Add vehicles to tracking lists
        _policeVehicles.AddRange([policeCar1, policeCar2]);
        _gangVehicles.AddRange([motorcycle1, motorcycle2, motorcycle3, motorcycle4, motorcycle5, motorcycle6, motorcycle7]);

        // Add officers to tracking list
        _policeOfficers.AddRange([officer1, officer2, officer3]);

        // Add bikers to tracking list
        _gangMembers.AddRange([biker1, biker2, biker3, biker4, biker5, biker6, biker7, biker8, biker9, biker10]);

        // Create search area blip
        _areaBlip = new Blip(SpawnPoint.Position.Around2D(1f, 2f), 80f) { Color = Color.Yellow, Alpha = .5f };
        _areaBlip.EnableRoute(Color.Yellow);
        BlipsToClear.Add(_areaBlip);

        // Make all entities persistent and add to cleanup
        foreach (var vehicle in _policeVehicles.Concat(_gangVehicles))
        {
            vehicle.IsPersistent = true;
            EntitiesToClear.Add(vehicle);
        }

        foreach (var biker in _gangMembers)
        {
            biker.IsPersistent = true;
            EntitiesToClear.Add(biker);
        }

        foreach (var officer in _policeOfficers)
        {
            officer.IsPersistent = true;
            EntitiesToClear.Add(officer);
        }
    }

    internal override void CalloutOnScene()
    {
        _areaBlip?.DisableRoute();

        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~b~Dispatch", "~r~OutNumbered", "~y~Stay in cover until backup arrives!");
        Functions.PlayScannerAudioUsingPosition("DISPATCH_SWAT_UNITS_FROM_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_99_01", SpawnPoint.Position);

        // Set relationship groups to make bikers hostile
        Game.SetRelationshipBetweenRelationshipGroups("LOSTERS", "COP", Relationship.Hate);
        Game.SetRelationshipBetweenRelationshipGroups("LOSTERS", "PLAYER", Relationship.Hate);

        // Make bikers wanted and fight
        foreach (var biker in _gangMembers.Where(b => b != null && b.Exists()))
        {
            biker.SetWanted(true);
            biker.Tasks.FightAgainstClosestHatedTarget(50f);
        }

        // Request backup
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);
        PyroFunctions.RequestBackup(Enums.BackupType.Code3);
    }

    internal override void CalloutRunning()
    {
        // End callout if player gets too far away after being on scene
        if (OnScene && Game.LocalPlayer.Character.DistanceTo(SpawnPoint.Position) > 90f)
            CalloutEnd();
    }

    internal override void CalloutEnd(bool forceCleanup = false)
    {
        Game.SetRelationshipBetweenRelationshipGroups("LOSTERS", "COP", Relationship.Neutral);
        Game.SetRelationshipBetweenRelationshipGroups("LOSTERS", "PLAYER", Relationship.Neutral);

        Game.DisplayHelp("Scene ~g~CODE 4", 5000);
        base.CalloutEnd(forceCleanup);
    }
}
