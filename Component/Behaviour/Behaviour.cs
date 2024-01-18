namespace SimpleECS.Component
{
    public class Behaviour:SComponent
    {
        public IBehaviourCtrl Ctrl { get; }
        private Behaviour(IBehaviourCtrl ctrl) { Ctrl = ctrl; }
        public static Behaviour CreateBehaviour(IBehaviourCtrl Ctrl) {
            return new Behaviour(Ctrl);
        }
        public static Behaviour CreateBehaviour<T>()  where T:IBehaviourCtrl,new(){
            return new Behaviour(new T());
        }
        public void Start() { Ctrl.SetEntity(GetEntity()); Ctrl.Start(); }
        public void Update() { Ctrl.Update(); }
        public void OnDestroy() { Ctrl.OnDestroy(); }
    }
}
