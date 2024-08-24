namespace Sharpl.Readers;

public struct OneOf(Reader[] Parts) : Reader
{
    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms)
    {
        foreach (var r in Parts)
        {
            if (r.Read(source, vm, ref loc, forms)) { return true; }
        }

        return false;
    }
}