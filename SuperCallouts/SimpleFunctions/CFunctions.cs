#region

using System;
using System.Linq;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;

#endregion

namespace SuperCallouts.SimpleFunctions;

internal class CFunctions
{
    private static readonly TupleList<Vector3, float> SideOfRoads = new();
    private static Tuple<Vector3, float> _chosenSpawnData;
    private static readonly Random RNd = new();

    internal static readonly Func<string, bool> IsLoaded = plugName =>
        Functions.GetAllUserPlugins().Any(assembly => assembly.GetName().Name.Equals(plugName));

    internal static Ped SetWanted(Ped wPed, bool isWanted) //Used to set a ped as wanted.
    {
        var thePersona = Functions.GetPersonaForPed(wPed);
        thePersona.Wanted = true;
        return wPed;
    }

    internal static bool IsWanted(Ped oPed) //Debugging: Used to check if the ped is wanted.
    {
        var persona = Functions.GetPersonaForPed(oPed);
        Game.Console.Print("Ped is Wanted? = " + persona.Wanted);
        return persona.Wanted;
    }

    internal static void SetDrunk(Ped bad, bool isDrunk)
    {
        GameFiber.StartNew(delegate
        {
            GameFiber.Yield();
            bad.Metadata.stpAlcoholDetected = isDrunk;
            var drunkAnimset = new AnimationSet("move_m@drunk@verydrunk");
            drunkAnimset.LoadAndWait();
            bad.MovementAnimationSet = drunkAnimset;
            NativeFunction.Natives.x95D2D383D5396B8A(bad, isDrunk);
        });
    }

    internal static void SetAnimation(Ped person, string theAnimation)
    {
        GameFiber.StartNew(delegate
        {
            GameFiber.Yield();
            var drunkAnimset = new AnimationSet(theAnimation);
            drunkAnimset.LoadAndWait();
            person.MovementAnimationSet = drunkAnimset;
        });
    }

    internal static void Damage(Vehicle vehicle, float radius, float amount)
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

