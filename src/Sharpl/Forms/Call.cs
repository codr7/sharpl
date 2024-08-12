namespace Sharpl.Forms;

using System.Text;

public class Call : Form
{
    public readonly Form[] Args;
    public readonly Form Target;


    public Call(Loc loc, Form target, Form[] args) : base(loc)
    {
        Target = target;
        Args = args;
    }

    public override void CollectIds(HashSet<string> result)
    {
        Target.CollectIds(result);
        foreach (var f in Args) { f.CollectIds(result); }
    }


    public override void Emit(VM vm, Queue args)
    {
        var splat = false;

        foreach (var f in Args)
        {
            if (f.IsSplat)
            {
                splat = true;
                break;
            }
        }

        var cas = new Queue(Args);
        if (splat) { vm.Emit(Ops.PushSplat.Make()); }
        Target.EmitCall(vm, cas);
        foreach (var a in cas) { args.Push(a); }
    }

    public override bool Equals(Form other)
    {
        if (other is Call f)
        {
            if (!Target.Equals(f.Target) || Args.Length != f.Args.Length) { return false; }

            for (var i = 0; i < Math.Min(Args.Length, f.Args.Length); i++)
            {
                if (!Args[i].Equals(f.Args[i])) { return false; }
            }

            return true;
        }

        return false;
    }

    public override Form Quote(Loc loc, VM vm) =>
        new Literal(Loc, Value.Make(Libs.Core.Form, new Call(loc, Target.Quote(loc, vm), Args.Select(a => a.Quote(loc, vm)).ToArray())));

    public override string ToString()
    {
        var b = new StringBuilder();
        b.Append('(');
        b.Append(Target);
        foreach (var a in Args) { b.Append($" {a}"); }
        b.Append(')');
        return b.ToString();
    }

    public override Form Unquote(Loc loc, VM vm) =>
        new Call(loc, Target.Unquote(loc, vm), Args.Select(a => a.Unquote(loc, vm)).ToArray());
}