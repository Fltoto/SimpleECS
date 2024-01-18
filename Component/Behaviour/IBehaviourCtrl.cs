namespace SimpleECS.Component
{
    public interface IBehaviourCtrl
    {
        public void SetEntity(SEntity entity);
        public SEntity GetEntity();
        public void Start();
        public void Update();
        public void OnDestroy();
    }
}
