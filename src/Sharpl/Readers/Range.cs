namespace Sharpl.Readers;

public struct Range : Reader
{
    public static readonly Range Instance = new Range();

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms)
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
        if (!vm.ReadForm(source, ref loc, forms)) { throw new ReadError(loc, "Missing right value"); }
        var right = forms.PopLast();
        forms.Push(new Forms.Call(formLoc, new Forms.Id(loc, "range"), [left, right]));
        return true;
    }
}