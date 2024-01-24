namespace SimpleECS.Component
{
    public class Behaviour : SComponent
    {
        /// <summary>
        /// Disabled只负责管理禁用Update与FixedUpdate
        /// </summary>
        public bool Disabled;
        public IBehaviourCtrl Ctrl { get; }
        private Behaviour(IBehaviourCtrl ctrl) { Ctrl = ctrl; }
        public static Behaviour CreateBehaviour(IBehaviourCtrl Ctrl)
        {
            return new Behaviour(Ctrl);
        }
        public static Behaviour CreateBehaviour<T>() where T : IBehaviourCtrl, new()
        {
            return new Behaviour(new T());
        }
        public void Start() { Ctrl.SetEntity(GetEntity()); Ctrl.Start(); }
        public void Update() { if (Disabled) { return; } Ctrl.Update(); }
        public void FixedUpdate() { if (Disabled) { return; } Ctrl.FixedUpdate(); }
        public void OnDestroy() { Ctrl.OnDestroy(); }
    }
}
