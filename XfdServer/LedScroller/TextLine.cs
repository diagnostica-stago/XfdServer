namespace XfdServer.LedScroller
{
    public class TextLineDescriptor
    {
        public TextLine.Font Font { get; set; } = TextLine.Font.None;

        public TextLine.TextColor TextColor { get; set; } = TextLine.TextColor.None;

        public TextLine.OpenEffect OpenEffect { get; set; } = TextLine.OpenEffect.ScrollLeft;

        public TextLine.CloseEffect CloseEffect { get; set; } = TextLine.CloseEffect.ScrollLeft;

        public TextLine.MoveSpeed MoveSpeed { get; set; } = TextLine.MoveSpeed.Normal;

        public TextLine.DisplayTime DisplayTime { get; set; } = TextLine.DisplayTime.S2Sec;

        public TextLine.BeepEffect BeepEffect { get; set; } = TextLine.BeepEffect.None;

        public TextLineDescriptor(TextLine.Font initialFont, TextLine.TextColor initialColor, TextLine.OpenEffect openEffect, TextLine.CloseEffect closeEffect, TextLine.MoveSpeed moveSpeed, TextLine.DisplayTime displayTime, TextLine.BeepEffect beepEffect)
        {
            Font = initialFont;
            TextColor = initialColor;
            OpenEffect = openEffect;
            CloseEffect = closeEffect;
            MoveSpeed = moveSpeed;
            DisplayTime = displayTime;
            BeepEffect = beepEffect;
        }

        public TextLineDescriptor(TextLine.Font initialFont, TextLine.TextColor initialColor, TextLine.OpenEffect openEffect, TextLine.CloseEffect closeEffect, TextLine.MoveSpeed moveSpeed, TextLine.DisplayTime displayTime)
        {
            Font = initialFont;
            TextColor = initialColor;
            OpenEffect = openEffect;
            CloseEffect = closeEffect;
            MoveSpeed = moveSpeed;
            DisplayTime = displayTime;
        }

        public TextLineDescriptor(TextLine.OpenEffect openEffect, TextLine.CloseEffect closeEffect, TextLine.DisplayTime displayTime)
        {
            OpenEffect = openEffect;
            CloseEffect = closeEffect;
            DisplayTime = displayTime;
        }

        public TextLineDescriptor()
        {
        }

        public TextLineDescriptor GetCopy()
        {
            return new TextLineDescriptor(Font, TextColor, OpenEffect, CloseEffect, MoveSpeed, DisplayTime, BeepEffect);
        }

        public TextLineDescriptor WithSpeed(TextLine.MoveSpeed moveSpeed)
        {
            MoveSpeed = moveSpeed;
            return this;
        }

        public TextLineDescriptor WithDisplayTime(TextLine.DisplayTime displayTime)
        {
            DisplayTime = displayTime;
            return this;
        }

        public TextLineDescriptor WithBeepEffect(TextLine.BeepEffect beepEffect)
        {
            BeepEffect = beepEffect;
            return this;
        }

        public TextLineDescriptor WithOpenEffect(TextLine.OpenEffect openEffect)
        {
            OpenEffect = openEffect;
            return this;
        }

        public TextLineDescriptor WithCloseEffect(TextLine.CloseEffect closeEffect)
        {
            CloseEffect = closeEffect;
            return this;
        }

        public static TextLineDescriptor NewFrom(TextLineDescriptor tld)
        {
            return tld.GetCopy();
        }
    }

    public class TextLine : AbstractCommand
    {
        public enum Font
        {
            None,
            Font4x7,
            Font5x7,
            Font6x7
        }

        public enum TextColor
        {
            None,
            Red,
            Green,
            Orange,
            InverseRed,
            InverseGreen,
            InverseOrange,
            RedOnGreen,
            GreenOnRed,
            RedYellowGreen,
            Rainbow
        }

        public enum MoveSpeed
        {
            Slow,
            Medium,
            Normal,
            Fast
        }

        public enum DisplayTime
        {
            S0Point5Sec,
            S1Sec,
            S2Sec,
            S3Sec,
            S4Sec,
            S5Sec,
            S6Sec,
            S7Sec,
            S8Sec,
            S9Sec,
            S10Sec,
            S11Sec,
            S12Sec,
            S13Sec,
            S14Sec,
            S15Sec,
            S16Sec,
            S17Sec,
            S18Sec,
            S19Sec,
            S20Sec,
            S21Sec,
            S22Sec,
            S23Sec,
            S24Sec,
            S25Sec
        }

        public enum OpenEffect
        {
            Immediate,
            Xopen,
            CurtainUp,
            CurtainDown,
            ScrollLeft,
            ScrollRight,
            VOpen,
            VClose,
            ScrollUp,
            ScrollDown,
            Hold,
            Snow,
            Twinkle,
            BlockMove,
            Random
        }

        public enum CloseEffect
        {
            Immediate,
            Xopen,
            CurtainUp,
            CurtainDown,
            ScrollLeft,
            ScrollRight,
            VOpen,
            VClose,
            ScrollUp,
            ScrollDown
        }

        public enum BeepEffect
        {
            None,
            BeepO_5,
            Beep1_0,
            Beep1_5,
            Beep2_0
        }

        static readonly string[] _fontStrings = { "", "<AC>", "<AA>", "<AB>" };
        static readonly string[] _textColorStrings = { "", "<CA>", "<CD>", "<CH>", "<CL>", "<CM>", "<CN>", "<CP>", "<CQ>", "<CR>", "<CS>" };
        static readonly string[] _effectStrings = { "<FA>", "<FB>", "<FC>", "<FD>", "<FE>", "<FF>", "<FG>", "<FH>", "<FI>", "<FJ>", "<FK>", "<FL>", "<FM>", "<FN>", "<FP>" };
        static readonly string[] _displayTimeStrings = { "<WA>", "<WB>", "<WC>", "<WD>", "<WE>", "<WF>", "<WG>", "<WH>", "<WI>", "<WJ>", "<WK>", "<WL>", "<WM>", "<WN>", "<WO>", "<WP>", "<WQ>", "<WR>", "<WS>", "<WT>", "<WU>", "<WV>", "<WW>", "<WX>", "<WY>", "<WZ>" };
        static readonly string[] _moveSpeedStrings = { "<Mq>", "<Ma>", "<MQ>", "<MA>" };
        static readonly string[] _beepStrings = { "", "<BA>", "<BB>", "<BC>", "<BD>"};

        string _text;

        public TextLine(char page, string text)
        {
            Page = page;
            Descriptor = new TextLineDescriptor();
            _text = text;
        }

        public TextLine(string text)
        {
            Descriptor = new TextLineDescriptor();
            _text = text;
        }

        public TextLine(char page, TextLineDescriptor desc, string text)
        {
            Page = page;
            Descriptor = desc;
            _text = text;
        }

        public TextLine(TextLineDescriptor desc, string text)
        {
            Descriptor = desc;
            _text = text;
        }

        public char Page { get; set; }

        public TextLineDescriptor Descriptor { get; }

        public static string GetTextColorString(TextColor color)
        {
            return _textColorStrings[(int)color];
        }

        public static string GetFontString(Font font)
        {
            return _fontStrings[(int)font];
        }

        public override string[] GetCommands()
        {
            string[] commands = new string[1];

            _text = _text.Replace("<Font4x7>", _fontStrings[(int)Font.Font4x7]);
            _text = _text.Replace("<Font5x7>", _fontStrings[(int)Font.Font5x7]);
            _text = _text.Replace("<Font6x7>", _fontStrings[(int)Font.Font6x7]);
            _text = _text.Replace("<Red>", _textColorStrings[(int)TextColor.Red]);
            _text = _text.Replace("<Green>", _textColorStrings[(int)TextColor.Green]);
            _text = _text.Replace("<Orange>", _textColorStrings[(int)TextColor.Orange]);
            _text = _text.Replace("<InverseRed>", _textColorStrings[(int)TextColor.InverseRed]);
            _text = _text.Replace("<InverseGreen>", _textColorStrings[(int)TextColor.InverseGreen]);
            _text = _text.Replace("<InverseOrange>", _textColorStrings[(int)TextColor.InverseOrange]);
            _text = _text.Replace("<RedOnGreen>", _textColorStrings[(int)TextColor.RedOnGreen]);
            _text = _text.Replace("<GreenOnRed>", _textColorStrings[(int)TextColor.GreenOnRed]);
            _text = _text.Replace("<RedYellowGreen>", _textColorStrings[(int)TextColor.RedYellowGreen]);
            _text = _text.Replace("<Rainbow>", _textColorStrings[(int)TextColor.Rainbow]);
            _text = _text.Replace("<Date>", "<KD>");
            _text = _text.Replace("<Time>", "<KT>");
            _text = _text.Replace("<Bell0.5Sec>", _beepStrings[(int)BeepEffect.BeepO_5]);
            _text = _text.Replace("<Bell1Sec>", _beepStrings[(int)BeepEffect.Beep1_0]);
            _text = _text.Replace("<Bell1.5Sec>", _beepStrings[(int)BeepEffect.Beep1_5]);
            _text = _text.Replace("<Bell2Sec>", _beepStrings[(int)BeepEffect.Beep2_0]);

            string textLineBody = "<L1><P" + Page + ">"
                + _effectStrings[(int)Descriptor.OpenEffect]
                + _moveSpeedStrings[(int)Descriptor.MoveSpeed]
                + _displayTimeStrings[(int)Descriptor.DisplayTime]
                + _effectStrings[(int)Descriptor.CloseEffect]
                + _fontStrings[(int)Descriptor.Font]
                + _textColorStrings[(int)Descriptor.TextColor]
                + _beepStrings[(int)Descriptor.BeepEffect]
                + _text;

            commands[0] = CreateCommand(textLineBody);

            return commands;
        }

        public override string ToString()
        {
            return string.Join("", GetCommands());
        }
    }
}