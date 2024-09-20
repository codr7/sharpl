using Sharpl.Libs;

namespace Sharpl.Readers;

public struct Char : Reader
{
    public static readonly Char Instance = new Char();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        var c = source.Peek();
        if (c is null || c != '\\') { return false; }
        var formLoc = loc;
        source.Read();
        loc.Column++;

        c = source.Read();
        if (c is null) { throw new ReadError("Invalid char literal", loc); }
        
        if (c == '\\') {
            c = source.Read() switch {
                'n' => '\n',
                'r' => '\r',
                's' => ' ',
                var e =>  throw new ReadError($"Invalid special char literal: {e}", loc)
            };
        }

        forms.Push(new Forms.Literal(formLoc, Value.Make(Core.Char, (char)c)));
        return true;
    }
}