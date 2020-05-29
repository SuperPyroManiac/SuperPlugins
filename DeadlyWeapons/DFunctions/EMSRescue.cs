using System;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using Rage;
using Rage.Native;

namespace DeadlyWeapons.DFunctions
{
    internal class EmsRescue
    {
        private Ped Player => Game.LocalPlayer.Character;
        private EmsState _eState = EmsState.CheckDeath;
        private Vehicle emsVehicle;
        private Ped emsDriver;
        private Ped emsPassenger;
        internal GameFiber _emsFiber;

        internal void Start()
        {
            try
            {
                _emsFiber = new GameFiber(delegate
                {
                    while (true)
                    {
                        PlayerDieEvent();
                        GameFiber.Yield();
                    }
                });
                _emsFiber.Start();
            }
            catch (Exception e)
            {
                Game.LogTrivial("Oops there was an error here. Please send this log to SuperPyroManiac!");
                Game.LogTrivial("Deadly Weapons Error Report Start");
                Game.LogTrivial("======================================================");
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("======================================================");
                Game.LogTrivial("Deadly Weapons Error Report End");
            }
        }

        private void PlayerDieEvent()
        {
            switch (_eState)
            {
                case EmsState.CheckDeath:
                    if (Player.IsDead)
                    {
                        Game.DisableAutomaticRespawn = true;
                        Game.FadeScreenOutOnDeath = false;
                        GameFiber.Sleep(4000);
                        Game.FadeScreenOut(500, true);
                        GameFiber.Sleep(10);
                        Player.Resurrect();
                        if (Player)
                        {
                            Player.IsInvincible = true;
                            Player.IsRagdoll = true;
                        }
                        GameFiber.Sleep(10);
                        Game.HandleRespawn();
                        Game.FadeScreenIn(250, true);
                        Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~r~Officer Down", "~y~Panic Activated",
                            "Rescue has been dispatched. Police backup enroute.");
                        Functions.RequestBackup(Player.Position, EBackupResponseType.Code3, EBackupUnitType.LocalUnit);
                        emsVehicle = Functions.RequestBackup(Player.Position, EBackupResponseType.Code3, EBackupUnitType.Ambulance);
                        emsDriver = emsVehicle.Driver;
                        emsPassenger = emsVehicle.GetPedOnSeat(0);
                        _eState = EmsState.CheckDistance;
                    }
                    break;
                case EmsState.CheckDistance:
                    if (emsDriver?.DistanceTo(Player.Position) < 10f || emsPassenger?.DistanceTo(Player.Position) < 10f)
                    {
                        emsDriver.BlockPermanentEvents = true;
                        emsDriver.IsPersistent = true;
                        emsPassenger.BlockPermanentEvents = true;
                        emsPassenger.IsPersistent = true;
                        if (emsDriver.IsInAnyVehicle(true))
                        {
                            emsDriver.Tasks.ClearImmediately();
                            emsDriver.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                        }

                        if (emsPassenger.IsInAnyVehicle(true))
                        {
                            emsPassenger.Tasks.ClearImmediately();
                            emsPassenger.Tasks.LeaveVehicle(LeaveVehicleFlags.LeaveDoorOpen);
                        }
                        GameFiber.Wait(3500);
                        NativeFunction.Natives.TASK_GO_TO_ENTITY(emsDriver, Game.LocalPlayer.Character, -1, 2f, 2f,
                            0, 0);
                        NativeFunction.Natives.TASK_GO_TO_ENTITY(emsPassenger, Game.LocalPlayer.Character, -1, 2f, 2f,
                            0, 0);
                        GameFiber.Wait(3500);
                        _eState = EmsState.RescueTask;
                    }
                    break;
                case EmsState.RescueTask:
                    emsDriver.Tasks.PlayAnimation("mini@cpr@char_a@cpr_str", "cpr_pumpchest", 1000, AnimationFlags.None).WaitForCompletion(10000);
                    emsDriver.Tasks.PlayAnimation("mini@cpr@char_a@cpr_str", "cpr_success", 1000, AnimationFlags.None).WaitForCompletion(10000);
                    End();
                    break;
                case EmsState.End:
                    break;
            }
        }
        private void End()
        {
            Player.IsRagdoll = false;
            Player.IsInvincible = false;
            emsVehicle?.Dismiss();
            emsDriver?.Dismiss();
            emsPassenger?.Dismiss();
            _eState = EmsState.CheckDeath;
        }

        private void timeout()
        {
            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(60000);
                End();
            });
        }
        
        private enum EmsState
        {
            CheckDeath,
            CheckDistance,
            RescueTask,
            End
        }
    }
}