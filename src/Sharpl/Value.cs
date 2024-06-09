namespace Sharpl;

using System.Text;

public readonly record struct Value(AnyType Type, object Data)
{
    public static explicit operator bool(Value v) {
        return v.Type.Bool(v);
    }

    public static Value Make<T>(Type<T> type, T data) where T : notnull
    {
        return new Value(type, data);
    }

    public static readonly Value F = Value.Make(Libs.Core.Bit, false);
    public static readonly Value Nil = Value.Make(Libs.Core.Nil, false);
    public static readonly Value T = Value.Make(Libs.Core.Bit, true);

    public void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        Type.Call(loc, vm, stack, this, arity);
    }

    public T Cast<T>(Type<T> type)
    {
        if (Type != type)
        {
            throw new Exception($"Type mismatch: {Type}/{type}");
        }

        return (T)Data;
    }

    public T Cast<T>(Loc loc, Type<T> type)
    {
        if (Type != type)
        {
            throw new EvalError(loc, $"Type mismatch: {Type}/{type}");
        }

        return (T)Data;
    }

    public Value Copy()
    {
        return Type.Copy(this);
    }

    public void Dump(StringBuilder result)
    {
        Type.Dump(this, result);
    }

    public void EmitCall(Loc loc, VM vm, Env env, Form.Queue args)
    {
        Type.EmitCall(loc, vm, env, this, args);
    }
    public void EmitId(Loc loc, VM vm, Env env, Form.Queue args)
    {
        Type.EmitId(loc, vm, env, this, args);
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

    public string Say()
    {
        var result = new StringBuilder();
        Say(result);
        return result.ToString();
    }

    public override string ToString()
    {
        var res = new StringBuilder();
        Dump(res);
        return res.ToString();
    }
}