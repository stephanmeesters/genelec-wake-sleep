using HidApi;
using System.Diagnostics;

namespace GenelecApp
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
            if (args.Any() && Enum.TryParse(args[0], true, out RunAction action))
            {
                RunCommand(action);
            }
            else
            {
                Console.WriteLine($"Usage: {ExecutableName} [sleep, wake]");
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

        static void WakeUp(DeviceInfo device)
        {
            using var transporter = new Transport(device);

            var dataQueue = new List<byte[]>();
            dataQueue.Add(new byte[] { 3, 0x7F });
            dataQueue.Add(new byte[] { 3, 1 });

            foreach(var data in dataQueue)
            {
                var msg = new GNetMessage(Constants.GNET_BROADCAST_ADDR, Constants.CID_WAKEUP, data);
                transporter.Send(msg);
                transporter.Send(msg);
            }
        }

        static void Shutdown(DeviceInfo device)
        {
            using var transporter = new Transport(device);

            var dataQueue = new List<byte[]>();
            dataQueue.Add(new byte[] { 0x03, 0x02 });
            dataQueue.Add(new byte[] { 0x03, 0x00 });

            foreach (var data in dataQueue)
            {
                var msg = new GNetMessage(Constants.GNET_BROADCAST_ADDR, Constants.CID_WAKEUP, data);
                transporter.Send(msg);
                transporter.Send(msg);
            }
        }

        static DeviceInfo? FindGLMDevice()
        {
            foreach (var deviceInfo in Hid.Enumerate())
            {
                using var device = deviceInfo.ConnectToDevice();
                if (device.GetDeviceInfo().VendorId == Constants.GENELEC_GLM_VID &&
                    device.GetDeviceInfo().ProductId == Constants.GENELEC_GLM_PID)
                    return deviceInfo;
            }

            return null;
        }

        static string ExecutableName => Path.GetFileName(Process.GetCurrentProcess()?.MainModule?.FileName) ?? "GenelecApp.exe";
    }
}