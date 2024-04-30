namespace Sharpl;

using System.Text;

using EmitArgs = LinkedList<Form>;

using S = ArrayStack<Value>;

public class AnyType
{
    public string Name { get; }

    public virtual void Call(Loc loc, VM vm, S stack, Value target, int arity, bool recursive) {
        if (arity != 0) {
            throw new EvalError(loc, "Wrong number of arguments");
        }

        stack.Push(target);
    }

    public virtual void Dump(Value value, StringBuilder result)
    {
        result.Append(value.Data.ToString());
    }

    public virtual void EmitCall(Loc loc, VM vm, Lib lib, Value target, EmitArgs args) {
        throw new EvalError(loc, "Call not supported");
    }

    public virtual bool Equals(Value left, Value right) {
        return left.Data == right.Data;
    }

    public virtual void Say(Value value, StringBuilder result) {
        Dump(value, result);
    }

    public override string ToString() {
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
}