namespace Sharpl.Readers;

public struct Quote : Reader
{
    public static readonly Quote Instance = new Quote();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        var c = source.Peek();
        if (c is null || c != '\'') { return false; }
        var formLoc = loc;
        loc.Column++;
        source.Read();
        if (vm.ReadForm(source, ref loc, forms) && forms.TryPopLast() is Form f) { forms.Push(new Forms.QuoteForm(formLoc, f)); }
        else { throw new ReadError("Missing quoted value", loc); }
        return true;
    }
}