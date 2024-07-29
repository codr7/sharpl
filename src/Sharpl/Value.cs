namespace Sharpl;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Sharpl.Types.Core;

public readonly record struct Value(AnyType Type, object Data) : IComparable<Value>
{
    public static explicit operator bool(Value v)
    {
        return v.Type.Bool(v);
    }

    public static Value Make<T>(Type<T> type, T data) where T : notnull
    {
        return new Value(type, data);
    }

    public static readonly Value F = Make(Libs.Core.Bit, false);
    public static readonly Value Nil = Make(Libs.Core.Nil, false);
    public static readonly Value T = Make(Libs.Core.Bit, true);

    public void Call(Loc loc, VM vm, Stack stack, int arity, int registerCount)
    {
        Type.Call(loc, vm, stack, this, arity, registerCount);
    }

    public T Cast<T>(Type<T> type) where T : class =>
        Type == type ? Unsafe.As<T>(Data) : TypeMismatch(Type, type);

    public T Cast<T>(Loc loc, Type<T> type) where T : class =>
        Type == type ? Unsafe.As<T>(Data) : TypeMismatch(loc, Type, type);

    public T CastUnbox<T>(Type<T> type) where T : struct =>
        Type == type ? Unsafe.Unbox<T>(Data) : TypeMismatch(Type, type);

    public T CastUnbox<T>(Loc loc, Type<T> type) where T : struct =>
        Type == type ? Unsafe.Unbox<T>(Data) : TypeMismatch(loc, Type, type);

    public T CastSlow<T>(Type<T> type) =>
        Type == type ? (T)Data : TypeMismatch(Type, type);

    public int CompareTo(Value other)
    {
        if (other.Type != Type)
        {
            return Type.Name.CompareTo(other.Type.Name);
        }

        if (Type is ComparableTrait ct)
        {
            return ComparableTrait.OrderInt(ct.Compare(this, other));
        }

        throw new Exception("Not comparable");
    }

    public Value Copy()
    {
        return Type.Copy(this);
    }

    public void Dump(StringBuilder result)
    {
        Type.Dump(this, result);
    }

    public void Emit(Loc loc, VM vm, Form.Queue args)
    {
        Type.Emit(loc, vm, this, args);
    }

    public void EmitCall(Loc loc, VM vm, Form.Queue args)
    {
        Type.EmitCall(loc, vm, this, args);
    }

    public void EmitId(Loc loc, VM vm, Form.Queue args)
    {
        Type.EmitId(loc, vm, this, args);
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

    public T? TryCastUnbox<T>(Type<T> type) where T : struct =>
        Type == type ? Unsafe.Unbox<T>(Data) : default(T?);

    public Form Unquote(Loc loc, VM vm)
    {
        return Type.Unquote(this, loc, vm);
    }

    [DoesNotReturn, StackTraceHidden]
    static T TypeMismatch<T>(AnyType lhs, Type<T> rhs)
    {
        throw new Exception($"Type mismatch: {lhs}/{rhs}");
    }

    [DoesNotReturn, StackTraceHidden]
    static T TypeMismatch<T>(Loc loc, AnyType lhs, Type<T> rhs)
    {
        throw new EvalError(loc, $"Type mismatch: {lhs}/{rhs}");
    }
}