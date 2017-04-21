using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using TeamCitySharp;
using XfdServer.LedScroller;

namespace XfdServer
{
    public class MainClass
    {
        private static ITeamCityClient _buildClient;
        private static Projects _expectedProjects;

        private static string _url;
        private static string _login;
        private static string _password;
        private static string _comPort;
        private static string _lastCommand = "";
        private static DateTime _lastUpdateTime;

        private const string URL_KEY = "url";
        private const string LOGIN_KEY = "login";
        private const string PASSWORD_KEY = "password";
        private const string COM_PORT_KEY = "comPort";
        private const string CONFIGURATION_FILE = "xfd.xml";

        public static void ExtractConfiguration()
        {
            _url = ConfigurationManager.AppSettings[URL_KEY];
            _login = ConfigurationManager.AppSettings[LOGIN_KEY];
            _password = ConfigurationManager.AppSettings[PASSWORD_KEY];
            _comPort = ConfigurationManager.AppSettings[COM_PORT_KEY];

            _expectedProjects = ConfigurationXmlFileReader.Read(CONFIGURATION_FILE);
        }

        public static void InitializeTeamcityCommunication()
        {
            var client = new TeamCityClient(_url);
            _buildClient = client;
            client.Connect(_login, _password);
        }

        public static void Main(string[] args)
        {
            ExtractConfiguration();
            InitializeTeamcityCommunication();

            DeviceDriver d = new DeviceDriver(_comPort);
            d.Write(new Brightness(TextLineDescriptors.BRIGHTNESS));


            var standUpHour = 11;
            var standUpMinute = 59;
            var standUpSecond = 30;

            var projects = new List<ProjectDescriptor>();
            foreach (var tcProject in _expectedProjects.Project)
            {
                projects.Add(new ProjectDescriptor(tcProject, _buildClient));
            }

            string endConstruction = _expectedProjects.Config.Single(x => x.name == "End Construction").value;
            string startConstruction = _expectedProjects.Config.Single(x => x.name == "Start Construction").value;
            DateTime endConstructionDate = DateTime.Parse(endConstruction);
            DateTime startConstructionDate = DateTime.Parse(startConstruction);
            TimeSpan remain = endConstructionDate - DateTime.Now;

            DisplayData(standUpHour, standUpMinute, standUpSecond, d, startConstructionDate, remain, endConstructionDate, projects);
        }

        private static void DisplayData(int standUpHour, int standUpMinute, int standUpSecond, DeviceDriver d, DateTime startConstructionDate, TimeSpan remain, DateTime endConstructionDate, List<ProjectDescriptor> projects)
        {
            while (true)
            {
                TextLineCollection c = new TextLineCollection();
                var shortDateString = DateTime.Now.TimeOfDay;
                if (shortDateString.Hours == standUpHour && shortDateString.Minutes >= standUpMinute && shortDateString.Minutes < standUpMinute + 1 && shortDateString.Seconds > standUpSecond)
                {
                    DisplayStandUp(d, c);
                }
                else if (_expectedProjects.Config.Single(x => x.name == "DisplayOption").value == "remainingTime")
                {
                    DisplayRemainingTime(d, c, startConstructionDate, remain, endConstructionDate);
                }
                else
                {
                    DisplayTeamCityProject(d, c, projects);
                }
            }
        }

        private static void DisplayRemainingTime(DeviceDriver d, TextLineCollection c, DateTime startConstructionDate, TimeSpan remain, DateTime endConstructionDate)
        {
            c.Add(TextLineDescriptors.Time, "   " + DateTime.Now.ToShortTimeString());
            c.Add(TextLineDescriptors.Time, " " + DateTime.Now.ToShortDateString());
            c.Add(TextLineDescriptors.Motd, "Fin Construction -> J - " + remain.Days);
            c.Add(TextLineDescriptors.SuccessDescriptor, "Deb : " + startConstructionDate.ToShortDateString());
            c.Add(TextLineDescriptors.WarnedDescriptor, "Fin : " + endConstructionDate.ToShortDateString());

            if (c.ToString() != _lastCommand)
            {
                try
                {
                    d.Write(c);
                    d.Write(new SelectPages(c.Count));
                    _lastCommand = c.ToString();
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Console.WriteLine("No message changes");
            }

            Thread.Sleep(10000);
        }

        private static void DisplayStandUp(DeviceDriver d, TextLineCollection c)
        {
            c.Add(TextLineDescriptors.StandUp, "   Stand Up !   ");

            if (c.ToString() != _lastCommand)
            {
                try
                {
                    d.Write(c);
                    d.Write(new SelectPages(c.Count));
                    _lastCommand = c.ToString();
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Console.WriteLine("No message changes");
            }
            Thread.Sleep(10000);
        }

        private static void DisplayTeamCityProject(DeviceDriver d, TextLineCollection c, IEnumerable<ProjectDescriptor> projects)
        {
            var timeFromLastUpdate = (DateTime.Now - _lastUpdateTime);
            bool mustUpdateData = timeFromLastUpdate > TimeSpan.FromMinutes(1);
            if (mustUpdateData)
            {
                _lastUpdateTime = DateTime.Now;
            }
            try
            {
                foreach (var proj in projects)
                {
                    // fill the data with teamcity request
                    if (mustUpdateData)
                    {
                        proj.UpdateFromTeamCity();
                    }
                    // get the lines to display on the XFD
                    var projectXfdLines = proj.ComputeXfdLines();
                    c.AddRange(projectXfdLines);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while extracting status: {0}", e.Message);
                c.Add(TextLineDescriptors.Xfd, "Result extraction error");
            }

            var currentCommand = c.ToString();
            if (currentCommand != _lastCommand)
            {
                try
                {
                    d.Write(c);
                    d.Write(new SelectPages(c.Count));
                    _lastCommand = currentCommand;
                }
                catch (Exception)
                {
                }
            }
            else
            {
                Console.WriteLine("No message changes ({0})", DateTime.Now);
            }

            Thread.Sleep(TimeSpan.FromSeconds(10));
        }
    }
}
