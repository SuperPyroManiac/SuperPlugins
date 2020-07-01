using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;

namespace SuperEvents2.Events
{
    public class CarAccident : AmbientEvent
    {
        public override void StartEvent(Vector3 spawnPoint, float spawnPointH)
        {
            base.StartEvent(spawnPoint, spawnPointH);
        }

        protected override void Process()
        {
            base.Process();
        }

        protected override void Interactions(UIMenu sender, UIMenuItem selItem, int index)
        {
            base.Interactions(sender, selItem, index);
        }

        protected override void Conversations(UIMenu sender, UIMenuItem selItem, int index)
        {
            base.Conversations(sender, selItem, index);
        }
    }
}