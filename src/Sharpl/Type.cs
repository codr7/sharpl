using System.Text;
using System.Text.Json;
using Sharpl.Forms;
using Sharpl.Libs;

namespace Sharpl;

public abstract class AnyType(string name)
{
    public string Name { get; } = name;

    public virtual bool Bool(Value value) => true;
    public virtual void Call(Loc loc, VM vm, Stack stack, int arity) => throw new EvalError(loc, "Not supported");

    public virtual void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        if (arity != 0) { throw new EvalError(loc, "Wrong number of arguments"); }
        stack.Push(target);
    }

    public virtual Value Copy(Value value) => value;
    public virtual void Dump(Value value, VM vm, StringBuilder result) => result.Append(value.Data.ToString());
    public virtual void Emit(Loc loc, VM vm, Value value, Form.Queue args) => vm.Emit(Ops.Push.Make(value));

    public virtual void EmitCall(Loc loc, VM vm, Value target, Form.Queue args)
    {
        var arity = args.Count;
        var splat = args.IsSplat;
        if (splat) { vm.Emit(Ops.PushSplat.Make()); }
        args.Emit(vm);
        vm.Emit(Ops.CallDirect.Make(loc, target, arity, splat, vm.NextRegisterIndex));
    }

    public abstract bool Equals(Value left, Value right);
    public virtual void Say(Value value, VM vm, StringBuilder result) => Dump(value, vm, result);
    public virtual string ToJson(Loc loc, Value value) => throw new EvalError(loc, $"Not supported: {value}");
    public override string ToString() => Name;
    public virtual Form Unquote(Loc loc, VM vm, Value value) => new Literal(loc, value);
}

public class Type<T>(string name) : AnyType(name)
{
    public override bool Equals(Value left, Value right) => left.CastSlow(this).Equals(right.CastSlow(this));
}