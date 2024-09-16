namespace Sharpl;

public readonly record struct Duration(int Months, TimeSpan Time) : IComparable<Duration>
{
    public DateTime AddTo(DateTime it) => it.AddMonths(Months).Add(Time);
    public Duration Add(Duration it) => new Duration(Months + it.Months, Time.Add(it.Time));

    public Duration Divide(int d) => new Duration(Months / d, Time.Divide(d));
    public int CompareTo(Duration other)
    {
        var r = Months.CompareTo(other.Months);
        if (r != 0) { return r; }
        return Time.CompareTo(other.Time);
    }

    public int Days => Time.Days;
    public int Hours => Time.Hours;
    public int Microseconds => Time.Microseconds;
    public int Milliseconds => Time.Milliseconds;
    public int Minutes => Time.Minutes;
    public Duration Multiply(int m) => new Duration(Months / m, Time.Multiply(m));
    public int Seconds => Time.Seconds;
    public DateTime SubtractFrom(DateTime it) => it.AddMonths(-Months).Subtract(Time);
    public Duration Subtract(Duration it) => new Duration(Months - it.Months, Time.Subtract(it.Time));
    public override string ToString() => $"(duration {Months} {Time})";
};