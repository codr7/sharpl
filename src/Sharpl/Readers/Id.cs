using System.Text;

namespace Sharpl.Readers;

public struct Id : Reader
{
    public static readonly Id Instance = new Id();

    public static bool Valid(char c) =>
        !(char.IsWhiteSpace(c) || char.IsControl(c) ||
            c == '(' || c == ')' ||
            c == '[' || c == ']' ||
            c == '{' || c == '}' ||
            c == '\'' || c == ',' || c == '.' || c == '"' || c == ':' || c == '&' || c == '#');


    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        var c = source.Peek();
        if (c is null) { return false; }
        if (!Valid((char)c) || char.IsDigit((char)c)) { return false; }

        var formLoc = loc;
        var buffer = new StringBuilder();

        while (true)
        {
            c = source.Peek();
            if (c is null) { break; }
            if (!Valid((char)c) || ((c == '*') && buffer.Length != 0)) { break; }
            source.Read();
            buffer.Append(c);
            loc.Column++;
            if (c == '^' && buffer.Length == 1) { break; }
        }

        if (buffer.Length == 0) { return false; }
        var s = buffer.ToString();
        forms.Push(s.Equals("_") ? new Forms.Nil(loc) : new Forms.Id(buffer.ToString(), formLoc));
        return true;
    }
}