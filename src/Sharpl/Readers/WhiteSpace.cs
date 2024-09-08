namespace Sharpl.Readers;

public struct WhiteSpace : Reader
{
    public static readonly WhiteSpace Instance = new WhiteSpace();

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var done = false;
 
        while (!done)
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
                    done = true;
                    break;
            }
        }

        return false;
    }
}