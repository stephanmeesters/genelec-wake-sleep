namespace GenelecApp
{
    /*
    
    https://github.com/markbergsma/genlc/blob/master/src/genlc/gnet.py
    
    Gnet is RS-485 based, and the GLM adapter bridges between RS485 and USB HID
    with little processing.

    Packet format appears to be as follows:
    +---------------+---------------+---------------+---------------+---------------+---------------+
    |   00 ... 07   |   08 ... 15   |  16 ... N-1   |   N...N+7   | N+8 ... N+15  | N+16 ... N+23 |
    +---------------+---------------+---------------+---------------+---------------+---------------+
    | address(RID) | command(CID) |     data      |     checksum(CRC16/GSM)      |  terminator   |
    |               |               |  (optional)   |     of payload(=)            |     (0x7E)    |
    +===============+===============+===============+---------------+---------------+---------------+\

    Fields:

    - address
        Target address of the device the query is directed to.
        0x01        GLM adapter
        0x02...     SAM monitors (after assignment)
        0xF0        Gnet multicast address (all monitors, before assignment)
        0xFF        Gnet broadcast address (all devices)

    - command (CID)
        Command/query code - see const.py

    - data
        Optional, variable length field with data bytes, depending on CID

    - checksum
        CRC16/GSM checksum of the message, starting with the address byte, up to
        and including the data bytes

    - terminator
        Terminator byte, always 0x7E
    */

    internal class GNetMessage
    {
        public GNetMessage(byte address, byte command)
        {
            Message.Add(address);
            Message.Add(command);
            Message.AddRange(CalculateCRC16GSM(Message));
            Message.Add(Constants.GNET_TERM);
        }
        public GNetMessage(byte address, byte command, byte[] data)
        {
            Message.Add(address);
            Message.Add(command);
            Message.AddRange(data);
            Message.AddRange(CalculateCRC16GSM(Message));
            Message.Add(Constants.GNET_TERM);
        }
        private static byte[] CalculateCRC16GSM(List<byte> data)
        {
            ushort crc = 0x0000;
            foreach (byte b in data)
            {
                crc ^= (ushort)(b << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            crc ^= 0xFFFF;

            byte[] bytes = BitConverter.GetBytes(crc);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        public List<byte> Message { get; } = new List<byte>();
    }
}
