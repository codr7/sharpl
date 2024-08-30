using System.Globalization;
using Sharpl.Libs;

namespace Sharpl;

public static class Json
{
    public static Value? ReadArray(TextReader source, ref Loc loc) => Value.Nil;

    public static Value? ReadDecimal(TextReader source, ref Loc loc, int value)
    {
        var startLoc = loc;
        var c = source.Peek();
        if (c == -1 || c != '.') { return null; }
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
            value = value * 10 + (int)CharUnicodeInfo.GetDecimalDigitValue(cc);
            e++;
            loc.Column++;
        }

        if (startLoc.Column == loc.Column) { return null; }
        return Value.Make(Core.Fix, Fix.Make(e, value));
    }

    public static Value? ReadId(TextReader source, ref Loc loc) => Value.Nil;
    public static Value? ReadMap(TextReader source, ref Loc loc) => Value.Nil;
    public static Value? ReadNumber(TextReader source, ref Loc loc)
    {
        var v = 0;
        var startLoc = loc;

        while (true)
        {
            var c = source.Peek();
            if (c == -1) { break; }
            if (c == '.') { return ReadDecimal(source, ref loc, v); }
            var cc = Convert.ToChar(c);
            if (!char.IsAsciiDigit(cc)) { break; }
            source.Read();
            v = v * 10 + CharUnicodeInfo.GetDecimalDigitValue(cc);
            loc.Column++;
        }

        if (startLoc.Column == loc.Column) { return null; }
        return Value.Make(Core.Int, v);
    }

    public static Value? ReadString(TextReader source, ref Loc loc) => Value.Nil;

    public static Value? ReadValue(TextReader source, ref Loc loc)
    {
        switch (source.Peek())
        {
            case -1: return null;
            case '[': return ReadArray(source, ref loc);
            case '{': return ReadMap(source, ref loc);
            case '"': return ReadString(source, ref loc);

            case var c:
                {
                    var cc = Convert.ToChar(c);
                    if (char.IsWhiteSpace(cc)) { return ReadWhitespace(source, ref loc); }
                    if (char.IsDigit(cc)) { return ReadNumber(source, ref loc); }
                    if (char.IsAscii(cc)) { return ReadId(source, ref loc); }
                    return null;
                }
        }
    }

    public static Value? ReadWhitespace(TextReader source, ref Loc loc) => Value.Nil;
}