    internal static void
        SpawnNormalCar(out Vehicle cVehicle, Vector3 spawnPoint, float heading = 0) //Spawn normal random car..
    {
        try
        {
            Model[] vehicleModels =
            {
                "DUKES", "BALLER", "BALLER2", "BISON", "BISON2", "BJXL", "CAVALCADE", "CHEETAH", "COGCABRIO",
                "ASEA", "ADDER", "FELON", "FELON2", "ZENTORNO", "WARRENER", "RAPIDGT", "INTRUDER", "FELTZER2",
                "FQ2", "RANCHERXL", "REBEL", "SCHWARZER", "COQUETTE", "CARBONIZZARE", "EMPEROR", "SULTAN",
                "EXEMPLAR", "MASSACRO", "DOMINATOR", "ASTEROPE", "PRAIRIE", "NINEF", "WASHINGTON", "CHINO", "CASCO",
                "INFERNUS", "ZTYPE", "DILETTANTE", "VIRGO", "F620", "PRIMO", "SULTAN", "EXEMPLAR", "F620", "FELON2",
                "FELON", "SENTINEL", "WINDSOR", "DOMINATOR", "DUKES", "GAUNTLET", "VIRGO", "ADDER", "BUFFALO",
                "ZENTORNO", "MASSACRO"
            };
            cVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], spawnPoint);
            cVehicle.IsPersistent = true;
        }
        catch (Exception e)
        {
            Game.Console.Print(
                "Oops there was an error spawning a vehicle. Using generic. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print("SuperCallouts Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print(e.ToString());
            Game.Console.Print("======================================================");
            Game.Console.Print("SuperCallouts Error Report End");
            cVehicle = new Vehicle("FELON", spawnPoint);
            cVehicle.IsPersistent = true;
        }
    }

    internal static void Code4Message()
    {
        GameFiber.StartNew(delegate
        {
            var bigMessage = new BigMessageThread();
            bigMessage.MessageInstance.ShowColoredShard("Code 4", "Callout Ended", HudColor.Green, HudColor.Black,
                2);
        });
    }

    internal static void SpawnAnyCar(out Vehicle cVehicle, Vector3 spawnPoint, float heading = 0) //Spawn ANY car..
    {
        try
        {
            Model[] vehicleModels =
            {
                "NINFEF2", "BUS", "COACH", "AIRBUS", "AMBULANCE", "BARRACKS", "BARRACKS2", "BALLER", "BALLER2",
                "BANSHEE", "BJXL", "BENSON", "BOBCATXL", "BUCCANEER", "BUFFALO", "BUFFALO2", "BULLDOZER", "BULLET",
                "BURRITO", "BURRITO2", "BURRITO3", "BURRITO4", "BURRITO5", "CAVALCADE", "CAVALCADE2", "POLICET",
                "GBURRITO", "CAMPER", "CARBONIZZARE", "CHEETAH", "COMET2", "COGCABRIO", "COQUETTE", "GRESLEY",
                "DUNE2", "HOTKNIFE", "DUBSTA", "DUBSTA2", "DUMP", "DOMINATOR", "EMPEROR", "EMPEROR2", "EMPEROR3",
                "ENTITYXF", "EXEMPLAR", "ELEGY2", "F620", "FBI", "FBI2", "FELON", "FELON2", "FELTZER2", "FIRETRUK",
                "FQ2", "FUGITIVE", "FUTO", "GRANGER", "GAUNTLET", "HABANERO", "INFERNUS", "INTRUDER", "JACKAL",
                "JOURNEY", "JB700", "KHAMELION", "LANDSTALKER", "MESA", "MESA2", "MESA3", "MIXER", "MINIVAN",
                "MIXER2", "MULE", "MULE2", "ORACLE", "ORACLE2", "MONROE", "PATRIOT", "PBUS", "PACKER", "PENUMBRA",
                "PEYOTE", "POLICE", "POLICE2", "POLICE3", "POLICE4", "PHANTOM", "PHOENIX", "PICADOR", "POUNDER",
                "PRANGER", "PRIMO", "RANCHERXL", "RANCHERXL2", "RAPIDGT", "RAPIDGT2", "RENTALBUS", "RUINER", "RIOT",
                "RIPLEY", "SABREGT", "SADLER", "SADLER2", "SANDKING", "SANDKING2", "SHERIFF", "SHERIFF2", "SPEEDO",
                "SPEEDO2", "STINGER", "STOCKADE", "STINGERGT", "SUPERD", "STRATUM", "SULTAN", "AKUMA", "PCJ",
                "FAGGIO2", "DAEMON", "BATI2"
            };
            cVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], spawnPoint);
            cVehicle.IsPersistent = true;
        }
        catch (Exception e)
        {
            Game.Console.Print(
                "Oops there was an error spawning a vehicle. Using generic. Please send this log to https://dsc.gg/ulss");
            Game.Console.Print("SuperCallouts Error Report Start");
            Game.Console.Print("======================================================");
            Game.Console.Print(e.ToString());
            Game.Console.Print("======================================================");
            Game.Console.Print("SuperCallouts Error Report End");
            cVehicle = new Vehicle("FELON", spawnPoint);
            cVehicle.IsPersistent = true;
        }
    }

    internal static void BuildUi(out MenuPool interaction, out UIMenu mainMenu, out UIMenu convoMenu,
        out UIMenuItem questioning, out UIMenuItem endCall)
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

    internal static void FireControl(Vector3 position, int children, bool isGasFire)
    {
        if (children > 25) return;
        NativeFunction.Natives.x6B83617E04503888(position.X, position.Y, position.Z, children, isGasFire);
    }

    internal static void FindSideOfRoad(int maxDistance, int minDistance, out Vector3 spawnPoint,
        out float spawnPointH)
    {
        foreach (var tuple in PulloverSpots.SideOfRoad)
            if (Vector3.Distance(tuple.Item1, Game.LocalPlayer.Character.Position) < maxDistance &&
                Vector3.Distance(tuple.Item1, Game.LocalPlayer.Character.Position) > minDistance)
                SideOfRoads.Add(tuple);
        if (SideOfRoads.Count == 0)
        {
            Game.Console.Print("SuperCallouts: Failed to find valid spawnpoint. Spawning on road.");
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