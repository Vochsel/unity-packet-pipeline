
namespace UPP_Types
{
    public enum MESSAGES : byte
    {
        Data = 0x00,
        Connection = 0x01,
        Disconnection = 0x02,
    }

    public enum CLIENT_TYPE : byte
    {
        Send = 0x00,
        Receive = 0x01
    }
}
