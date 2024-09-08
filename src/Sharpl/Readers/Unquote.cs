namespace Sharpl.Readers;

public struct Unquote : Reader
{
    public static readonly Unquote Instance = new Unquote();

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var c = source.Peek();
        if (c is null || c != ',') { return false; }
        var formLoc = loc;
        loc.Column++;
        source.Read();

        if (!vm.ReadForm(source, ref loc, forms)) { throw new ReadError(loc, "Missing unquoted form"); }

        WhiteSpace.Instance.Read(source, vm, ref loc, forms);

        if (source.Peek() == '*')
        {
            if (!Splat.Instance.Read(source, vm, ref loc, forms)) { throw new ReadError(loc, "Failed reading unquoted splat"); }
        }

        forms.Push(new Forms.UnquoteForm(formLoc, forms.PopLast()));
        return true;
    }
}