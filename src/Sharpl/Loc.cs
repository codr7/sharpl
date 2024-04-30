namespace Sharpl;

public struct Loc {
    public int Column;
    public int Line;
    public readonly string Source;

    public Loc(string source, int line, int column) {
        Source = source;
        Line = line;
        Column = column;
    }

    public override string ToString() {
        return $"{Source}@{Line}:{Column}";
    }
}