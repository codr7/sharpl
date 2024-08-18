namespace Sharpl.Readers;

using System.Text;

public struct Id : Reader
{
    public static readonly Id Instance = new Id();

    public static bool Valid(char c) =>
        !(Char.IsWhiteSpace(c) || Char.IsControl(c) ||
            c == '(' || c == ')' ||
            c == '[' || c == ']' ||
            c == '{' || c == '}' ||
            c == '\'' || c == ',' || c == '"' || c == ':' || c == '&');


    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var c = source.Peek();
        if (c == -1) { return false; }
        var cc = Convert.ToChar(c);
        if (!Valid(cc) || char.IsDigit(cc)) { return false; }

        var formLoc = loc;
        var buffer = new StringBuilder();

        while (true)
        {
            c = source.Peek();
            if (c == -1) { break; }
            cc = Convert.ToChar(c);
            if (!Valid(cc) || ((c == '*') && buffer.Length != 0)) { break; }
            source.Read();
            buffer.Append(cc);
            loc.Column++;
            if (c == '^' && buffer.Length == 1) { break; }
        }

        if (buffer.Length == 0) { return false; }
        forms.Push(new Forms.Id(formLoc, buffer.ToString()));
        return true;
    }
}