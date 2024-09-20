namespace Sharpl.Readers;

public struct And : Reader
{
    public static readonly And Instance = new And();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        if (forms.Count == 0) { return false; }
        var c = source.Peek();
        if (c is null || c != '&') { return false; }
        var left = forms.PopLast();
        var formLoc = left.Loc;
        loc.Column++;
        source.Read();
        if (!vm.ReadForm(source, ref loc, forms)) { throw new ReadError("Missing right value", loc); }
        WhiteSpace.Instance.Read(source, vm, forms, ref loc);
        if (source.Peek() == '&' && !Read(source, vm, forms, ref loc)) { throw new ReadError("Failed reading nested and form", loc); }
        var right = forms.PopLast();
        forms.Push(new Forms.And(left, right, formLoc));
        return true;
    }
}