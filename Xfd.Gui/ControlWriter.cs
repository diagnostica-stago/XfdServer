using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;

namespace Xfd.Gui
{
    public class ControlWriter : TextWriter
    {
        private readonly TextEditor _textEditor;
        private ScrollViewer _scrollViewer;

        public ControlWriter(TextEditor textEditor)
        {
            _textEditor = textEditor;
            _textEditor.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = _textEditor.GetChildOfType<ScrollViewer>();
        }

        public override void Write(char value)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _textEditor.AppendText(value.ToString());
                AutoScrollDown();
            }));
        }

        public override void Write(string value)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _textEditor.AppendText(value);
                AutoScrollDown();
            }));
        }

        private void AutoScrollDown()
        {
            if (_scrollViewer == null)
            {
                return;
            }
            if (Math.Abs(_scrollViewer.VerticalOffset - _scrollViewer.ScrollableHeight) < 0.001)
            {
                _textEditor.ScrollToEnd();
            }
        }

        public override Encoding Encoding => Encoding.UTF8;
    }
}