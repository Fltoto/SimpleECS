namespace SimpleECS
{
    public abstract class SGlobalComponent : ISGlobalComponent
    {
        public ushort uid;
        public ushort UID => uid;

        public void SetUID(ushort UID)
        {
            uid = UID;
        }
    }
}
