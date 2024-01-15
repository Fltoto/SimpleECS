namespace SimpleECS
{
    public abstract class SComponent : ISComponent
    {
        public ushort uid;
        public ushort UID => uid;
        public SEntity Entity { get; private set; }

        public SEntity GetEntity()
        {
            return Entity;
        }

        public void SetEntity(SEntity Entity)
        {
            this.Entity = Entity;
        }

        public void SetUID(ushort UID)
        {
            uid = UID;
        }
    }
}
