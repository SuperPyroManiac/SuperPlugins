using System;
using LSPD_First_Response.Mod.API;
using PyroCommon.PyroFunctions;
using Rage;
using SuperEvents.Attributes;

namespace SuperEvents.Events;

[EventInfo("Officer Under Fire", "Help the other officer!")]
internal class PulloverShooting : AmbientEvent
{
    private Ped? _cPed;
    private Vehicle? _cVehicle;
    private Vector3 _spawnPoint;
    private float _spawnPointH;
    private Ped? _sPed;
    private Vehicle? _sVehicle;

    private Tasks _tasks = Tasks.CheckDistance;

    protected override Vector3 EventLocation { get; set; }

    protected override void OnStartEvent()
    {
        //Setup
        PyroFunctions.FindSideOfRoad(120, 45, out _spawnPoint, out _spawnPointH);
        EventLocation = _spawnPoint;
        if (_spawnPoint.DistanceTo(Player) < 35f)
        {
            EndEvent(true);
            return;
        }

        //Vehicles
        _cVehicle = new Vehicle("POLICE2", _spawnPoint) { Heading = _spawnPointH, IsPersistent = true };
        PyroFunctions.SpawnNormalCar(out _sVehicle, _cVehicle.GetOffsetPositionFront(8));
        _sVehicle.Metadata.searchDriver = "~r~baggy of meth~s~, ~g~a pair of shoes~s~";
        _sVehicle.Metadata.searchTrunk = "~y~stacks of past due medical bills~s~";
        EntitiesToClear.Add(_cVehicle);
        EntitiesToClear.Add(_sVehicle);
        //Peds
        _cPed = new Ped("s_m_y_cop_01", Vector3.Zero, 0f) { IsPersistent = true, BlockPermanentEvents = true };
        _cPed.WarpIntoVehicle(_cVehicle, -1);
        _cPed.Inventory.Weapons.Add(WeaponHash.CombatPistol).Ammo = -1;
        _sPed = new Ped { IsPersistent = true, Health = 400, BlockPermanentEvents = true };
        _sPed.WarpIntoVehicle(_sVehicle, -1);
        _sPed.Inventory.Weapons.Add(WeaponHash.BullpupShotgun).Ammo = -1;
        PyroFunctions.SetWanted(_sPed, true);
        _sPed.Metadata.stpAlcoholDetected = true;
        _sPed.Metadata.hasGunPermit = false;
        _sPed.Metadata.searchPed = "~r~assault rifle~s~, ~r~pistol~s~, ~r~used meth pipe~s~, ~y~suicide letter~s~";
        EntitiesToClear.Add(_cPed);
        EntitiesToClear.Add(_sPed);
    }

    protected override void OnProcess()
    {
        try
        {
            if ( _sPed == null || _cPed == null )
            {
                EndEvent(true);
                return;
            }
            
            switch (_tasks)
            {
                case Tasks.CheckDistance:
                    if (Game.LocalPlayer.Character.DistanceTo(_spawnPoint) < 30f)
                    {
                        _tasks = Tasks.OnScene;
                    }

                    break;
                case Tasks.OnScene:
                    var choice = new Random(DateTime.Now.Millisecond).Next(1, 4);
                    Log.Info("PulloverShooting event picked scenerio #" + choice);
                    switch (choice)
                    {
                        case 1:
                            _sPed.BlockPermanentEvents = false;
                            _sPed.Tasks.FightAgainst(_cPed);
                            _cPed.BlockPermanentEvents = false;
                            _cPed.Tasks.FightAgainst(_sPed);
                            break;
                        case 2:
                            _sPed.BlockPermanentEvents = false;
                            _sPed.Tasks.FightAgainst(Player);
                            _cPed.Kill();
                            break;
                        case 3:
                            _sPed.BlockPermanentEvents = false;
                            var pursuit = Functions.CreatePursuit();
                            Functions.AddPedToPursuit(pursuit, _sPed);
                            Functions.SetPursuitIsActiveForPlayer(pursuit, Player);
                            _cPed.Kill();
                            break;
                        default:
                            EndEvent(true);
                            break;
                    }

                    _tasks = Tasks.End;
                    break;
                case Tasks.End:
                    break;
                default:
                    EndEvent(true);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error( e.ToString());
            EndEvent(true);
        }
    }

    protected override void OnCleanup()
    {
    }

    private enum Tasks
    {
        CheckDistance,
        OnScene,
        End
    }
}