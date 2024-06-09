namespace Sharpl.Forms;

using System.Text;

public class Array : Form
{
    public readonly Form[] Items;

    public Array(Loc loc, Form[] items) : base(loc)
    {
        Items = items;
    }

    public override void Emit(VM vm, Env env, Form.Queue args)
    {
        var itemArgs = new Form.Queue();

        vm.Emit(Ops.CreateArray.Make(Items.Length));
        var i = 0;

        foreach (var f in Items) {
            f.Emit(vm, env, itemArgs);
            vm.Emit(Ops.SetArrayItem.Make(i));
            i++;
        }
    }

    public override string ToString() {
        var b = new StringBuilder();
        b.Append('[');
        var i = 0;

        foreach (var v in Items) {
            if (i > 0) {
                b.Append(' ');
            }

            b.Append(v);
            i++;
        }

        b.Append(']');
        return b.ToString();
    }     
}