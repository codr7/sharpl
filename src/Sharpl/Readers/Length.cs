namespace Sharpl.Readers;

public struct Length : Reader
{
    public static readonly Length Instance = new Length();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var c = source.Peek();
        if (c == -1 || c != '#') { return false; }
        var formLoc = loc;
        loc.Column++;
        source.Read();

        if (vm.ReadForm(source, ref loc, forms) && forms.TryPopLast() is Form f) {
            forms.Push(new Forms.Call(formLoc, new Forms.Id(formLoc, "length"), [f])); 
        }
        else { throw new ReadError(loc, "Missing length value"); }
        
        return true;
    }
}