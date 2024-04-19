using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace PyroCommon.API;

public abstract class PyroFunctions
{
    public static Vehicle SpawnCar(Location location)
    {
        Model[] vehicleModels =
        {
            "PRAIRIE", "EXEMPLAR", "ORACLE", "PREVION", "BUFFALO4", "DOMINATOR", "DOMINATOR3", "GAUNTLET",
            "GAUNTLET4", "ASTEROPE", "CINQUEMILA", "PREMIER", "SCHAFTER2", "TAILGATER", "WASHINGTON", "BUFFALO",
            "BUFFALO2", "CALICO", "SULTAN", "SULTAN2", "BALLER2", "GRANGER2", "BURRITO3", "SPEEDO", "BLISTA",
            "KANJO", "F620", "FELON", "JACKAL", "ORACLE2", "SENTINEL", "SENTINEL2", "ZION", "KANJOSJ", "RUINER",
            "ELLIE", "VIGERO2", "KOMODA", "VECTRE", "CAVALCADE", "CAVALCADE2", "REBLA", "SEMINOLE2", "ASTRON",
            "ASTRON2"
        };
        var cVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], location.Position, location.Heading);
        cVehicle.IsPersistent = true;
        return cVehicle;
    }
    
    public static Ped SpawnPed(Location location)
    {
        var cPed = new Ped(location.Position, location.Heading);
        cPed.IsPersistent = true;
        cPed.BlockPermanentEvents = true;
        return cPed;
    }

    public static Blip CreateSearchBlip(Location location, Color color, bool route = false, bool randomize = false, float size = 80f)
    {
        if (randomize) location.Position = location.Position.Around2D(size / 2, size - 5);
        var cBlip = new Blip(location.Position, size);
        cBlip.Alpha = 0.5f;
        cBlip.Color = color;
        if (route) cBlip.EnableRoute(color);
        return cBlip;
    }

    
    
    
    //TODO: Redo all these shitty old functions. I swear I was retarded when I made these.
    internal static void Ragdoll(Ped ped)
    {
        try
        {
            GameFiber.StartNew(delegate
            {
                if (!ped.Exists()) return;
                ped.IsRagdoll = true;
                GameFiber.Wait(500);
                if (!ped.Exists()) return;
                ped.IsRagdoll = false;
            });
        }
        catch (Exception)
        {
            Log.Warning("Unable to remove ragdoll. Most likely the subject died first.");
        }
    }
    
    public static void SpawnNormalCar(out Vehicle cVehicle, Vector3 spawnPoint, float heading = 0) //Spawn normal random car..
    {
        Model[] vehicleModels =
        {
            "PRAIRIE", "EXEMPLAR", "ORACLE", "PREVION", "BUFFALO4", "DOMINATOR", "DOMINATOR3", "GAUNTLET",
            "GAUNTLET4", "ASTEROPE", "CINQUEMILA", "PREMIER", "SCHAFTER2", "TAILGATER", "WASHINGTON", "BUFFALO",
            "BUFFALO2", "CALICO", "SULTAN", "SULTAN2", "BALLER2", "GRANGER2", "BURRITO3", "SPEEDO", "BLISTA",
            "KANJO", "F620", "FELON", "JACKAL", "ORACLE2", "SENTINEL", "SENTINEL2", "ZION", "KANJOSJ", "RUINER",
            "ELLIE", "VIGERO2", "KOMODA", "VECTRE", "CAVALCADE", "CAVALCADE2", "REBLA", "SEMINOLE2", "ASTRON",
            "ASTRON2"
        };
        cVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], spawnPoint);
        cVehicle.IsPersistent = true;
    }

    public static void SpawnAnyCar(out Vehicle cVehicle, Vector3 spawnPoint, float heading = 0) //Spawn ANY random car..
    {
        Model[] vehicleModels =
        {
            "NINFEF2", "BUS", "COACH", "AIRBUS", "AMBULANCE", "BARRACKS", "BARRACKS2", "BALLER", "BALLER2",
            "BANSHEE", "BJXL", "BENSON", "BOBCATXL", "BUCCANEER", "BUFFALO", "BUFFALO2", "BULLDOZER", "BULLET",
            "BURRITO", "BURRITO2", "BURRITO3", "BURRITO4", "BURRITO5", "CAVALCADE", "CAVALCADE2", "POLICET",
            "GBURRITO", "CAMPER", "CARBONIZZARE", "CHEETAH", "COMET2", "COGCABRIO", "COQUETTE", "GRESLEY", "DUNE2",
            "HOTKNIFE", "DUBSTA", "DUBSTA2", "DUMP", "DOMINATOR", "EMPEROR", "EMPEROR2", "EMPEROR3", "ENTITYXF",
            "EXEMPLAR", "ELEGY2", "F620", "FBI", "FBI2", "FELON", "FELON2", "FELTZER2", "FIRETRUK", "FQ2",
            "FUGITIVE", "FUTO", "GRANGER", "GAUNTLET", "HABANERO", "INFERNUS", "INTRUDER", "JACKAL", "JOURNEY",
            "JB700", "KHAMELION", "LANDSTALKER", "MESA", "MESA2", "MESA3", "MIXER", "MINIVAN", "MIXER2", "MULE",
            "MULE2", "ORACLE", "ORACLE2", "MONROE", "PATRIOT", "PBUS", "PACKER", "PENUMBRA", "PEYOTE", "POLICE",
            "POLICE2", "POLICE3", "POLICE4", "PHANTOM", "PHOENIX", "PICADOR", "POUNDER", "PRANGER", "PRIMO",
            "RANCHERXL", "RANCHERXL2", "RAPIDGT", "RAPIDGT2", "RENTALBUS", "RUINER", "RIOT", "RIPLEY", "SABREGT",
            "SADLER", "SADLER2", "SANDKING", "SANDKING2", "SHERIFF", "SHERIFF2", "SPEEDO", "SPEEDO2", "STINGER",
            "STOCKADE", "STINGERGT", "SUPERD", "STRATUM", "SULTAN", "AKUMA", "PCJ", "FAGGIO2", "DAEMON", "BATI2"
        };
        cVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], spawnPoint);
        cVehicle.IsPersistent = true;
    }

    public static void DamageVehicle(Vehicle vehicle, float radius, float amount)
    {
        var model = vehicle.Model;
        model.GetDimensions(out var vector31, out var vector32);
        var num = new Random().Next(10, 45);
        for (var index = 0; index < num; ++index)
        {
            var randomInt1 = MathHelper.GetRandomSingle(vector31.X, vector32.X);
            var randomInt2 = MathHelper.GetRandomSingle(vector31.Y, vector32.Y);
            var randomInt3 = MathHelper.GetRandomSingle(vector31.Z, vector32.Z);
            vehicle.Deform(new Vector3(randomInt1, randomInt2, randomInt3), radius, amount);
        }
    }

    internal static void BuildUi(out MenuPool interaction, out UIMenu mainMenu, out UIMenu convoMenu,
        out UIMenuItem questioning, out UIMenuItem endCall) //TODO: Redo swat calls
    {
        interaction = new MenuPool();
        mainMenu = new UIMenu("SuperCallouts", "Choose an option.");
        convoMenu = new UIMenu("SuperCallouts", "~s~Choose an option.");
        questioning = new UIMenuItem("Speak With Subjects");
        endCall = new UIMenuItem("~y~End Callout", "Ends the callout.");
        interaction.Add(mainMenu);
        interaction.Add(convoMenu);
        mainMenu.AddItem(questioning);
        mainMenu.AddItem(endCall);
        mainMenu.BindMenuToItem(convoMenu, questioning);
        convoMenu.ParentMenu = mainMenu;
        questioning.Enabled = false;
        mainMenu.AllowCameraMovement = true;
        mainMenu.MouseControlsEnabled = false;
        convoMenu.AllowCameraMovement = true;
        convoMenu.MouseControlsEnabled = false;
        mainMenu.RefreshIndex();
        convoMenu.RefreshIndex();
    }

    public static void SetAnimation(Ped person, string theAnimation)
    {
        GameFiber.StartNew(delegate
        {
            GameFiber.Yield();
            var drunkAnimset = new AnimationSet(theAnimation);
            drunkAnimset.LoadAndWait();
            person.MovementAnimationSet = drunkAnimset;
        });
    }

    public static Ped SetWanted(Ped ped, bool isWanted) //Used to set a ped as wanted.
    {
        if (!ped.Exists()) return null;
        var thePersona = Functions.GetPersonaForPed(ped);
        thePersona.Wanted = true;
        return ped;
    }

    public static bool IsWanted(Ped oPed) //Debugging: Used to check if the ped is wanted.
    {
        var persona = Functions.GetPersonaForPed(oPed);
        Log.Info("Ped is Wanted? = " + persona.Wanted);
        return persona.Wanted;
    }

    public static void SetDrunk(Ped ped, bool isDrunk)
    {
        GameFiber.StartNew(delegate
        {
            GameFiber.Yield();
            if (!ped.Exists()) return;
            ped.Metadata.stpAlcoholDetected = isDrunk;
            var drunkAnimset = new AnimationSet("move_m@drunk@verydrunk");
            drunkAnimset.LoadAndWait();
            ped.MovementAnimationSet = drunkAnimset;
            NativeFunction.Natives.x95D2D383D5396B8A(ped, isDrunk);
        });
    }

    public static void FireControl(Vector3 position, int children, bool isGasFire)
    {
        if (children > 25) return;
        NativeFunction.Natives.x6B83617E04503888(position.X, position.Y, position.Z, children, isGasFire);
    }
    
    public static Location GetSideOfRoad(int maxDistance, int minDistance)
    {
        var matches = new List<Location>();
        foreach (var location in Locations.SideOfRoad)
        {
            if (Vector3.Distance(location.Position, Game.LocalPlayer.Character.Position) < maxDistance &&
                Vector3.Distance(location.Position, Game.LocalPlayer.Character.Position) > minDistance)
                matches.Add(location);
        }

        if (matches.Count == 0)
        {
            Log.Info("Failed to find valid spawnpoint. Spawning on road.");
            return new Location(World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(minDistance, maxDistance)), 0);
        }
        return matches[new Random().Next(matches.Count)];
    }

    [Obsolete("Method is deprecated, please use GetSideOfRoad instead.")] //TODO: Delete this
    public static void FindSideOfRoad(int maxDistance, int minDistance, out Vector3 spawnPoint, out float spawnPointH)
    {
        var matches = new List<Location>();
        foreach (var location in Locations.SideOfRoad)
        {
            if (Vector3.Distance(location.Position, Game.LocalPlayer.Character.Position) < maxDistance &&
                Vector3.Distance(location.Position, Game.LocalPlayer.Character.Position) > minDistance) 
                matches.Add(location);
        }

        if (matches.Count == 0)
        {
            Log.Info("Failed to find valid spawnpoint. Spawning on road.");
            spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(45f, 100f));
            spawnPointH = 0;
        }
        else
        {
            var match = matches[new Random().Next(matches.Count)];
            spawnPoint = match.Position;
            spawnPointH = match.Heading;
        }
    }
}