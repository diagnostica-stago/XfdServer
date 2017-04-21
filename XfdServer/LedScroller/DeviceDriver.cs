using System;
using System.IO.Ports;
using System.Threading;

namespace XfdServer.LedScroller
{
    public sealed class DeviceDriver : IDisposable
    {
        readonly SerialPort _p;

        public DeviceDriver(string port)
        {
            _p = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
        }

        public void Write(AbstractCommand command)
        {
            _p.Open();
            try
            {
                foreach (string c in command.GetCommands())
                {
                    Console.WriteLine(c);
                    _p.Write(c);
                    Thread.Sleep(100);
                }
            }
            catch (TimeoutException)
            {
            }
            finally
            {
                _p.Close();
            }
        }

        public void Dispose()
        {
            _p?.Dispose();
        }
    }
}
