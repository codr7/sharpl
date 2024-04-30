namespace Sharpl;

using System.Text;

using EmitArgs = LinkedList<Form>;
using S = ArrayStack<Value>;

public readonly record struct Value(AnyType Type, dynamic Data)
{
    public static Value Make<T>(Type<T> type, dynamic data)
    {
        return new Value(type, data);
    }

    public static readonly Value Nil = Value.Make(Libs.Core.Nil, false);

    public void Call(Loc loc, VM vm, S stack, int arity, bool recursive) {
        Type.Call(loc, vm, stack, this, arity, recursive);
    }

    public void Dump(StringBuilder result)
    {
        Type.Dump(this, result);
    }

    public void EmitCall(Loc loc, VM vm, Lib lib, EmitArgs args) {
        Type.EmitCall(loc, vm, lib, this, args);
    }

    public bool Equals(Value other)
    {
        return Type == other.Type && Type.Equals(this, other);
    }

    public override int GetHashCode()
    {
        return Data.GetHashCode();
    }

    public void Say(StringBuilder result)
    {
        Type.Say(this, result);
    }

    public override string ToString()
    {
        var res = new StringBuilder();
        Dump(res);
        return res.ToString();
    }
}