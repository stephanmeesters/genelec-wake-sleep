using HidApi;

namespace GenelecApp
{
    internal class Transport : IDisposable
    {
        public Transport(DeviceInfo deviceInfo)
        {
            HidDevice = deviceInfo.ConnectToDevice();
        }

        public void Send(GNetMessage message)
        {
            var data = Escape(message.Message);

            var payload = new List<byte>();
            payload.Add(0x0);
            payload.Add((byte)(0x80 + data.Count));
            payload.AddRange(data);
#if DEBUG
            Console.WriteLine($"Send command: {BitConverter.ToString(payload.ToArray())}");
#endif
            payload.AddRange(Enumerable.Repeat((byte)0x0, 65 - payload.Count));

            Console.WriteLine($"Send command: {BitConverter.ToString(payload.AsReadOnly().ToArray())}");
            

            Thread.Sleep(3);
            try
            {
                HidDevice.Write(payload.AsReadOnly().ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Write error occurred: {ex}");
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
            HidDevice.Dispose();
        }

        public Device HidDevice { get; }
    }
}
