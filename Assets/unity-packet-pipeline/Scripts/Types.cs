
namespace NPP_Types
{
    public enum MESSAGES : byte
    {
        Data = 0x00,
        Connection = 0x01,
        Disconnection = 0x02,
    }

    public enum CLIENTS : byte
    {
        SendReceive = 0x00,
        Send = 0x01,
        Receive = 0x02
    }
}
