namespace SimpleECS.Component {
    public abstract class BehaviourCtrl : IBehaviourCtrl
    {
        private SEntity Entity;
        public SEntity GetEntity()
        {
            return Entity;
        }

        public virtual void OnDestroy()
        {
            
        }

        public void SetEntity(SEntity entity)
        {
            Entity = entity;
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate() { 
        }
    }
}
