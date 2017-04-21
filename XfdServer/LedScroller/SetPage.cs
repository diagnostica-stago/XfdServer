namespace XfdServer.LedScroller
{
    public class SetPage : AbstractCommand
    {
        readonly char _page;

        public SetPage(char page)
        {
            _page = page;
        }

        public override string ToString()
        {
            return CreateCommand("<RP" + _page + ">");
        }
    }
}
