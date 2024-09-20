namespace Sharpl.Readers;

public struct Range : Reader
{
    public static readonly Range Instance = new Range();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        if (forms.Empty) { return false; }
        var c = source.Peek();
        if (c is null || c != '.') { return false; }
        source.Read();
        c = source.Peek();

        if (c != '.')
        {
            source.Unread('.');
            return false;
        }

        source.Read();
        var formLoc = loc;
        loc.Column += 2;
        var left = forms.PopLast();
        if (!vm.ReadForm(source, ref loc, forms)) { throw new ReadError("Missing max", loc); }
        var right = forms.PopLast();
        WhiteSpace.Instance.Read(source, vm, forms, ref loc);
        c = source.Peek();
        Form? stride = null;

        if (c == ':')
        {
            source.Read();
            loc.Column++;
            if (!vm.ReadForm(source, ref loc, forms)) { throw new ReadError("Missing stride", loc); }
            stride = forms.PopLast();
        }

        forms.Push(new Forms.Call(formLoc, new Forms.Id(loc, "range"), [left, right, stride ?? new Forms.Nil(loc)]));
        return true;
    }
}