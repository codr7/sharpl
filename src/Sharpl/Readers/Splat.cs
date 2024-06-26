namespace Sharpl.Readers;

public struct Splat: Reader {
    public static readonly Splat Instance = new Splat();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms) {
        var c = source.Peek();

        if (c == -1 || c != '*' || forms.Count == 0) {
            return false;
        }

        var formLoc = loc;
        loc.Column++;
        source.Read();
        var target = forms.PopLast();

#pragma warning disable CS8604 // Possible null reference argument.
        forms.Push(new Forms.Splat(formLoc, target));
#pragma warning restore CS8604 // Possible null reference argument.
        return true;
    }
}