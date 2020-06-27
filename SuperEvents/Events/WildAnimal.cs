using System;
using System.Drawing;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace SuperEvents.Events
{
    public class WildAnimal : AmbientEvent
    {
        private Ped _animal;
        private Blip _cBlip;
        private Vector3 _spawnPoint;
        private bool _onScene;
        //UI Items
        private readonly MenuPool _interaction = new MenuPool();
        private readonly UIMenu _mainMenu = new UIMenu("SuperEvents", "~y~Choose an option.");
        private readonly UIMenuItem _endCall = new UIMenuItem("~y~End Event", "Ends the event early.");
        internal static void Launch()
        {
            var EventBooter = new WildAnimal();
            EventBooter.StartEvent();
        }
        protected override void StartEvent()
        {
            _spawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(150f));
            Model[] meanAnimal = {"A_C_MTLION", "A_C_COYOTE"};
            _animal = new Ped(meanAnimal[new Random().Next(meanAnimal.Length)], _spawnPoint, 50) {IsPersistent = true};
            //Start UI
            _interaction.Add(_mainMenu);
            _mainMenu.AddItem(_endCall);
            _mainMenu.RefreshIndex();
            _mainMenu.OnItemSelect += Interactions;
            if (!Settings.ShowBlips)
            {
                base.StartEvent();
                return;
            }
            _cBlip = _animal.AttachBlip();
            _cBlip.Color = Color.Red;
            _cBlip.Scale = .5f;
            base.StartEvent();
        }
        protected override void MainLogic()
        {
            GameFiber.StartNew(delegate
            {
                while (EventsActive)
                {
                    try
                    {
                        GameFiber.Yield();
                        if (!_onScene && Game.LocalPlayer.Character.DistanceTo(_animal) < 20f)
                        {
                            _onScene = true;
                            _animal.Tasks.FightAgainst(Game.LocalPlayer.Character);
                            if (Settings.ShowHints)
                                Game.DisplayNotification("3dtextures", "mpgroundlogo_cops", "~y~Officer Sighting",
                                    "~r~Wild Animal", "Stop the animal from hurting anyone.");
                            Game.DisplayHelp("~y~Press ~r~" + Settings.Interact + "~y~ to open interaction menu.");
                        }
                        if (_onScene && _animal.IsDead) End();
                        if (_onScene && Game.LocalPlayer.Character.DistanceTo(_animal) > 100f) End();
                        if (Game.IsKeyDown(Settings.EndEvent)) End();
                        if (Game.IsKeyDown(Settings.Interact))
                        {
                            _mainMenu.Visible = !_mainMenu.Visible;
                        }
                        _interaction.ProcessMenus();
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivial("Oops there was an error here. Please send this log to https://discord.gg/xsdAXJb");
                        Game.LogTrivial("SuperEvents Error Report Start");
                        Game.LogTrivial("======================================================");
                        Game.LogTrivial(e.ToString());
                        Game.LogTrivial("======================================================");
                        Game.LogTrivial("SuperEvents Error Report End");
                        End();
                    }
                }
            });
            base.MainLogic();
        }
        protected override void End()
        {
            _interaction.CloseAllMenus();
            if (_cBlip) _cBlip.Delete();
            if (_animal) _animal.Dismiss();
            base.End();
        }
        private void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            if (selItem == _endCall)
            {
                Game.DisplaySubtitle("~y~Event Ended.");
                End();
            }
        }

    }
}