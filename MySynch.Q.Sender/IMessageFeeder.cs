using System;
using MySynch.Q.Common.Contracts;

namespace MySynch.Q.Sender
{
    public interface IMessageFeeder
    {
        void Initialize();
        Action<BodyTransferMessage> PublishMessage { get; set; }
        Func<bool> ShouldPublishMessage { get; set; }
        bool More { get; set; }
    }
}