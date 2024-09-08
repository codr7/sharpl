namespace Sharpl.Readers;

public struct Pair: Reader {
    public static readonly Pair Instance = new Pair();

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms) {
        if (forms.Count == 0) { return false; }
        var c = source.Peek();
        if (c is null || c != ':') { return false; }
        var left = forms.TryPopLast();
        if (left is null) { throw new EmitError(loc, "Missing left value"); }
        var formLoc = left.Loc;
        loc.Column++;
        source.Read();
        if (!vm.ReadForm(source, ref loc, forms)) { throw new ReadError(loc, "Missing right value"); }
        WhiteSpace.Instance.Read(source, vm, ref loc, forms);
        if (source.Peek() == ':' && !vm.ReadForm(source, ref loc, forms)) { throw new ReadError(loc, "Failed reading nested pair"); }
        var right = forms.TryPopLast();
        if (right is null) { throw new ReadError(loc, "Missing right value"); }
        forms.Push(new Forms.Pair(formLoc, left, right));
        return true;
    }
}