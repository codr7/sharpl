namespace Sharpl.Readers;

using System.Text;

public struct Id : Reader
{
    public static readonly Id Instance = new Id();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var formLoc = loc;
        var buffer = new StringBuilder();

        while (true)
        {
            var c = source.Peek();

            if (c == -1)
            {
                break;
            }

            var cc = Convert.ToChar(c);

            if (Char.IsWhiteSpace(cc) || Char.IsControl(cc) ||
                c == '(' || c == ')' ||
                c == '[' || c == ']' ||
                c == '{' || c == '}' ||
                c == '"' || c == '\'' || c == ':' ||
                (c == '*' && buffer.Length != 0))
            {
                break;
            }

            source.Read();
            buffer.Append(cc);
            loc.Column++;

            if (cc == '^' && buffer.Length == 1)
            {
                break;
            }
        }

        if (buffer.Length == 0)
        {
            return false;
        }

        forms.Push(new Forms.Id(formLoc, buffer.ToString()));
        return true;
    }
}