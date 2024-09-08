namespace Sharpl.Readers;

public struct Quote : Reader
{
    public static readonly Quote Instance = new Quote();

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var c = source.Peek();
        if (c is null || c != '\'') { return false; }
        var formLoc = loc;
        loc.Column++;
        source.Read();
        if (vm.ReadForm(source, ref loc, forms) && forms.TryPopLast() is Form f) { forms.Push(new Forms.QuoteForm(formLoc, f)); }
        else { throw new ReadError(loc, "Missing quoted value"); }
        return true;
    }
}