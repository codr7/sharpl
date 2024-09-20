namespace Sharpl.Readers;

public struct Unquote : Reader
{
    public static readonly Unquote Instance = new Unquote();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        var c = source.Peek();
        if (c is null || c != ',') { return false; }
        var formLoc = loc;
        loc.Column++;
        source.Read();

        if (!vm.ReadForm(source, ref loc, forms)) { throw new ReadError("Missing unquoted form", loc); }

        WhiteSpace.Instance.Read(source, vm, forms, ref loc);

        if (source.Peek() == '*')
        {
            if (!Splat.Instance.Read(source, vm, forms, ref loc)) { throw new ReadError("Failed reading unquoted splat", loc); }
        }

        forms.Push(new Forms.UnquoteForm(formLoc, forms.PopLast()));
        return true;
    }
}