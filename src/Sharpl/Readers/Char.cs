using Sharpl.Libs;

namespace Sharpl.Readers;

public struct Char : Reader
{
    public static readonly Char Instance = new Char();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var c = source.Peek();
        if (c == -1 || c != '\\') { return false; }
        var formLoc = loc;
        source.Read();
        loc.Column++;

        c = source.Read();
        if (c == -1) { throw new ReadError(loc, "Invalid char literal"); }
        
        if (c == '\\') {
            c = source.Read() switch {
                'n' => '\n',
                'r' => '\r',
                var e =>  throw new ReadError(loc, $"Invalid special char literal: {e}")
            };
        }

        var cc = Convert.ToChar(c);
        forms.Push(new Forms.Literal(formLoc, Value.Make(Core.Char, cc)));
        return true;
    }
}