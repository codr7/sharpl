using Sharpl.Libs;
using System.Globalization;
using System.Text;

namespace Sharpl;

public static class Json
{
    public static Value? ReadArray(VM vm, TextReader source, ref Loc loc)
    {
        var c = source.Peek();
        if (c == -1 || c != '[') { return null; }
        loc.Column++;
        source.Read();
        var items = new List<Value>();

        while (true)
        {
            ReadWhitespace(source, ref loc);
            switch (source.Peek())
            {
                case -1: throw new ReadError("Unexpected end of array", loc);

                case ']':
                    {
                        loc.Column++;
                        source.Read();
                        goto EXIT;
                    }

                case ',':
                    {
                        loc.Column++;
                        source.Read();
                        break;
                    }
            }

            if (ReadValue(vm, source, ref loc) is Value it) { items.Add(it); }
            else { throw new ReadError("Unexpected end of array", loc); }
        }

    EXIT:
        return Value.Make(Core.Array, items.ToArray());
    }

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

        return (startLoc.Column == loc.Column) ? null : Value.Make(Core.Fix, Fix.Make(e, value));
    }

    public static Value? ReadId(TextReader source, ref Loc loc)
    {
        var c = source.Peek();
        if (c == -1) { return null; }
        var cc = Convert.ToChar(c);
        if (!char.IsAscii(cc)) { return null; }
        var buffer = new StringBuilder();

        while (true)
        {
            c = source.Peek();
            if (c == -1 || c == ',' || c == '[' || c == ']' || c == '{' || c == '}' || c == '"') { break; }
            cc = Convert.ToChar(c);
            if (!char.IsAscii(cc)) { break; }
            source.Read();
            buffer.Append(cc);
            loc.Column++;
        }

        return buffer.ToString() switch
        {
            "" => null,
            "null" => Value._,
            "true" => Value.T,
            "false" => Value.F,
            var id => throw new ReadError($"Unknown id: {id}", loc)
        };
    }

    public static Value? ReadMap(VM vm, TextReader source, ref Loc loc)
    {
        var c = source.Peek();
        if (c == -1 || c != '{') { return null; }
        loc.Column++;
        source.Read();
        var m = new OrderedMap<Value, Value>();

        while (true)
        {
            ReadWhitespace(source, ref loc);
            switch (source.Peek())
            {
                case -1: throw new ReadError("Unexpected end of map", loc);

                case '}':
                    {
                        loc.Column++;
                        source.Read();
                        goto EXIT;
                    }

                case ',':
                    {
                        loc.Column++;
                        source.Read();
                        break;
                    }
            }

            if (ReadString(source, ref loc) is Value k)
            {
                ReadWhitespace(source, ref loc);
                c = source.Peek();
                if (c != ':') { throw new ReadError($"Invalid map: {c}", loc); }
                loc.Column++;
                source.Read();
                if (ReadValue(vm, source, ref loc) is Value v) { m[Value.Make(Core.Sym, vm.Intern(k.Cast(Core.String)))] = v; }
                else { throw new ReadError("Unexpected end of map", loc); }
            }
            else { throw new ReadError("Unexpected end of map", loc); }
        }

    EXIT:
        return Value.Make(Core.Map, m);
    }
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

        return (startLoc.Column == loc.Column) ? null : Value.Make(Core.Int, v);
    }

    public static Value? ReadString(TextReader source, ref Loc loc)
    {
        var c = source.Peek();
        if (c == -1 || c != '"') { return null; }
        source.Read();
        var sb = new StringBuilder();

        while (true)
        {
            c = source.Peek();
            if (c == -1) { throw new ReadError("Invalid string", loc); }
            source.Read();
            if (c == '"') { break; }

            if (c == '\\')
            {
                loc.Column++;

                c = source.Read() switch
                {
                    'r' => '\r',
                    'n' => '\n',
                    '\\' => '\\',
                    '"' => '"',
                    var v => throw new ReadError($"Invalid escape: {Convert.ToChar(v)}", loc)
                };
            }

            sb.Append(Convert.ToChar(c));
            loc.Column++;
        }

        var s = sb.ToString();
        return (s == "") ? null : Value.Make(Core.String, s);
    }

    public static Value? ReadValue(VM vm, TextReader source, ref Loc loc)
    {
    START:
        switch (source.Peek())
        {
            case -1: return null;
            case '[': return ReadArray(vm, source, ref loc);
            case '{': return ReadMap(vm, source, ref loc);
            case '"': return ReadString(source, ref loc);

            case var c:
                {
                    var cc = Convert.ToChar(c);

                    if (char.IsWhiteSpace(cc))
                    {
                        ReadWhitespace(source, ref loc);
                        goto START;
                    }

                    if (char.IsDigit(cc)) { return ReadNumber(source, ref loc); }
                    if (char.IsAscii(cc)) { return ReadId(source, ref loc); }
                    return null;
                }
        }
    }

    public static void ReadWhitespace(TextReader source, ref Loc loc)
    {
        while (true)
        {
            switch (source.Peek())
            {
                case ' ':
                case '\t':
                    loc.Column++;
                    source.Read();
                    break;
                case '\r':
                case '\n':
                    loc.NewLine();
                    source.Read();
                    break;
                default:
                    return;
            }
        }
    }
}