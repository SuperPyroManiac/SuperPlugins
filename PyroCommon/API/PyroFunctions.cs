using System;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace PyroCommon.API;

public abstract class PyroFunctions
{
    private static readonly TupleList<Vector3, float> SideOfRoads = new();
    private static Tuple<Vector3, float> _chosenSpawnData;
    private static readonly Random RNd = new();
    
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
            "R300", "EXEMPLAR", "ORACLE", "PREVION", "BUFFALO4", "DOMINATOR", "DOMINATOR3", "JESTER3",
            "GAUNTLET4", "DOMINATOR7", "CINQUEMILA", "PREMIER", "SCHAFTER2", "TAILGATER2", "WASHINGTON", "BUFFALO",
            "BUFFALO2", "CALICO", "SULTAN", "SULTAN2", "BALLER7", "GRANGER2", "BURRITO3", "SPEEDO", "ASBO",
            "KANJO", "F620", "FELON", "JACKAL", "ORACLE2", "SENTINEL", "SENTINEL2", "KANJO", "KANJOSJ", "TAHOMA",
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
            "NINFEF2", "BUS", "COACH", "AIRBUS", "AMBULANCE", "BARRACKS", "BARRACKS2", "RT3000", "BALLER7",
            "PANTHERE", "BJXL", "BENSON", "YOSEMITE3", "VAMOS", "BUFFALO", "BUFFALO2", "BULLDOZER", "BULLET",
            "BURRITO", "BURRITO2", "BURRITO3", "BURRITO4", "BURRITO5", "CAVALCADE", "CAVALCADE2", "POLICET",
            "GBURRITO", "CAMPER", "JESTER4", "TENF", "COMET2", "COGCABRIO", "COQUETTE4", "GRESLEY", "DUNE2",
            "HOTKNIFE", "DUBSTA", "DUBSTA2", "DUMP", "DOMINATOR", "EMPEROR", "EMPEROR2", "EMPEROR3", "ENTITYXF",
            "EXEMPLAR", "ELEGY2", "ZR350", "FBI", "FBI2", "FELON", "FELON2", "FELTZER2", "FIRETRUK", "FQ2",
            "FUGITIVE", "FUTO2", "EVERON", "GAUNTLET", "TOROS", "INFERNUS2", "REMUS", "JACKAL", "JOURNEY",
            "JESTER3", "DRAFTER", "LANDSTALKER", "MESA", "MESA2", "MESA3", "MIXER", "MINIVAN", "MIXER2", "MULE",
            "MULE2", "ORACLE", "ORACLE2", "VISERIS", "PATRIOT3", "PBUS", "PACKER", "PENUMBRA2", "TULIP2", "POLICE",
            "POLICE2", "POLICE3", "POLICE4", "PHANTOM", "PHOENIX", "HELLION", "POUNDER", "PRANGER", "PRIMO",
            "BOOR", "RANCHERXL2", "COMET7", "RAPIDGT2", "RENTALBUS", "RUINER", "RIOT", "RIPLEY", "SABREGT",
            "SADLER", "SADLER2", "SANDKING", "SANDKING2", "SHERIFF", "SHERIFF2", "SPEEDO", "SPEEDO2", "YOUGA4",
            "STOCKADE", "TORERO", "SUPERD", "STRATUM", "SULTAN3", "AKUMA", "PCJ", "FAGGIO2", "DAEMON", "BATI2"
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

    public static void FindSideOfRoad(int maxDistance, int minDistance, out Vector3 spawnPoint,
        out float spawnPointH)
    {
        foreach (var tuple in PulloverSpots.SideOfRoad)
            if (Vector3.Distance(tuple.Item1, Game.LocalPlayer.Character.Position) < maxDistance &&
                Vector3.Distance(tuple.Item1, Game.LocalPlayer.Character.Position) > minDistance)
                SideOfRoads.Add(tuple);
        if (SideOfRoads.Count == 0)
        {
            Log.Info("Failed to find valid spawnpoint. Spawning on road.");
            spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(45f, 100f));
            spawnPointH = 0;
        }
        else
        {
            _chosenSpawnData = SideOfRoads[RNd.Next(SideOfRoads.Count)];
            //_sideOfRoads.OrderBy(x => x.Item1.DistanceTo(Game.LocalPlayer.Character.Position)).FirstOrDefault();
            spawnPoint = _chosenSpawnData.Item1;
            spawnPointH = _chosenSpawnData.Item2;
        }
    }
}