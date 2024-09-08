using System.Globalization;
using Sharpl.Libs;

namespace Sharpl.Readers;

public struct Int : Reader
{
    public static readonly Int Instance = new Int();

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var formLoc = loc;
        var v = 0;

        while (true)
        {
            var c = source.Peek();
            if (c is null) { break; }
            
            if (c == '.') { 
                source.Read();
                c = source.Peek();
                source.Unread('.');
                if (c == '.') { break; } 
                return Fix.Instance.Read(source, vm, ref loc, forms, formLoc, v); 
            }

            if (!char.IsAsciiDigit((char)c)) { break; }
            source.Read();
            v = v * 10 + CharUnicodeInfo.GetDecimalDigitValue((char)c);
            loc.Column++;
        }

        if (formLoc.Column == loc.Column) { return false; }
        forms.Push(new Forms.Literal(formLoc, Value.Make(Core.Int, v)));
        return true;
    }
}