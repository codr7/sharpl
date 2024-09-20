using System.Text;

namespace Sharpl.Forms;

public class Array : Form
{
    public readonly Form[] Items;

    public Array(Form[] items, Loc loc) : base(loc)
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
            Form cf = new Call(new Id("Array", Loc), its, Loc);
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

    public override bool Expand(VM vm, Queue args)
    {
        var result = false;
        var newItems = new Form[Items.Length];

        for (var i = 0; i < Items.Length; i++)
        {
            if (Items[i].Expand(vm, args)) { result = true; }
            newItems[i] = args.PopLast();
        }

        args.Push(new Array(newItems, Loc));
        return result;
    }

    public override Value? GetValue(VM vm) =>
        Items.All(it => it is Literal) ? Value.Make(Libs.Core.Array, Items.Select(it => (it as Literal)!.Value.Copy()).ToArray()) : null;

    public override Form Quote(VM vm, Loc loc) =>
        new Array(Items.Select(it => it.Quote(vm, loc)).ToArray(), loc);

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

    public override Form Unquote(VM vm, Loc loc) =>
        new Array(Items.Select(it => it.Unquote(vm, loc)).ToArray(), loc);
}