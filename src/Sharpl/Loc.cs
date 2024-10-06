namespace Sharpl;

public struct Loc: IComparable<Loc>
{
    public int Column;
    public int Line;
    public readonly string Source;

    public Loc(string source, int line = 1, int column = 1)
    {
        Source = source;
        Line = line;
        Column = column;
    }

    public void NewLine()
    {
        Line++;
        Column = 1;
    }

    public override string ToString() =>
        $"{Source}@{Line}:{Column}";

    public int CompareTo(Loc other)
    {
        var s = Source.CompareTo(other.Source);
        if (s != 0) return s;
        var l = Line.CompareTo(other.Line);
        if (l != 0) return l;
        return Column.CompareTo(other.Column);
    }
}