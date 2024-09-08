namespace Sharpl.Readers;

public struct Array: Reader {
    public static readonly Array Instance = new Array();

    public bool Read(Source source, VM vm, ref Loc loc, Form.Queue forms) {
        var c = source.Peek();
        if (c == -1 || c != '[') { return false; }
        var formLoc = loc;
        loc.Column++;
        source.Read();
        
        var items = new Form.Queue();

        while (true) {
            WhiteSpace.Instance.Read(source, vm, ref loc, forms);
            c = source.Peek();
            if (c is null) { throw new ReadError(loc, "Unexpected end of array"); }
            
            if (c == ']') {
                loc.Column++;
                source.Read();
                break;
            }

            if (!vm.ReadForm(source, ref loc, items)) { throw new ReadError(loc, "Unexpected end of array"); }
        }

        forms.Push(new Forms.Array(formLoc, items.Items));
        return true;
    }
}