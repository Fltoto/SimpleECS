using SimpleECS.Component;
using System;

namespace SimpleECS.System
{
    public class BehaviourSys : ISSystem
    {
        public BehaviourSys(SWorld world) {
            world.OnAddEntity += OnEnter;
            world.OnRemoveEntity += OnExit;
        }
        private void OnEnter(SEntity entity) {
            var be = entity.GetComponents<Behaviour>();
            if (be==null) {
                return;
            }
            foreach (var b in be) {
                (b as Behaviour).Start();
            }
        }
        private void OnExit(SEntity entity) {
            var be = entity.GetComponents<Behaviour>();
            if (be == null)
            {
                return;
            }
            foreach (var b in be)
            {
                (b as Behaviour).OnDestroy();
            }
        }
        public Type GetAimComponet()
        {
            return typeof(Behaviour);
        }

        public void Update(ISComponent Component)
        {
            (Component as Behaviour).Update();
        }
    }
}
