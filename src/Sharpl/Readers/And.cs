namespace Sharpl.Readers;

public struct And: Reader {
    public static readonly And Instance = new And();

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms) {        
        if (forms.Count == 0) { return false; }
        var c = source.Peek();
        if (c is null || c != '&') { return false; }
        var left = forms.PopLast();        
        var formLoc = left.Loc;
        loc.Column++;
        source.Read();
        if (!vm.ReadForm(source, ref loc, forms)) { throw new ReadError(loc, "Missing right value"); }
        WhiteSpace.Instance.Read(source, vm, ref loc, forms);
        if (source.Peek() == '&' && !Read(source, vm, ref loc, forms)) { throw new ReadError(loc, "Failed reading nested and form"); }
        var right = forms.PopLast();
        forms.Push(new Forms.And(formLoc, left, right));
        return true;
    }
}