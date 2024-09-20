namespace Sharpl.Readers;

public struct Call : Reader
{
    public static readonly Call Instance = new Call();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        var c = source.Peek();
        if (c is null || c != '(') { return false; }

        var formLoc = loc;
        loc.Column++;
        source.Read();
        var args = new Form.Queue();

        while (true)
        {
            WhiteSpace.Instance.Read(source, vm, args, ref loc);
            c = source.Peek();

            if (c is null) { throw new ReadError("Unexpected end of call", loc); }

            if (c == ')')
            {
                loc.Column++;
                source.Read();
                break;
            }

            if (!vm.ReadForm(source, ref loc, args)) { throw new ReadError("Unexpected end of call " + forms, loc); }
        }

        if (args.Empty) { throw new ReadError("Missing call target", loc); }
        var target = args.Pop();
        forms.Push(new Forms.Call((Form)target, args.Items, formLoc));
        return true;
    }
}