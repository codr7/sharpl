using Sharpl.Libs;

namespace Sharpl.Readers;

public struct Char : Reader
{
    public static readonly Char Instance = new Char();

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var c = source.Peek();
        if (c is null || c != '\\') { return false; }
        var formLoc = loc;
        source.Read();
        loc.Column++;

        c = source.Read();
        if (c is null) { throw new ReadError(loc, "Invalid char literal"); }
        
        if (c == '\\') {
            c = source.Read() switch {
                'n' => '\n',
                'r' => '\r',
                's' => ' ',
                var e =>  throw new ReadError(loc, $"Invalid special char literal: {e}")
            };
        }

        forms.Push(new Forms.Literal(formLoc, Value.Make(Core.Char, (char)c)));
        return true;
    }
}