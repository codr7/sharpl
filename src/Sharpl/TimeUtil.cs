using System.Globalization;

namespace Sharpl;

public static class TimeUtil
{
    public static int IsoWeek(this DateTime t)
    {
        // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
        // be the same week# as whatever Thursday, Friday or Saturday are,
        // and we always get those right
        
        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(t);
        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday) { t = t.AddDays(3); }

        // Return the week of our adjusted day
        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(t, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }
}