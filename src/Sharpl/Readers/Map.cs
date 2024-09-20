namespace Sharpl.Readers;

public struct Map: Reader {
    public static readonly Map Instance = new Map();

    public bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc)
    {
        var c = source.Peek();
        if (c is null || c != '{') { return false; }
        var formLoc = loc;
        loc.Column++;
        source.Read();
        var items = new Form.Queue();

        while (true) {
            WhiteSpace.Instance.Read(source, vm, forms, ref loc);
            c = source.Peek();
            if (c is null) { throw new ReadError("Unexpected end of map", loc); }
            
            if (c == '}') {
                loc.Column++;
                source.Read();
                break;
            }

            if (!vm.ReadForm(source, ref loc, items)) { throw new ReadError("Unexpected end of map", loc); }
        }

        forms.Push(new Forms.Map(items.Items, formLoc));
        return true;
    }
}