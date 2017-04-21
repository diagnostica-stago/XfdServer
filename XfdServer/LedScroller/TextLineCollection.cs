using System;
using System.Collections;

namespace XfdServer.LedScroller
{
    public class TextLineCollection : AbstractCommand, ICollection
    {
        readonly ArrayList _textLines;

        public TextLineCollection()
        {
            _textLines = new ArrayList();
        }

        public void Add(string text)
        {
            _textLines.Add(new TextLine(text));
        }

        public void Add(TextLineDescriptor desc, string text)
        {
            _textLines.Add(new TextLine(desc, text));
        }

        public void Add(TextLine line)
        {
            _textLines.Add(line);
        }

        public void AddRange(IEnumerable list)
        {
            foreach (var elem in list)
            {
                _textLines.Add(elem);
            }
        }

        public override string[] GetCommands()
        {
            char page = 'A';
            string[] result = new string[_textLines.Count + 2];
            result[0] = CreateCommand("<BE>");
            result[_textLines.Count + 1] = CreateCommand("<BF>");
            for (int i = 0; i < _textLines.Count; i++)
            {
                TextLine line = _textLines[i] as TextLine;
                line.Page = page;
                line.GetCommands().CopyTo(result, i + 1);
                page = (char)(page + 1);
            }
            return result;
        }

        public override string ToString()
        {
            return string.Join("", GetCommands());
        }

        #region ICollection Member

        public void CopyTo(Array array, int index)
        {
            _textLines.CopyTo(array, index);
        }

        public int Count => _textLines.Count;

        public bool IsSynchronized => _textLines.IsSynchronized;

        public object SyncRoot => _textLines.SyncRoot;

        #endregion

        #region IEnumerable Member

        public IEnumerator GetEnumerator()
        {
            return _textLines.GetEnumerator();
        }

        #endregion
    }
}
