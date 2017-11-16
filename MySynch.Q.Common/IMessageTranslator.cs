using MySynch.Q.Common.Contracts;

namespace MySynch.Q.Common
{
    public interface IMessageTranslator
    {
        TransferMessage Translate(TransferMessage inMessage);
    }
}
