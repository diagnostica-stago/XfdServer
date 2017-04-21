namespace XfdServer.LedScroller
{
    public class Brightness : AbstractCommand
    {
        public enum BrightnessEnum
        {
            Percent100,
            Percent75,
            Percent50,
            Percent25
        }

        static readonly string[] _brightnessStrings = { "<BA>", "<BB>", "<BC>", "<BD>" };
        readonly BrightnessEnum _brightness;

        public Brightness(BrightnessEnum brightness)
        {
            _brightness = brightness;
        }

        public override string ToString()
        {
            return CreateCommand(_brightnessStrings[(int)_brightness]);
        }
    }
}
