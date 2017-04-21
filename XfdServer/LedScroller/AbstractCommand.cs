namespace XfdServer.LedScroller
{
    public abstract class AbstractCommand
    {
        readonly string _deviceId = "00";

        protected AbstractCommand()
        {
        }

        protected AbstractCommand(string deviceId)
        {
            _deviceId = deviceId;
        }

        protected string CreateCommand(string commandBody)
        {
            string result = "<ID" + _deviceId + ">" + commandBody;

            byte checksum = 0;
            foreach (char c in commandBody)
            {
                checksum = (byte)(checksum ^ (byte)c);
            }
            result += checksum.ToString("X02");

            return result + "<E>";
        }

        public virtual string[] GetCommands()
        {
            return new[] { ToString() };
        }
    }
}
