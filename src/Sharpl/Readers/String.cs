using System.Text;
using Sharpl.Libs;

namespace Sharpl.Readers;

public struct String : Reader
{
    public static readonly String Instance = new String();


    public static char? GetEscape(char? c) => c switch
    {
        'r' => '\r',
        'n' => '\n',
        '\\' => '\\',
        '"' => '"',
        _ => null
    };

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var c = source.Peek();
        if (c is null || c != '"') { return false; }
        source.Read();
        var formLoc = loc;
        var s = new StringBuilder();

        while (true)
        {
            c = source.Peek();
            if (c is null) { throw new ReadError(loc, "Invalid string"); }
            source.Read();
            loc.Column++;
            if (c == '"') { break; }

            if (c == '\\')
            {
                c = source.Peek();

                if (GetEscape(source.Peek()) is char ec)
                {
                    source.Read();
                    c = ec;
                }
                else { s.Append('\\'); }

                source.Read();
            }

            s.Append(c);
            loc.Column++;
        }

        forms.Push(new Forms.Literal(formLoc, Value.Make(Core.String, s.ToString())));
        return true;
    }
}