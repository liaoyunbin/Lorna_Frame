
namespace LornaGame.ModuleExtensions
{
    public class TimeUtils
    {
        public static string FormatTimeSec2Srt(int _sec)
        {
            int hour = _sec / 3600;
            int minute = _sec % 3600 / 60;
            int second = _sec % 60;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        }

        public static float MillisecondsToSeconds(int _sec)
        {
            return (float)_sec * 0.001f;
        }
    }
}
