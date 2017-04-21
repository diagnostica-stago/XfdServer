using XfdServer.LedScroller;

namespace XfdServer
{
    public static class TextLineDescriptors
    {
        public const Brightness.BrightnessEnum BRIGHTNESS = Brightness.BrightnessEnum.Percent50;

        private const TextLine.MoveSpeed SPEED = TextLine.MoveSpeed.Fast;
        private const TextLine.OpenEffect OPEN = TextLine.OpenEffect.ScrollUp;
        private const TextLine.CloseEffect CLOSE = TextLine.CloseEffect.ScrollUp;
        private const TextLine.DisplayTime DISPLAY = TextLine.DisplayTime.S3Sec;
        private const TextLine.Font FONT = TextLine.Font.Font4x7;

        public static TextLineDescriptor StandUp = new TextLineDescriptor(
            TextLine.Font.Font5x7,
            TextLine.TextColor.RedOnGreen,
            TextLine.OpenEffect.Xopen,
            TextLine.CloseEffect.Xopen,
            SPEED,
            TextLine.DisplayTime.S5Sec,
            TextLine.BeepEffect.Beep2_0);

        public static TextLineDescriptor WarnedDescriptor = new TextLineDescriptor(
            FONT,
            TextLine.TextColor.Red,
            OPEN,
            CLOSE,
            SPEED,
            DISPLAY);

        public static TextLineDescriptor SuccessDescriptor = new TextLineDescriptor(
            FONT,
            TextLine.TextColor.Green,
            OPEN,
            CLOSE,
            SPEED,
            DISPLAY);

        public static TextLineDescriptor Xfd = new TextLineDescriptor(
            TextLine.Font.Font4x7,
            TextLine.TextColor.Rainbow,
            TextLine.OpenEffect.Snow,
            TextLine.CloseEffect.CurtainDown,
            TextLine.MoveSpeed.Medium,
            TextLine.DisplayTime.S2Sec);

        public static TextLineDescriptor Time = new TextLineDescriptor(
           TextLine.Font.Font6x7,
           TextLine.TextColor.Orange,
           TextLine.OpenEffect.Snow,
           TextLine.CloseEffect.CurtainDown,
           TextLine.MoveSpeed.Slow,
           TextLine.DisplayTime.S8Sec);

        public static TextLineDescriptor Motd = new TextLineDescriptor(
            TextLine.Font.None,
            TextLine.TextColor.RedYellowGreen,
            TextLine.OpenEffect.ScrollLeft,
            TextLine.CloseEffect.ScrollLeft,
            TextLine.MoveSpeed.Fast,
            TextLine.DisplayTime.S0Point5Sec);

        public static TextLineDescriptor ErrorDescriptor = new TextLineDescriptor(
            FONT,
            TextLine.TextColor.Red,
            OPEN,
            CLOSE,
            SPEED,
            DISPLAY);

        public static TextLineDescriptor WarningDescriptor = new TextLineDescriptor(
            FONT,
            TextLine.TextColor.Orange,
            OPEN,
            CLOSE,
            SPEED,
            DISPLAY);
    }
}