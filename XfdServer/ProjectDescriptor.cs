using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamCitySharp.DomainEntities;
using TeamCitySharp;
using XfdServer.LedScroller;

namespace XfdServer
{
    public class ProjectDescriptor
    {
        private static ITeamCityClient _buildQuery;
        private List<BuildDescriptor> _builds;
        private readonly ProjectsProject _teamCityProject;

        public ProjectDescriptor(ProjectsProject teamCityProject, ITeamCityClient buildQuery)
        {
            _teamCityProject = teamCityProject;
            _buildQuery = buildQuery;
        }

        public ProjectDescriptor(ProjectsProject project, List<BuildDescriptor> builds)
        {
            Name = project.name;
            _builds = builds;
        }

        public string Name { get; }

        private BuildState CurrentGlobalState { get; set; }

        private string ColorString => TextLine.GetTextColorString(GetColor());

        private string ReversedColorString => TextLine.GetTextColorString(GetReversedColor());

        public int? PreviouFailedTestCount { get; private set; }

        public BuildState? PreviousGlobalState { get; private set; }

        public int? FailedTestCount => _builds.Where(b => b.ShowTestCount).Sum(b => b.FailedTestCount);

        public int? PassedTestCount => _builds.Where(b => b.ShowTestCount).Sum(b => b.PassedTestCount);

        public void UpdateFromTeamCity()
        {
            var builds = ExtractBuilds(_teamCityProject);
            _builds = builds;
        }

        private static List<BuildDescriptor> ExtractBuilds(ProjectsProject project)
        {
            var buildDescriptors = new List<BuildDescriptor>();
            var expectedBuildConfigurations = project.BuildConfigurations.ToList();

            string projectName = HttpUtility.UrlEncode(project.name).Replace("+", "%20");
            var buildConfigs = _buildQuery.Projects.ByName(projectName).BuildTypes.BuildType;
            foreach (var buildConfig in buildConfigs)
            {
                var buildConfiguration = expectedBuildConfigurations.FirstOrDefault(bc => bc.name == buildConfig.Name);
                if (buildConfiguration == null)
                {
                    continue;
                }

                Build build = _buildQuery.Builds.LastBuildByBuildConfigId(buildConfig.Id);
                if (build != null)
                {
                    var stats =  _buildQuery.Statistics.GetByBuildId(build.Id);
                    buildDescriptors.Add(new BuildDescriptor(buildConfiguration, build, stats));
                }
                else
                {
                    buildDescriptors.Add(new BuildDescriptor(buildConfiguration));
                }
            }
            return buildDescriptors;
        }

        public void ComputeGlobalState()
        {
            if (_builds.Any(bd => bd.State == BuildState.ERROR))
            {
                CurrentGlobalState = BuildState.ERROR;
            }
            else if (_builds.Any(bd => bd.State == BuildState.WARNING))
            {
                CurrentGlobalState = BuildState.WARNING;
            }
            else
            {
                CurrentGlobalState = BuildState.OK;
            }
        }

