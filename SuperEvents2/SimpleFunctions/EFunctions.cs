#region

using System;
using LSPD_First_Response.Engine.Scripting.Entities;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;

#endregion

namespace SuperEvents2.SimpleFunctions
{
    public class EFunctions
    {
        private static TupleList<Vector3, float> _sideOfRoads = new TupleList<Vector3, float>();
        private static Tuple<Vector3, float> _chosenSpawnData;
        private static Random _rNd = new Random();

        public static Ped SetWanted(Ped wPed, bool isWanted) //Used to set a ped as wanted.
        {
            Persona thePersona = Functions.GetPersonaForPed(wPed);
            thePersona.Wanted = true;
            return wPed;
        }

        public bool IsWanted(Ped oPed) //Debugging: Used to check if the ped is wanted.
        {
            Persona persona = Functions.GetPersonaForPed(oPed);
            Game.LogTrivial("Ped is Wanted? = " + persona.Wanted);
            return persona.Wanted;
        }

        public static void SetDrunk(Ped bad, bool isDrunk)
        {
            GameFiber.StartNew(delegate
            {
                GameFiber.Yield();
                bad.Metadata.stpAlcoholDetected = isDrunk;
                var drunkAnimset = new AnimationSet("move_m@drunk@verydrunk");
                drunkAnimset.LoadAndWait();
                bad.MovementAnimationSet = drunkAnimset;
                Rage.Native.NativeFunction.Natives.SET_PED_IS_DRUNK(bad, isDrunk);
            });
            return;
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
            return;
        }

        public static void Damage(Vehicle vehicle, float radius, float amount)
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

        public static void SpawnNormalCar(out Vehicle cVehicle, Vector3 spawnPoint) //Spawn normal random car..
        {
            Model[] vehicleModels = { "DUKES", "BALLER", "BALLER2", "BISON", "BISON2", "BJXL", "CAVALCADE", "CHEETAH", "COGCABRIO", "ASEA", "ADDER", "FELON", "FELON2", "ZENTORNO", "WARRENER", "RAPIDGT", "INTRUDER", "FELTZER2", "FQ2", "RANCHERXL", "REBEL", "SCHWARZER", "COQUETTE", "CARBONIZZARE", "EMPEROR", "SULTAN", "EXEMPLAR", "MASSACRO", "DOMINATOR", "ASTEROPE", "PRAIRIE", "NINEF", "WASHINGTON", "CHINO", "CASCO", "INFERNUS", "ZTYPE", "DILETTANTE", "VIRGO", "F620", "PRIMO", "SULTAN", "EXEMPLAR", "F620", "FELON2", "FELON", "SENTINEL", "WINDSOR", "DOMINATOR", "DUKES", "GAUNTLET", "VIRGO", "ADDER", "BUFFALO", "ZENTORNO", "MASSACRO" };
            cVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], spawnPoint);
            cVehicle.IsPersistent = true;
        }

        public static void SpawnAnyCar(out Vehicle cVehicle, Vector3 spawnPoint) //Spawn ANY random car..
        {
            Model[] vehicleModels = { "NINFEF2", "BUS", "COACH", "AIRBUS", "AMBULANCE", "BARRACKS", "BARRACKS2", "BALLER", "BALLER2", "BANSHEE", "BJXL", "BENSON", "BOBCATXL", "BUCCANEER", "BUFFALO", "BUFFALO2", "BULLDOZER", "BULLET", "BURRITO", "BURRITO2", "BURRITO3", "BURRITO4", "BURRITO5", "CAVALCADE", "CAVALCADE2", "POLICET", "GBURRITO", "CAMPER", "CARBONIZZARE", "CHEETAH", "COMET2", "COGCABRIO", "COQUETTE", "GRESLEY", "DUNE2", "HOTKNIFE", "DUBSTA", "DUBSTA2", "DUMP", "DOMINATOR", "EMPEROR", "EMPEROR2", "EMPEROR3", "ENTITYXF", "EXEMPLAR", "ELEGY2", "F620", "FBI", "FBI2", "FELON", "FELON2", "FELTZER2", "FIRETRUK", "FQ2", "FUGITIVE", "FUTO", "GRANGER", "GAUNTLET", "HABANERO", "INFERNUS", "INTRUDER", "JACKAL", "JOURNEY", "JB700", "KHAMELION", "LANDSTALKER", "MESA", "MESA2", "MESA3", "MIXER", "MINIVAN", "MIXER2", "MULE", "MULE2", "ORACLE", "ORACLE2", "MONROE", "PATRIOT", "PBUS", "PACKER", "PENUMBRA", "PEYOTE", "POLICE", "POLICE2", "POLICE3", "POLICE4", "PHANTOM", "PHOENIX", "PICADOR", "POUNDER", "PRANGER", "PRIMO", "RANCHERXL", "RANCHERXL2", "RAPIDGT", "RAPIDGT2", "RENTALBUS", "RUINER", "RIOT", "RIPLEY", "SABREGT", "SADLER", "SADLER2", "SANDKING", "SANDKING2", "SHERIFF", "SHERIFF2", "SPEEDO", "SPEEDO2", "STINGER", "STOCKADE", "STINGERGT", "SUPERD", "STRATUM", "SULTAN", "AKUMA", "PCJ", "FAGGIO2", "DAEMON", "BATI2" };
            cVehicle = new Vehicle(vehicleModels[new Random().Next(vehicleModels.Length)], spawnPoint);
            cVehicle.IsPersistent = true;
        }
        
        public static void FireControl(Vector3 position, int children, bool isGasFire)
        {
            if (children > 25) return;
            NativeFunction.Natives.StartScriptFire(position.X, position.Y, position.Z, children, isGasFire);
        }

        public static void FindSideOfRoad(int maxDistance, int minDistance, out Vector3 spawnPoint, out float spawnPointH)
        {
            foreach (Tuple<Vector3, float> tuple in PulloverSpots.SideOfRoad)
            {
                if ((Vector3.Distance(tuple.Item1, Game.LocalPlayer.Character.Position) < maxDistance) && (Vector3.Distance(tuple.Item1, Game.LocalPlayer.Character.Position) > minDistance))
                {
                    _sideOfRoads.Add(tuple);
                }
            }
            if (_sideOfRoads.Count == 0)
            {
                Game.LogTrivial("SuperEvents: Failed to find valid spawnpoint. Spawning on road.");
                spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(45f, 100f));
                spawnPointH = 0;
            }
            else
            {
                _chosenSpawnData = _sideOfRoads[_rNd.Next(_sideOfRoads.Count)];
                    //_sideOfRoads.OrderBy(x => x.Item1.DistanceTo(Game.LocalPlayer.Character.Position)).FirstOrDefault();
                spawnPoint = _chosenSpawnData.Item1;
                spawnPointH = _chosenSpawnData.Item2;
            }
        }
    }
}
