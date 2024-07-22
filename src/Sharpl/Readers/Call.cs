namespace Sharpl.Readers;

public struct Call : Reader
{
    public static readonly Call Instance = new Call();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms)
    {
        var c = source.Peek();

        if (c == -1 || c != '(')
        {
            return false;
        }

        var formLoc = loc;
        loc.Column++;
        source.Read();
        var args = new Form.Queue();

        while (true)
        {
            WhiteSpace.Instance.Read(source, vm, ref loc, args);
            c = source.Peek();

            if (c == -1)
            {
                throw new ReadError(loc, "Unexpected end of call");
            }

            if (c == ')')
            {
                loc.Column++;
                source.Read();
                break;
            }

            if (!vm.ReadForm(source, ref loc, args))
            {
                throw new ReadError(loc, "Unexpected end of call " + forms);
            }
        }

        if (args.Empty)
        {
            throw new ReadError(loc, "Missing call target");
        }

        var target = args.Pop();
        forms.Push(new Forms.Call(formLoc, (Form)target, args.Items));
        return true;
    }
}