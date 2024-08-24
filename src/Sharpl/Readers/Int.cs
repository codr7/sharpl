using System.Globalization;
using Sharpl.Libs;

namespace Sharpl.Readers;

public struct Int : Reader
{
    public static readonly Int Instance = new Int();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var formLoc = loc;
        var v = 0;

        while (true)
        {
            var c = source.Peek();
            if (c == -1) { break; }
            if (c == '.') { return Fix.Instance.Read(source, vm, ref loc, forms, formLoc, v); }
            var cc = Convert.ToChar(c);
            if (!char.IsAsciiDigit(cc)) { break; }
            source.Read();
            v = v * 10 + CharUnicodeInfo.GetDecimalDigitValue(cc);
            loc.Column++;
        }

        if (formLoc.Column == loc.Column) { return false; }
        forms.Push(new Forms.Literal(formLoc, Value.Make(Core.Int, v)));
        return true;
    }
}