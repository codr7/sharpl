namespace Sharpl;

using System.Text;
using Sharpl.Forms;

public abstract class AnyType
{
    public string Name { get; }

    public virtual bool Bool(Value value)
    {
        return true;
    }

    public virtual void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        throw new EvalError(loc, "Not supported");
    }

    public virtual void Call(Loc loc, VM vm, Stack stack, Value target, int arity, int registerCount)
    {
        if (arity != 0)
        {
            throw new EvalError(loc, "Wrong number of arguments");
        }

        stack.Push(target);
    }

    public virtual Value Copy(Value value)
    {
        return value;
    }

    public virtual void Dump(Value value, StringBuilder result)
    {
        result.Append(value.Data.ToString());
    }

    public virtual void Emit(Loc loc, VM vm, Value value, Form.Queue args)
    {
        vm.Emit(Ops.Push.Make(value));
    }

    public virtual void EmitCall(Loc loc, VM vm, Value target, Form.Queue args)
    {
        var arity = args.Count;
        var splat = args.IsSplat;

        if (splat)
        {
            vm.Emit(Ops.PushSplat.Make());
        }

        args.Emit(vm);
        vm.Emit(Ops.CallDirect.Make(loc, target, arity, splat, vm.NextRegisterIndex));
    }

    public virtual void EmitId(Loc loc, VM vm, Value value, Form.Queue args)
    {
        Emit(loc, vm, value, args);
    }

    public abstract bool Equals(Value left, Value right);

    public virtual void Say(Value value, StringBuilder result)
    {
        Dump(value, result);
    }

    public override string ToString()
    {
        return $"(Type {Name})";
    }

    protected AnyType(string name)
    {
        Name = name;
    }
}

public class Type<T> : AnyType
{
    public Type(string name) : base(name) { }

    public override bool Equals(Value left, Value right)
    {
        return left.Cast(this).Equals(right.Cast(this));
    }
}