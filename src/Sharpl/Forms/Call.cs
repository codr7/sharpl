namespace Sharpl.Forms;

using System.Data;
using System.Text;
using Sharpl.Libs;

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
        var t = Target;

        while (t is Pair pf)
        {
            if (pf.Right is Nil)
            {
                t = pf.Left;
            }
            else if (pf.Left is Nil) { t = pf.Right; }
            else { throw new EvalError($"Invalid call target: {pf}", Loc); }
        }

        t.EmitCall(vm, cas);
        foreach (var a in cas) { args.Push(a); }

        t = Target;

        while (t is Pair pf)
        {
            vm.Emit(Ops.Unzip.Make(Loc));

            if (pf.Right is Nil)
            {
                t = pf.Left;
            }
            else if (pf.Left is Nil)
            {
                t = pf.Right;
                vm.Emit(Ops.Swap.Make(Loc));
            }

            vm.Emit(Ops.Drop.Make(1));
        }
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

    public override bool Expand(VM vm, Queue args) {
        var result = false;

        if (Target.GetValue(vm) is Value tv && tv.Type == Core.Meta && Args.All(a => a is Literal)) {
            var stack = new Stack();
            foreach (var a in Args) { stack.Push((a as Literal)!.Value); }
            Core.Meta.Call(Loc, vm, stack, tv, Args.Length, vm.NextRegisterIndex);
            if (stack.Pop() is Value v) {args.Push(new Literal(Loc, v)); }
            else { throw new EmitError("Expected value", Loc); }
            result = true;
        } else {
            if (Target.Expand(vm, args)) { result = true; }
            var t = args.PopLast();
            var callArgs = new Form[Args.Length];
            
            for (var i = 0; i < Args.Length; i++) {
                if (Args[i].Expand(vm, args)) { result = true; }
                callArgs[i] = args.PopLast();
            }

            args.Push(new Call(Loc, t, callArgs));
        }

        return result;
    }

    public override Form Quote(Loc loc, VM vm) =>
        new Literal(Loc, Value.Make(Libs.Core.Form, new Call(loc, Target.Quote(loc, vm), Args.Select(a => a.Quote(loc, vm)).ToArray())));

    public override string Dump(VM vm)
    {
        var b = new StringBuilder();
        b.Append('(');
        b.Append(Target.Dump(vm));
        foreach (var a in Args) { b.Append($" {a.Dump(vm)}"); }
        b.Append(')');
        return b.ToString();
    }

    public override Form Unquote(Loc loc, VM vm) {
        return new Call(loc, Target.Unquote(loc, vm), Args.Select(a => a.Unquote(loc, vm)).ToArray());
    }
}