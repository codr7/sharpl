namespace Sharpl.Forms;

using System.Text;
using Sharpl.Libs;

public class Array : Form
{
    public readonly Form[] Items;

    public Array(Loc loc, Form[] items) : base(loc)
    {
        Items = items;
    }

    public override void CollectIds(HashSet<string> result)
    {
        foreach (var f in Items)
        {
            f.CollectIds(result);
        }
    }

    public override void Emit(VM vm, Form.Queue args)
    {
        var splat = false;

        foreach (var f in Items)
        {
            if (f.IsSplat)
            {
                splat = true;
                break;
            }
        }

        if (splat)
        {
            args.PushFirst(new Call(Loc, new Id(Loc, "Array"), Items));
        }
        else
        {
            var itemArgs = new Form.Queue();

            vm.Emit(Ops.CreateArray.Make(Items.Length));
            var i = 0;

            foreach (var f in Items)
            {
                f.Emit(vm, itemArgs);
                vm.Emit(Ops.SetArrayItem.Make(i));
                i++;
            }
        }
    }

    public override string ToString()
    {
        var b = new StringBuilder();
        b.Append('[');
        var i = 0;

        foreach (var v in Items)
        {
            if (i > 0)
            {
                b.Append(' ');
            }

            b.Append(v);
            i++;
        }

        b.Append(']');
        return b.ToString();
    }
}