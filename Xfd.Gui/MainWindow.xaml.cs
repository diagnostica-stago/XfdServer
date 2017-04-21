using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using XfdServer;

namespace Xfd.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                MainClass.Main(new string[] {});
            });
            Console.SetOut(new ControlWriter(xamlTextBlock));


            NotifyIcon ni = new NotifyIcon();
            ni.Icon = new Icon("Assets/TrafficLight.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }
    }
}
