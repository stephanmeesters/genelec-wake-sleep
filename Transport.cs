using HidApiAdapter;

namespace GenelecApp
{
    internal class Transport : IDisposable
    {
        public Transport(HidDevice hidDevice)
        {
            HidDevice = hidDevice;
            HidDevice.Connect();
        }

        public void Send(GNetMessage message)
        {
            var data = Escape(message.Message);

            var payload = new List<byte>();
            payload.Add((byte)(0x80 + data.Count));
            payload.AddRange(data);
#if DEBUG
            Console.WriteLine($"Send command: {BitConverter.ToString(payload.ToArray())}");
#endif
            payload.AddRange(Enumerable.Repeat((byte)0x0, 64 - payload.Count));

            Thread.Sleep(3);
            var result = HidDevice.Write(payload.ToArray());
            if(result == -1)
            {
                Console.WriteLine("Write error occurred.");
            }
            
        }

        private static List<byte> Escape(List<byte> msg)
        {
            // Perform PPP stuffing
            if (msg[^1] != Constants.GNET_TERM)
            {
                throw new ArgumentException("The last byte in the message must be GNET_TERM.");
            }

            // The message up to the terminator should be escaped with "PPP stuffing"
            List<byte> escapedMsg = new List<byte>();

            for (int i = 0; i < msg.Count - 1; i++)
            {
                byte currentByte = msg[i];

                if (currentByte == 0x7D)
                {
                    escapedMsg.Add(0x7D);
                    escapedMsg.Add(0x5D);
                }
                else if (currentByte == 0x7E)
                {
                    escapedMsg.Add(0x7D);
                    escapedMsg.Add(0x5E);
                }
                else
                {
                    escapedMsg.Add(currentByte);
                }
            }

            escapedMsg.Add(Constants.GNET_TERM);

            return escapedMsg;
        }

        public void Dispose()
        {
            HidDevice.Disconnect();
        }

        public HidDevice HidDevice { get; }
    }
}
