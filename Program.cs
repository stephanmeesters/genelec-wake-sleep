using HidApiAdapter;

namespace GenelecApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        enum RunAction
        {
            WAKE = 0,
            SLEEP = 1
        }
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide an integer argument 0 or 1 (WAKE or SLEEP).");
                return;
            }

            if (Enum.TryParse(args[0], out RunAction action))
            {
                if (Enum.IsDefined(typeof(RunAction), action))
                {
                    RunCommand(action);
                }
                else
                {
                    Console.WriteLine("Invalid input. The argument should be 0 or 1 (WAKE or SLEEP).");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. The argument should be 0 or 1 (WAKE or SLEEP).");
            }
        }

        static void RunCommand(RunAction action)
        {
            var device = FindGLMDevice();
            if (device == null)
            { 
                Console.WriteLine("USB device could not be found.");
                return;
            }

            switch(action)
            {
                case RunAction.WAKE:
                    WakeUp(device);
                    break;
                case RunAction.SLEEP:
                    Shutdown(device);
                    break ;
            }
        }

        static void WakeUp(HidDevice device)
        {
            using var transporter = new Transport(device);

            var dataQueue = new List<byte[]>();
            dataQueue.Add(new byte[] { 3, 0x7F });
            dataQueue.Add(new byte[] { 3, 1 });

            foreach(var data in dataQueue)
            {
                var msg = new GNetMessage(Constants.GNET_BROADCAST_ADDR, Constants.CID_WAKEUP, data);
                // Send broadcast message twice
                Thread.Sleep(30);
                transporter.Send(msg);
                Thread.Sleep(30);
                transporter.Send(msg);
            }
        }

        static void Shutdown(HidDevice device)
        {
            using var transporter = new Transport(device);

            var dataQueue = new List<byte[]>();
            dataQueue.Add(new byte[] { 0x03, 0x02 });
            dataQueue.Add(new byte[] { 0x03, 0x00 });

            foreach (var data in dataQueue)
            {
                var msg = new GNetMessage(Constants.GNET_BROADCAST_ADDR, Constants.CID_WAKEUP, data);
                // Send broadcast message twice
                Thread.Sleep(30);
                transporter.Send(msg);
                Thread.Sleep(30);
                transporter.Send(msg);
            }
        }

        static HidDevice? FindGLMDevice()
        {
            var devices = HidDeviceManager.GetManager().SearchDevices(0, 0);
            if (devices.Any())
            {
                foreach (var device in devices)
                {
                    device.Connect();

                    var vid = device.VendorId;
                    var pid = device.ProductId;

                    device.Disconnect();

                    if (vid == Constants.GENELEC_GLM_VID &&
                        pid == Constants.GENELEC_GLM_PID)
                        return device;
                }
            }

            return null;
        }
    }
}