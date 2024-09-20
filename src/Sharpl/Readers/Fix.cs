using System.Globalization;
using Sharpl.Libs;

namespace Sharpl.Readers;

public struct Fix : Reader
{
    public static readonly Fix Instance = new Fix();

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms, Loc formLoc, long val)
    {
        var c = source.Peek();
        if (c is null || c != '.') { return false; }
        source.Read();
        c = source.Peek();
        
        if (c == '.') {
            source.Unread('.');
            return false;
        }

        loc.Column++;
        byte e = 0;

        while (true)
        {
            c = source.Peek();
            if (c is null) { break; }
            if (!char.IsAsciiDigit((char)c)) { break; }
            source.Read();
            val = val * 10 + (long)CharUnicodeInfo.GetDecimalDigitValue((char)c);
            e++;
            loc.Column++;
        }

        if (formLoc.Column == loc.Column) { return false; }
        forms.Push(new Forms.Literal(Value.Make(Core.Fix, Sharpl.Fix.Make(e, val)), formLoc));
        return true;
    }

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc) =>
        Read(source, vm, ref loc, forms, loc, 0);
}