namespace XfdServer.LedScroller
{
    public class BeepCommand : TextLine
    {
        public BeepCommand(BeepEffect beepEffect)
            : base(new TextLineDescriptor(Font.None, TextColor.None, OpenEffect.Immediate, CloseEffect.Immediate, MoveSpeed.Slow, GetDisplayTime(beepEffect), beepEffect), string.Empty)
        {
        }

        private static DisplayTime GetDisplayTime(BeepEffect beepEffect)
        {
            switch (beepEffect)
            {
                case BeepEffect.BeepO_5:
                    return DisplayTime.S0Point5Sec;
                case BeepEffect.Beep1_0:
                    return DisplayTime.S1Sec;
                case BeepEffect.Beep1_5:
                    return DisplayTime.S2Sec;
                case BeepEffect.Beep2_0:
                    return DisplayTime.S2Sec;
                default:
                    return DisplayTime.S0Point5Sec;
            }
        }
    }
}