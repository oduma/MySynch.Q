namespace MySynch.Q.Sender
{
    public interface IPublisher
    {
        void Initialize();
        void TryStart();
        void Stop();
    }
}