namespace MySynch.Q.Receiver
{
    public interface IConsummer
    {
        void Initialize();
        void Stop();
        void TryStart(object obj);
        bool More { get; set; }
    }
}