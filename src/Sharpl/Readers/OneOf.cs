namespace Sharpl.Readers;

public struct OneOf(Reader[] Parts) : Reader
{
    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        foreach (var r in Parts)
        {
            if (r.Read(source, vm, forms, ref loc)) { return true; }
        }

        return false;
    }
}