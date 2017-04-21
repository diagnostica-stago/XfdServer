namespace XfdServer.LedScroller
{
    public class FreeCommand : AbstractCommand
    {
        private readonly string _command;

        public FreeCommand(string command)
        {
            _command = command;
        }

        public override string[] GetCommands()
        {
            string[] commands = new string[1];
            commands[0] = CreateCommand(_command);
            return commands;
        }
    }
}