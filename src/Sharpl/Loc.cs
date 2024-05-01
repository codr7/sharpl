namespace Sharpl;

public struct Loc {
    public int Column;
    public int Line;
    public readonly string Source;

    public Loc(string source, int line = 1, int column = 1) {
        Source = source;
        Line = line;
        Column = column;
    }

    public void NewLine() {
        Line++;
        Column = 1;
    }

    public override string ToString() {
        return $"{Source}@{Line}:{Column}";
    }
}