namespace MySynch.Q.Receiver
{
    public interface IReceiverService
    {
        void Start();
        void Stop();
        void Pause();
        void Continue();
        void Shutdown();
    }
}