        public TextLineCollection ComputeXfdLines()
        {
            var xfdLines = new TextLineCollection();
            ComputeGlobalState();
            TextLine.BeepEffect? beepEffect = null;
            if (PreviousGlobalState.HasValue && CurrentGlobalState != PreviousGlobalState.Value)
            {
                switch (CurrentGlobalState)
                {
                    case BuildState.OK:
                        break;
                    case BuildState.WARNING when PreviousGlobalState.Value < BuildState.WARNING:
                        beepEffect = TextLine.BeepEffect.Beep1_0;
                        break;
                    case BuildState.ERROR when PreviousGlobalState.Value < BuildState.ERROR:
                        beepEffect = TextLine.BeepEffect.Beep2_0;
                        break;
                }
            }
            int newFailedTestCount = _builds.Where(bd => bd.ShowTestCount && bd.FailedTestCount.HasValue)
                                            .Sum(buildDescriptor => buildDescriptor.FailedTestCount.Value);

            var showGlobalStatus = true;
            if (PreviouFailedTestCount.HasValue && newFailedTestCount > PreviouFailedTestCount.Value)
            {
                var diffCount = newFailedTestCount - PreviouFailedTestCount;
                var descriptor = CurrentGlobalState == BuildState.ERROR ? TextLineDescriptors.ErrorDescriptor : TextLineDescriptors.WarningDescriptor;
                var textLineDescriptor = TextLineDescriptor.NewFrom(descriptor)
                                                           .WithOpenEffect(TextLine.OpenEffect.ScrollLeft)
                                                           .WithDisplayTime(TextLine.DisplayTime.S2Sec);
                if (!beepEffect.HasValue)
                {
                    beepEffect = TextLine.BeepEffect.BeepO_5;
                }

                var testStr = diffCount > 1 ? "tests" : "test";
                xfdLines.Add(new TextLine(textLineDescriptor, WarningPrefix + $"+{diffCount} {testStr} " + Strings.KOString));
                showGlobalStatus = false;
            }
            PreviouFailedTestCount = newFailedTestCount;
            PreviousGlobalState = CurrentGlobalState;
            AddXfdStatusLines(xfdLines, showGlobalStatus);

            // set the beep effect on the first line
            if (beepEffect.HasValue)
            {
                IEnumerator enumerator = xfdLines.GetEnumerator();
                enumerator.MoveNext();
                var item = enumerator.Current as TextLine;
                item.Descriptor.WithBeepEffect(beepEffect.Value);
            }

            return xfdLines;
        }

        private TextLine.TextColor GetColor()
        {
            switch (CurrentGlobalState)
            {
                case BuildState.ERROR:
                    return TextLine.TextColor.Red;
                case BuildState.WARNING:
                    return TextLine.TextColor.Orange;
                case BuildState.OK:
                    return TextLine.TextColor.Green;
                default:
                    throw new Exception("Invalid state value");
            }
        }

        private TextLine.TextColor GetReversedColor()
        {
            switch (CurrentGlobalState)
            {
                case BuildState.ERROR:
                    return TextLine.TextColor.InverseRed;
                case BuildState.WARNING:
                    return TextLine.TextColor.InverseOrange;
                case BuildState.OK:
                    return TextLine.TextColor.InverseGreen;
                default:
                    throw new Exception("Invalid state value");
            }
        }

        private void AddXfdStatusLines(TextLineCollection c, bool showGlobalStatus)
        {
            if (showGlobalStatus)
            {
                AddXfdGlobalBuildStateLines(c);
            }
            AddXfdBuildsStatesLines(c);
            AddXfdGlobalTestStateLines(c);
            AddXfdTestsStatesLines(c);
        }

        private void AddXfdGlobalBuildStateLines(TextLineCollection c)
        {
            var failedBuilds = _builds.Where(b => b.IsFailed).ToList();
            if (failedBuilds.Any())
            {
                var textLineDescriptor = TextLineDescriptor.NewFrom(TextLineDescriptors.ErrorDescriptor)
                                                           .WithDisplayTime(TextLine.DisplayTime.S2Sec)
                                                           .WithOpenEffect(TextLine.OpenEffect.ScrollLeft);
                var failedBuildCount = failedBuilds.Count;
                var successfulBuildCount = _builds.Count - failedBuildCount;
                c.Add(textLineDescriptor, string.Format(FormatGlobalStateString("{0}{1}" + Strings.OKString + " {2}{3}" + Strings.KOString), GreenStr, successfulBuildCount, RedStr, failedBuildCount));
            }
            else
            {
                var textLineDescriptor = TextLineDescriptor.NewFrom(TextLineDescriptors.SuccessDescriptor)
                                                           .WithDisplayTime(TextLine.DisplayTime.S3Sec)
                                                           .WithOpenEffect(TextLine.OpenEffect.ScrollLeft);
                c.Add(textLineDescriptor, FormatGlobalStateString("All Builds OK"));
            }
        }

