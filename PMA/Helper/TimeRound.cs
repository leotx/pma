using System;

namespace PMA.Helper
{
    public static class TimeRound
    {
        private static DateTime RoundUp(this TimeSpan dt, TimeSpan d)
        {
            var delta = (d.Ticks - (dt.Ticks%d.Ticks))%d.Ticks;
            return new DateTime(dt.Ticks + delta);
        }

        private static DateTime RoundDown(this TimeSpan dt, TimeSpan d)
        {
            var delta = dt.Ticks%d.Ticks;
            return new DateTime(dt.Ticks - delta);
        }

        public static DateTime RoundToNearest(this TimeSpan dt, int time)
        {
            var timeSpan = TimeSpan.FromMinutes(time);
            var delta = dt.Ticks % timeSpan.Ticks;
            var roundUp = delta > timeSpan.Ticks / 2;

            return roundUp ? dt.RoundUp(timeSpan) : dt.RoundDown(timeSpan);
        }
    }
}