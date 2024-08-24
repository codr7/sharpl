using System.Globalization;
using Sharpl.Libs;

namespace Sharpl.Readers;

public struct Fix : Reader
{
    public static readonly Fix Instance = new Fix();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms, Loc formLoc, long val)
    {
        var c = source.Peek();
        if (c == -1 || c != '.') { return false; }
        loc.Column++;
        source.Read();
        byte e = 0;

        while (true)
        {
            c = source.Peek();
            if (c == -1) { break; }
            var cc = Convert.ToChar(c);
            if (!char.IsAsciiDigit(cc)) { break; }
            source.Read();
            val = val * 10 + (long)CharUnicodeInfo.GetDecimalDigitValue(cc);
            e++;
            loc.Column++;
        }

        if (formLoc.Column == loc.Column) { return false; }
        forms.Push(new Forms.Literal(formLoc, Value.Make(Core.Fix, Sharpl.Fix.Make(e, val))));
        return true;
    }

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms) =>
        Read(source, vm, ref loc, forms, loc, 0);
}