namespace SimpleECS
{
    public interface ISGlobalComponent : ISComponentBase
    {
        public ushort UID { get; }
        public void SetUID(ushort UID);
    }
}
