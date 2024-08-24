using System.Text;
using Sharpl.Libs;

namespace Sharpl.Readers;

public struct String : Reader
{
    public static readonly String Instance = new String();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var c = source.Peek();
        if (c == -1 || c != '"') { return false; }
        source.Read();
        var formLoc = loc;
        var s = new StringBuilder();

        while (true)
        {
            c = source.Peek();
            if (c == -1) { throw new ReadError(loc, "Invalid string"); }
            source.Read();
            if (c == '"') { break; }
            s.Append(Convert.ToChar(c));
            loc.Column++;
        }

        forms.Push(new Forms.Literal(formLoc, Value.Make(Core.String, s.ToString())));
        return true;
    }
}