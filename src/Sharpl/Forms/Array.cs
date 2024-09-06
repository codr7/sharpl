using System.Text;

namespace Sharpl.Forms;

public class Array : Form
{
    public readonly Form[] Items;

    public Array(Loc loc, Form[] items) : base(loc)
    {
        Items = items;
    }

    public override void CollectIds(HashSet<string> result)
    {
        foreach (var f in Items) { f.CollectIds(result); }
    }

    public override void Emit(VM vm, Queue args)
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
            var its = Items;
            Form cf = new Call(Loc, new Id(Loc, "Array"), its);
            args.PushFirst(cf);
        }
        else
        {
            vm.Emit(Ops.CreateArray.Make(Items.Length));
            var i = 0;

            foreach (var f in Items)
            {
                vm.Emit(f);
                vm.Emit(Ops.SetArrayItem.Make(i));
                i++;
            }
        }
    }

    public override bool Equals(Form other)
    {
        if (other is Array f)
        {
            if (Items.Length != f.Items.Length) { return false; }

            for (var i = 0; i < Math.Min(Items.Length, f.Items.Length); i++)
            {
                if (!Items[i].Equals(f.Items[i])) { return false; }
            }

            return true;
        }

        return false;
    }

    public override bool Expand(VM vm, Queue args) {
        var result = false;
        var newItems = new Form[Items.Length];
        
        for (var i = 0; i < Items.Length; i++) {
            if (Items[i].Expand(vm, args)) { result = true; }
            newItems[i] = args.PopLast();
        }

        args.Push(new Array(Loc, newItems));
        return result;
    }

    public override Value? GetValue(VM vm) =>
        Items.All(it => it is Literal) ? Value.Make(Libs.Core.Array, Items.Select(it => (it as Literal)!.Value.Copy()).ToArray()) : null;

    public override Form Quote(Loc loc, VM vm) => 
        new Array(loc, Items.Select(it => it.Quote(loc, vm)).ToArray());

    public override string Dump(VM vm)
    {
        var b = new StringBuilder();
        b.Append('[');
        var i = 0;

        foreach (var v in Items)
        {
            if (i > 0) { b.Append(' '); }
            b.Append(v.Dump(vm));
            i++;
        }

        b.Append(']');
        return b.ToString();
    }

    public override Form Unquote(Loc loc, VM vm) =>
        new Array(loc, Items.Select(it => it.Unquote(loc, vm)).ToArray());
}