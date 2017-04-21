namespace XfdServer.LedScroller
{
    public class SelectPages : AbstractCommand
    {
        static readonly string _pagesString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        readonly string _pages;

        public SelectPages(string pages)
        {
            _pages = pages;
        }

        public SelectPages(int count)
        {
            _pages = _pagesString.Substring(0, count);
        }

        public override string ToString()
        {
            return CreateCommand("<TA>00010100009912302359" + _pages);
        }
    }
}
