namespace Sharpl.Readers;

public struct Length : Reader
{
    public static readonly Length Instance = new Length();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        var c = source.Peek();
        if (c is null || c != '#') { return false; }
        var formLoc = loc;
        loc.Column++;
        source.Read();

        if (vm.ReadForm(source, ref loc, forms) && forms.TryPopLast() is Form f)
        {
            forms.Push(new Forms.Call(new Forms.Id("length", formLoc), [f], formLoc));
        }
        else { throw new ReadError("Missing length value", loc); }

        return true;
    }
}