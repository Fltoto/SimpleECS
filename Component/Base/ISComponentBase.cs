namespace SimpleECS
{
    public interface ISComponentBase
    {
        public ushort UID { get; }
        public void SetUID(ushort UID);
    }
}
