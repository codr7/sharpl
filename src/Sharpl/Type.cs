using Sharpl.Forms;
using System.Text;

namespace Sharpl;

public abstract class AnyType(string name)
{
    public string Name { get; } = name;

    public virtual bool Bool(Value value) => true;
    public virtual void Call(VM vm, Stack stack, int arity, Loc loc) => throw new EvalError("Not supported", loc);

    public virtual void Call(VM vm, Stack stack, Value target, int arity, int registerCount, bool eval, Loc loc)
    {
        switch (arity)
        {
            case 0:
                stack.Push(target);
                break;
            default:
                throw new EvalError($"Wrong number of arguments: {this}", loc);
        }
    }

    public virtual Value Copy(Value value) => value;
    public virtual void Dump(Value value, VM vm, StringBuilder result) => result.Append(value.Data.ToString());
    public virtual void Emit(VM vm, Value value, Form.Queue args, Loc loc) => vm.Emit(Ops.Push.Make(value));

    public virtual void EmitCall(VM vm, Value target, Form.Queue args, Loc loc)
    {
        var arity = args.Count;
        var splat = args.IsSplat;
        if (splat) { vm.Emit(Ops.PushSplat.Make()); }
        args.Emit(vm);
        vm.Emit(Ops.CallDirect.Make(target, arity, splat, vm.NextRegisterIndex, loc));
    }

    public abstract bool Equals(Value left, Value right);
    public virtual void Say(Value value, VM vm, StringBuilder result) => Dump(value, vm, result);
    public virtual string ToJson(Value value, Loc loc) => throw new EvalError($"Not supported: {value}", loc);
    public override string ToString() => Name;
    public virtual Form Unquote(VM vm, Value value, Loc loc) => new Literal(value, loc);
}

public class Type<T>(string name) : AnyType(name)
{
    public override bool Equals(Value left, Value right) => left.CastSlow(this).Equals(right.CastSlow(this));
}