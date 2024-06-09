namespace Sharpl.Readers;

using System.Text;

public struct Call: Reader {
    public static readonly Call Instance = new Call();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms) {
        var c = source.Peek();

        if (c == -1 || c != '(') {
            return false;
        }

        var formLoc = loc;
        loc.Column++;
        source.Read();

        if (!vm.ReadForm(source, ref loc, forms)) {
            throw new ReadError(loc, "Missing call target");
        }

        var target = forms.PopLast();

        if (target is null) {
            throw new ReadError(loc, "Missing call target");
        }
        
        var args = new Form.Queue();

        while (true) {
            WhiteSpace.Instance.Read(source, vm, ref loc, forms);
            c = source.Peek();

            if (c == -1) {
                throw new ReadError(loc, "Unexpected end of call form");
            }
            
            if (c == ')') {
                loc.Column++;
                source.Read();
                break;
            }

            if (!vm.ReadForm(source, ref loc, args)) {
                throw new ReadError(loc, "Unexpected end of call form");
            }
        }

        forms.Push(new Forms.Call(formLoc, (Form)target, args.Items));
        return true;
    }
}