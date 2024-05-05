namespace Sharpl.Readers;

using System.Text;

public struct WhiteSpace : Reader
{
    public static readonly WhiteSpace Instance = new WhiteSpace();

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
            
            switch (c)
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
                    done = true;
                    break;
            }
        }

        return false;
    }
}