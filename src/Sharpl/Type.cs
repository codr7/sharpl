namespace Sharpl;

using System.Text;

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

    public virtual void Call(Loc loc, VM vm, Stack stack, Value target, int arity)
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

    public virtual void EmitCall(Loc loc, VM vm, Value target, Form.Queue args)
    {
        var arity = args.Count;
        args.Emit(vm);
        vm.Emit(Ops.CallDirect.Make(loc, target, arity));
    }

    public virtual void EmitId(Loc loc, VM vm, Value value, Form.Queue args)
    {
        vm.Emit(Ops.Push.Make(value));
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