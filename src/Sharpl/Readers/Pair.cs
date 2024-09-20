namespace Sharpl.Readers;

public struct Pair : Reader
{
    public static readonly Pair Instance = new Pair();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        if (forms.Count == 0) { return false; }
        var c = source.Peek();
        if (c is null || c != ':') { return false; }
        var left = forms.TryPopLast();
        if (left is null) { throw new EmitError("Missing left value", loc); }
        var formLoc = left.Loc;
        loc.Column++;
        source.Read();
        if (!vm.ReadForm(source, ref loc, forms)) { throw new ReadError("Missing right value", loc); }
        WhiteSpace.Instance.Read(source, vm, forms, ref loc);
        if (source.Peek() == ':' && !vm.ReadForm(source, ref loc, forms)) { throw new ReadError("Failed reading nested pair", loc); }
        var right = forms.TryPopLast();
        if (right is null) { throw new ReadError("Missing right value", loc); }
        forms.Push(new Forms.Pair(left, right, formLoc));
        return true;
    }
}