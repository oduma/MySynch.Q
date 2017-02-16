namespace MySynch.Q.Sender
{
    public interface ISenderService
    {
        void Start();
        void Stop();
        void Continue();
        void Pause();
        void Shutdown();
    }
}