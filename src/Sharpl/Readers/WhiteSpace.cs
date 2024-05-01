namespace Sharpl.Readers;

using System.Text;

public struct WhiteSpace : Reader
{
    public static readonly Id Instance = new Id();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var done = false;

        while (!done)
        {
            var c = source.Peek();

            if (c == -1)
            {
                break;
            }

            source.Read();

            switch (Convert.ToChar(c))
            {
                case ' ':
                case '\t':  
                    loc.Column++;
                    break;
                case '\n':
                    loc.NewLine();
                    break;
                default:
                    done = true;
                    break;
            }
        }

        return false;
    }
}