        private void AddXfdGlobalTestStateLines(TextLineCollection c)
        {
            var failedTestCount = FailedTestCount;
            var passedTestCount = PassedTestCount;
            if (failedTestCount != null && passedTestCount != null)
            {
                var textLineDescriptor = failedTestCount > 0 && passedTestCount == 0
                                             ? TextLineDescriptors.ErrorDescriptor
                                             : failedTestCount == 0 && passedTestCount > 0
                                                   ? TextLineDescriptors.SuccessDescriptor
                                                   : TextLineDescriptors.WarningDescriptor;
                var tstLineDesc = TextLineDescriptor.NewFrom(textLineDescriptor)
                                                    .WithDisplayTime(TextLine.DisplayTime.S3Sec)
                                                    .WithOpenEffect(TextLine.OpenEffect.ScrollUp);
                c.Add(tstLineDesc, string.Format("{0}{1} {2}" + Strings.OKString + " {3}{4}" + Strings.KOString, GlobalTestPrefix, GreenStr, passedTestCount, RedStr, failedTestCount));
            }
        }

        private void AddXfdBuildsStatesLines(TextLineCollection c)
        {
            AddXfdTextOnBuilds(c, b => b.IsFailed, 2, BuildPrefix, null, b => Strings.KOString);
        }

        private void AddXfdTestsStatesLines(TextLineCollection c)
        {
            AddXfdTextOnBuilds(c, b => b.ShowTestCount && b.FailedTestCount > 0, 1, TestPrefix, b => b.PassedTestCount.ToString(), b => b.FailedTestCount.ToString());
        }

        private void AddXfdTextOnBuilds(TextLineCollection c, Func<BuildDescriptor, bool> filter, int maxPerLine, string prefix, Func<BuildDescriptor, string> okCompString, Func<BuildDescriptor, string> koCompString)
        {
            var failedBuilds = _builds.Where(filter).ToList();
            // show the broken builds
            var buildStr = string.Empty;

            var textLineDescriptor = TextLineDescriptor.NewFrom(TextLineDescriptors.WarningDescriptor)
                                                       .WithDisplayTime(TextLine.DisplayTime.S1Sec)
                                                       .WithOpenEffect(TextLine.OpenEffect.ScrollUp)
                                                       .WithCloseEffect(TextLine.CloseEffect.ScrollUp);
            int indexOnBuilds = 0, indexOnLine = 0;
            foreach (var build in failedBuilds.OrderByDescending(bd => bd.IsCritical))
            {
                indexOnBuilds++;
                indexOnLine++;
                buildStr += build.BuildId + ", ";
                if (indexOnLine >= maxPerLine || indexOnBuilds == failedBuilds.Count)
                {
                    buildStr = buildStr.Remove(buildStr.Length - 2, 2);
                    string str = ReversedColorString + prefix + ColorString + SubstatePrefix + buildStr + ": ";
                    if (okCompString != null)
                    {
                        str += GreenStr + okCompString(build) + " ";
                    }
                    if (koCompString != null)
                    {
                        str += RedStr + koCompString(build);
                    }
                    c.Add(textLineDescriptor, str);
                    buildStr = string.Empty;
                    indexOnLine = 0;
                }
            }
        }


        private static string RedStr => TextLine.GetTextColorString(TextLine.TextColor.Red);

        private static string GreenStr => TextLine.GetTextColorString(TextLine.TextColor.Green);

        private string FormatGlobalStateString(string message)
        {
            return GlobalBuildPrefix + message + StateSuffixString;
        }

        public string GlobalTestPrefix => ReversedColorString + "<AB><U45><U46><AC>";

        public string GlobalBuildPrefix => ReversedColorString + "<AB><U40><U41><U42><AC>" + ColorString + " ";

        public string StateSuffixString => string.Empty;

        public string SubstatePrefix => ColorString + "<AC><U42><AC>" + ColorString + " ";

        public string WarningPrefix => ReversedColorString + "<AA><U48><AC>" + ColorString + " ";

        private static string BuildPrefix => "<AB><U40>";

        private static string TestPrefix => "<AB><U47>";
    }
}