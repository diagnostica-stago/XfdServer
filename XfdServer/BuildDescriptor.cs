using System;
using System.Collections.Generic;
using System.Linq;
using TeamCitySharp.DomainEntities;
using XfdServer.LedScroller;

namespace XfdServer
{
    public enum BuildState
    {
        OK,
        WARNING,
        ERROR
    }

    public class BuildDescriptor
    {
        public BuildDescriptor(ProjectsProjectBuildConfiguration buildConfig, Build buildResult, List<Property> properties)
        {
            Name = buildConfig.name;
            IsCritical = buildConfig.criticity == criticityLevel.high;
            IsFailed = !Equals(buildResult.Status, "SUCCESS");
            var testFailedProp = properties.SingleOrDefault(p => p.Name == "FailedTestCount");
            if (testFailedProp != null)
            {
                FailedTestCount = int.Parse(testFailedProp.Value);
            }
            var passedTestCount = properties.SingleOrDefault(p => p.Name == "PassedTestCount");
            if (passedTestCount != null)
            {
                PassedTestCount = int.Parse(passedTestCount.Value);
            }
            ShowTestCount = buildConfig.showTestCount;
        }

        public BuildDescriptor(ProjectsProjectBuildConfiguration buildConfig)
        {
            Name = buildConfig.name;
            IsCritical = false;
            IsFailed = false;
            FailedTestCount = 0;
            ShowTestCount = false;
        }

        public string Name { get; }
        public bool IsCritical { get; }
        public bool IsFailed { get; }
        public int? FailedTestCount { get; }
        public int? PassedTestCount { get; private set; }
        public bool ShowTestCount { get; }

        public string BuildId => Name.Split('-')[0].Split(' ')[1];

        public string ShortName => Name.Split('-')[1];

        public TextLine TextLine => new TextLine(Descriptor, Description);

        public TextLine GetPrefixedTextLine(string prefix)
        {
            return new TextLine(Descriptor, prefix + StatusColor + Description);
        }

        public TextLineDescriptor Descriptor => (IsCritical ? TextLineDescriptors.ErrorDescriptor : TextLineDescriptors.WarningDescriptor).WithDisplayTime(TextLine.DisplayTime.S1Sec);

        public string StatusColor
        {
            get
            {
                switch (State)
                {
                    case BuildState.ERROR:
                        return TextLine.GetTextColorString(TextLine.TextColor.Red);
                    case BuildState.WARNING:
                        return TextLine.GetTextColorString(TextLine.TextColor.Orange);
                    case BuildState.OK:
                    default:
                        return TextLine.GetTextColorString(TextLine.TextColor.Green);
                }
            }
        }

        public string Description
        {
            get
            {
                if (FailedTestCount != null && ShowTestCount)
                {
                    return $"{BuildId}: {FailedTestCount} tst" + Strings.KOString;
                }
                if (IsFailed)
                {
                    return $"{BuildId}: build" + Strings.KOString;
                }
                return $"{ShortName} succesful";
            }
        }

        public bool ShowAsBrokenBuild => IsFailed && (!FailedTestCount.HasValue || !ShowTestCount);

        public BuildState State
        {
            get
            {
                if (IsCritical && IsFailed)
                {
                    return BuildState.ERROR;
                }
                if (IsFailed)
                {
                    return BuildState.WARNING;
                }
                return BuildState.OK;
            }
        }
    }
}