namespace Sharpl.Forms;

using System.Text;

public class Map : Form
{
    public readonly (Form, Form)[] Items;

    public Map(Loc loc, (Form, Form)[] items) : base(loc)
    {
        Items = items;
    }

    public override void CollectIds(HashSet<string> result)
    {
        foreach (var f in Items)
        {
            f.Item1.CollectIds(result);
            f.Item2.CollectIds(result);
        }
    }

    public override void Emit(VM vm, Form.Queue args, int quoted)
    {

        var itemArgs = new Form.Queue();
        vm.Emit(Ops.CreateMap.Make(Items.Length));
        var i = 0;

        foreach (var f in Items)
        {
            f.Item1.Emit(vm, itemArgs, quoted);
            f.Item2.Emit(vm, itemArgs, quoted);
            vm.Emit(Ops.AddMapItem.Make());
            i++;
        }
    }

    public override string ToString()
    {
        var b = new StringBuilder();
        b.Append('{');
        var i = 0;

        foreach (var v in Items)
        {
            if (i > 0)
            {
                b.Append(' ');
            }

            b.Append(v.Item1);
            b.Append(':');
            b.Append(v.Item2);
            i++;
        }

        b.Append('}');
        return b.ToString();
    }
}