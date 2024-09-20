namespace Sharpl.Readers;

public struct Array: Reader {
    public static readonly Array Instance = new Array();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        var c = source.Peek();
        if (c == -1 || c != '[') { return false; }
        var formLoc = loc;
        loc.Column++;
        source.Read();
        
        var items = new Form.Queue();

        while (true) {
            WhiteSpace.Instance.Read(source, vm, forms, ref loc);
            c = source.Peek();
            if (c is null) { throw new ReadError("Unexpected end of array", loc); }
            
            if (c == ']') {
                loc.Column++;
                source.Read();
                break;
            }

            if (!vm.ReadForm(source, ref loc, items)) { throw new ReadError("Unexpected end of array", loc); }
        }

        forms.Push(new Forms.Array(formLoc, items.Items));
        return true;
    }
}