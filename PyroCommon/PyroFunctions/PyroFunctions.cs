using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using PyroCommon.Objects;
using PyroCommon.UIManager;
using PyroCommon.Wrappers;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using YamlDotNet.Serialization;
using Location = PyroCommon.API.Location;

namespace PyroCommon.PyroFunctions;

public static class PyroFunctions
{
    internal static CalloutInfoAttribute[] RegisteredScCallouts { get; } = GenerateCalloutNames().ToArray();
    
    public static T DeserializeYaml<T>(string path, string resourceFileName)
    {
        try
        {
            if (!File.Exists(path))
            {
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
                using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"PyroCommon.Libs.Resources.{resourceFileName}");
                using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                resourceStream?.CopyTo(fileStream);
            }
            using var reader = new StreamReader(path);
            return new DeserializerBuilder().Build().Deserialize<T>(reader);
        }
        catch (Exception e)
        {
            Log.Error($"Error deserializing YAML at {path}: {e.Message}");
            throw;
        }
    }

    
    public static void AddDrugItem(string item, Enums.DrugType drugType, Ped ped = null, Vehicle vehicle = null, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere)
    {
        if (ped != null && Main.UsingStp) SearchItems.AddStpPedSearchItems(ped, item);
        if (vehicle != null && Main.UsingStp) SearchItems.AddStpVehicleDriverSearchItems(vehicle, item);
        //if (Main.UsingPr) SearchItems.AddDrugItem(item, drugType, itemLocation, ped, vehicle);
    }
    public static void AddWeaponItem(string item, string weaponId, Ped ped = null, Vehicle vehicle = null, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere)
    {
        if (ped != null && Main.UsingStp) SearchItems.AddStpPedSearchItems(ped, item);
        if (vehicle != null && Main.UsingStp) SearchItems.AddStpVehicleDriverSearchItems(vehicle, item);
        //if (Main.UsingPr) SearchItems.AddWeaponItem(item, weaponId, itemLocation, ped, vehicle);
    }
    public static void AddFirearmItem(string item, string weaponId, bool visible, bool stolen, bool equiped = false, Ped ped = null, Vehicle vehicle = null, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere)
    {
        if (ped != null)
        {
            ped.Inventory.GiveNewWeapon(weaponId, -1, equiped);
            if (Main.UsingStp) SearchItems.AddStpPedSearchItems(ped, item);
        }
        if (vehicle != null && Main.UsingStp) SearchItems.AddStpVehicleDriverSearchItems(vehicle, item);
        //if (Main.UsingPr) SearchItems.AddFirearmItem(item, weaponId, visible, stolen, itemLocation, ped, vehicle);
    }
    public static void AddSearchItem(string item, Ped ped = null, Vehicle vehicle = null, Enums.ItemLocation itemLocation = Enums.ItemLocation.Anywhere)
    {
        if (ped != null && Main.UsingStp) SearchItems.AddStpPedSearchItems(ped, item);
        if (vehicle != null && Main.UsingStp) SearchItems.AddStpVehicleDriverSearchItems(vehicle, item);
        //if (Main.UsingPr) SearchItems.AddSearchItem(item, itemLocation, ped, vehicle);
    }
    
    public static void ClearSearchItems(Ped ped = null, Vehicle vehicle = null)
    {
        if (ped != null)
        {
            if (Main.UsingStp) ped.Metadata.searchPed = string.Empty;
            ped.Inventory.Weapons.Clear();
        }
        if (vehicle != null && Main.UsingStp)
        {
            vehicle.Metadata.searchDriver = string.Empty;
            vehicle.Metadata.searchPassenger = string.Empty;
            vehicle.Metadata.searchTrunk = string.Empty;
        }
        //if (Main.UsingPr) SearchItems.ClearAllItems(ped, vehicle);
    }
    
    public static void DrawMarker(Enums.MarkerType type, Vector3 position, float scale, Color color, bool bounce)
    {
        NativeFunction.Natives.x28477EC23D892089((int)type, position.X, position.Y, position.Z, 0, 0, 0, 0, 180, 0, scale, scale, scale, color.R, color.G, color.B, color.A, bounce, true, 1, false, 0, 0, false);
        //PyroFunctions.DrawMarker(Enums.MarkerType.Arrow, _cVehicle.Position, 0.3f, Color.Red, true);
    }

    public static Vehicle SpawnCar(Location location)
    {
        Model[] vehicleModels =
        [
            "PRAIRIE", "EXEMPLAR", "ORACLE", "PREVION", "BUFFALO4", "DOMINATOR", "DOMINATOR3", "GAUNTLET",
            "GAUNTLET4", "ASTEROPE", "CINQUEMILA", "PREMIER", "SCHAFTER2", "TAILGATER", "WASHINGTON", "BUFFALO",
            "BUFFALO2", "CALICO", "SULTAN", "SULTAN2", "BALLER2", "GRANGER2", "BURRITO3", "SPEEDO", "BLISTA",
            "KANJO", "F620", "FELON", "JACKAL", "ORACLE2", "SENTINEL", "SENTINEL2", "ZION", "KANJOSJ", "RUINER",
            "ELLIE", "VIGERO2", "KOMODA", "VECTRE", "CAVALCADE", "CAVALCADE2", "REBLA", "SEMINOLE2", "ASTRON",
            "ASTRON2"
        ];
        var cVehicle = new Vehicle(vehicleModels[new Random(DateTime.Now.Millisecond).Next(vehicleModels.Length)], location.Position, location.Heading);
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
            return new Location(World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(minDistance, maxDistance)));
        }
        return matches[new Random(DateTime.Now.Millisecond).Next(matches.Count)];
    }
    
    internal static LHandle StartPursuit(bool areSuspectsPulledOver, bool randomizePursuitAttributes, params Ped[] suspects)
    {
        if (areSuspectsPulledOver) Functions.ForceEndCurrentPullover();
        var pursuitLHandle = Functions.CreatePursuit();

        foreach (var suspect in suspects)
        {
            if (randomizePursuitAttributes)
            {
                RandomizePursuitAttributes(suspect);
            }
            Functions.AddPedToPursuit(pursuitLHandle, suspect);
        }
        Functions.SetPursuitIsActiveForPlayer(pursuitLHandle, true);
        
        return pursuitLHandle;
    }
    
    private static void RandomizePursuitAttributes(Ped ped)
    {
        var rnd = new Random(DateTime.Now.Millisecond);
        var pursuitAttributes = Functions.GetPedPursuitAttributes(ped);

        // Max driving speed must be above min driving speed at all times or else it throws an error and/or crashes the game
        pursuitAttributes.MaxDrivingSpeed = MathHelper.ConvertMilesPerHourToMetersPerSecond(rnd.Next(61, 100));
        pursuitAttributes.MinDrivingSpeed = MathHelper.ConvertMilesPerHourToMetersPerSecond(rnd.Next(30, 60));

        // These cannot be above 2f or else the driving starts glitching out
        pursuitAttributes.HandlingAbility = (float)Math.Round((float)(rnd.NextDouble() * (2.0 - 0.1) + 0.1), 1);
        pursuitAttributes.HandlingAbilityTurns = (float)Math.Round((float)(rnd.NextDouble() * (2.0 - 0.1) + 0.1), 1);

        pursuitAttributes.HandlingAbilityBurstTireMult = rnd.Next(-2, 1);

        pursuitAttributes.ExhaustionDuration = rnd.Next(3, 10) * 1000;
        pursuitAttributes.ExhaustionInterval = rnd.Next(20, 60) * 1000;

        pursuitAttributes.BurstTireSurrenderMult = rnd.Next(0, 3);
        pursuitAttributes.SurrenderChanceTireBurst = rnd.Next(0, 25);
        pursuitAttributes.SurrenderChanceTireBurstAndCrashed = rnd.Next(10, 40);

        pursuitAttributes.SurrenderChanceCarBadlyDamaged = rnd.Next(0, 20);

        pursuitAttributes.SurrenderChancePitted = rnd.Next(1, 20);
        pursuitAttributes.SurrenderChancePittedAndCrashed = rnd.Next(1, 30);
        pursuitAttributes.SurrenderChancePittedAndSlowedDown = rnd.Next(0, 25);

        // In seconds
        pursuitAttributes.AverageBurstTireSurrenderTime = rnd.Next(30, 120);
        pursuitAttributes.AverageSurrenderTime = rnd.Next(750, 10000);
    }
    
    //TODO: These will be removed when the remaster is complete!
    
    [Obsolete("Method is deprecated, please use Ped.SetWalkAnimation instead.")]
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
    
    [Obsolete("Method is deprecated, please use Vehicle.ApplyDamage instead.")]
    public static void DamageVehicle(Vehicle vehicle, float radius, float amount)
    {
        var model = vehicle.Model;
        model.GetDimensions(out var vector31, out var vector32);
        var num = new Random(DateTime.Now.Millisecond).Next(10, 45);
        for (var index = 0; index < num; ++index)
        {
            var randomInt1 = MathHelper.GetRandomSingle(vector31.X, vector32.X);
            var randomInt2 = MathHelper.GetRandomSingle(vector31.Y, vector32.Y);
            var randomInt3 = MathHelper.GetRandomSingle(vector31.Z, vector32.Z);
            vehicle.Deform(new Vector3(randomInt1, randomInt2, randomInt3), radius, amount);
        }
    }
    
    [Obsolete("Method is deprecated, please use Ped.SetWanted instead.")]
    public static Ped SetWanted(Ped ped, bool isWanted)
    {
        if (!ped.Exists()) return null;
        var thePersona = Functions.GetPersonaForPed(ped);
        thePersona.Wanted = true;
        return ped;
    }
    
    [Obsolete("Method is deprecated, please use Ped.SetDrunk instead.")]
    public static void SetDrunk(Ped ped, Enums.DrunkState drunkState)
    {
        GameFiber.StartNew(delegate
        {
            GameFiber.Yield();
            if (!ped.Exists()) return;
            // if (Main.UsingPr)
            // {
            //     PedInfo.SetDrunk(ped, drunkState);
            //     return;
            // }
            ped.Metadata.stpAlcoholDetected = true;
            var drunkAnimset = new AnimationSet("move_m@drunk@verydrunk");
            drunkAnimset.LoadAndWait();
            ped.MovementAnimationSet = drunkAnimset;
            NativeFunction.Natives.x95D2D383D5396B8A(ped, true);
        });
    }
    
    [Obsolete("Method is deprecated, please use SpawnCar instead.")]
    public static void SpawnNormalCar(out Vehicle cVehicle, Vector3 spawnPoint, float heading = 0) //Spawn normal random car..
    {
        Model[] vehicleModels =
        [
            "PRAIRIE", "EXEMPLAR", "ORACLE", "PREVION", "BUFFALO4", "DOMINATOR", "DOMINATOR3", "GAUNTLET",
            "GAUNTLET4", "ASTEROPE", "CINQUEMILA", "PREMIER", "SCHAFTER2", "TAILGATER", "WASHINGTON", "BUFFALO",
            "BUFFALO2", "CALICO", "SULTAN", "SULTAN2", "BALLER2", "GRANGER2", "BURRITO3", "SPEEDO", "BLISTA",
            "KANJO", "F620", "FELON", "JACKAL", "ORACLE2", "SENTINEL", "SENTINEL2", "ZION", "KANJOSJ", "RUINER",
            "ELLIE", "VIGERO2", "KOMODA", "VECTRE", "CAVALCADE", "CAVALCADE2", "REBLA", "SEMINOLE2", "ASTRON",
            "ASTRON2"
        ];
        cVehicle = new Vehicle(vehicleModels[new Random(DateTime.Now.Millisecond).Next(vehicleModels.Length)], spawnPoint);
        cVehicle.IsPersistent = true;
    }

    [Obsolete("Method is deprecated, please use SpawnCar instead.")]
    public static void SpawnAnyCar(out Vehicle cVehicle, Vector3 spawnPoint, float heading = 0) //Spawn ANY random car..
    {
        Model[] vehicleModels =
        [
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
        ];
        cVehicle = new Vehicle(vehicleModels[new Random(DateTime.Now.Millisecond).Next(vehicleModels.Length)], spawnPoint);
        cVehicle.IsPersistent = true;
    }

    [Obsolete("Method is deprecated, please use built in methods instead.")]
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
        Style.ApplyStyle(interaction, false);
    }

    [Obsolete("Method is deprecated, please use SetDrunk instead.")]
    public static void SetDrunkOld(Ped ped, bool isDrunk)
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

    [Obsolete("Method is deprecated, please use GetSideOfRoad instead.")]
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
            var match = matches[new Random(DateTime.Now.Millisecond).Next(matches.Count)];
            spawnPoint = match.Position;
            spawnPointH = match.Heading;
        }
    }
    
    public static Keys ConvertStringToClosestKey(string input, Keys defaultKey)
    {
        try
        {
            if (Enum.TryParse<Keys>(input, true, out var key)) return key;
            var allKeys = Enum.GetNames(typeof(Keys));
            var closestMatch = allKeys.FirstOrDefault(k => k.StartsWith(input, StringComparison.OrdinalIgnoreCase));
            if (closestMatch != null && Enum.TryParse<Keys>(closestMatch, out var matchedKey)) return matchedKey;
            Game.DisplayHelp("Invalid key. Please try again.");
            return defaultKey;
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            return defaultKey;
        }
    }
    
    internal static void ProcessMsg(string plainText)
    {
        if ( !Settings.ErrorReporting ) return;
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(Assembly.GetCallingAssembly().FullName.Split(',').First().PadRight(32).Substring(0, 32));
        aes.GenerateIV(); using var tfm = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText + Assembly.GetCallingAssembly().FullName.Split(',').First());
        var encryptedBytes = tfm.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        try
        {
            var data = Encoding.UTF8.GetBytes(Convert.ToBase64String(aes.IV) + ":" + Convert.ToBase64String(encryptedBytes));
            new TcpClient("158.69.120.20", 8055).GetStream().Write(data, 0, data.Length);
        }
        catch (Exception ex) { Log.Error($"Error sending message to server: {ex.Message}", false); }
    }
    
    private static IEnumerable<CalloutInfoAttribute> GenerateCalloutNames()
    {
        return Functions.GetAllUserPlugins().First(assembly => assembly.GetName().Name.Equals("SuperCallouts")).GetTypes().SelectMany(type => type.GetCustomAttributes<CalloutInfoAttribute>());
    }
}