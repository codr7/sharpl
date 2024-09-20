namespace Sharpl.Readers;

public struct Splat: Reader {
    public static readonly Splat Instance = new Splat();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        var c = source.Peek();
        if (c is null || c != '*' || forms.Count == 0) { return false; }
        var formLoc = loc;
        loc.Column++;
        source.Read();
        var target = forms.TryPopLast();
#pragma warning disable CS8604 
        forms.Push(new Forms.Splat(target, formLoc));
#pragma warning restore CS8604
        return true;
    }
}