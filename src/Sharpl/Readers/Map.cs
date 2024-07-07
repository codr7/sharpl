using Microsoft.VisualBasic;

namespace Sharpl.Readers;

public struct Map: Reader {
    public static readonly Map Instance = new Map();

    public bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms) {
        var c = source.Peek();

        if (c == -1 || c != '{') {
            return false;
        }

        var formLoc = loc;
        loc.Column++;
        source.Read();
        
        var itemForms = new Form.Queue();

        while (true) {
            WhiteSpace.Instance.Read(source, vm, ref loc, forms);
            c = source.Peek();

            if (c == -1) {
                throw new ReadError(loc, "Unexpected end of map");
            }
            
            if (c == '}') {
                loc.Column++;
                source.Read();
                break;
            }

            if (!vm.ReadForm(source, ref loc, itemForms)) {
                throw new ReadError(loc, "Unexpected end of map");
            }
        }

        var items = itemForms.AsEnumerable().Select(it => {
            if (it is Forms.Pair p) {
                return (p.Left, p.Right);
            }

            throw new ReadError(it.Loc, $"Invalid map item: {it}");
        }).ToArray();

        forms.Push(new Forms.Map(formLoc, items));
        return true;
    }
}