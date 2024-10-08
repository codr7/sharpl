using Sharpl.Types.Core;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sharpl;

public readonly record struct Value(AnyType Type, object Data) : IComparable<Value>
{
    public static explicit operator bool(Value v) => v.Type.Bool(v);
    public static Value Make<T>(Type<T> type, T data) where T : notnull => new Value(type, data);

    public static readonly Value _ = Make(Libs.Core.Nil, false);
    public static readonly Value F = Make(Libs.Core.Bit, false);
    public static readonly Value T = Make(Libs.Core.Bit, true);
    
    public void Call(VM vm, Stack stack, int arity, int registerCount, bool eval, Loc loc) =>
        Type.Call(vm, stack, this, arity, registerCount, eval, loc);

    // Please do not remove the type checks below.
    // These methods provide slightly more optimal type check and cast path
    // as compared to regular cast operators. The reason for this is that
    // Sharpl has its own type system abstraction which upholds type safety
    // guarantees, therefore we can avoid double-checking whether the type is
    // correct. However, if the type comparisons below are removed, this will
    // make the code below very unsafe by reinterpreting structs and classes
    // as arbitrary types which will can lead to memory corruption and crashes.
    public T Cast<T>(Type<T> type) where T : class =>
        (Type.Cast<Type<T>>() is Type<T>) ? Unsafe.As<T>(Data) : TypeMismatch(Type, type);

    public T Cast<T>(Type<T> type, Loc loc) where T : class =>
        (Type.Cast<Type<T>>() is Type<T>) ? Unsafe.As<T>(Data) : TypeMismatch(loc, Type, type);

    public T CastUnbox<T>(Type<T> type) where T : struct =>
        (Type.Cast<Type<T>>() is Type<T>) ? Unsafe.As<StrongBox<T>>(Data).Value : TypeMismatch(Type, type);

    public T CastUnbox<T>(Type<T> type, Loc loc) where T : struct =>
        (Type.Cast<Type<T>>() is Type<T>) ? Unsafe.As<StrongBox<T>>(Data).Value : TypeMismatch(loc, Type, type);

    // Do not remove Nullable<T> overloads - they are necessary
    // to correctly handle unboxing of nullable structs.
    public T? CastUnbox<T>(Type<T?> type) where T : struct => (T?)Data;

    public T? CastUnbox<T>(Type<T?> type, Loc loc) where T : struct =>
        (Type.Cast<Type<T>>() is Type<T>) ? (T?)Data : TypeMismatch(loc, Type, type);

    public T CastSlow<T>(Type<T> type) => (T)Data;

    public int CompareTo(Value other)
    {
        if (other.Type != Type) { return Type.Name.CompareTo(other.Type.Name); }
        if (Type is ComparableTrait ct)
        { return ComparableTrait.OrderInt(ct.Compare(this, other)); }
        throw new Exception("Not comparable");
    }

    public Value Copy() => Type.Copy(this);
    public void Dump(VM vm, StringBuilder result) => Type.Dump(vm, this, result);

    public string Dump(VM vm)
    {
        var res = new StringBuilder();
        Dump(vm, res);
        return res.ToString();
    }

    public void Emit(VM vm, Form.Queue args, Loc loc) => Type.Emit(vm, this, args, loc);
    public void EmitCall(VM vm, Form.Queue args, Loc loc) => Type.EmitCall(vm, this, args, loc);
    public bool Equals(Value other) => Type == other.Type && Type.Equals(this, other);
    public override int GetHashCode() => Data.GetHashCode();

    public bool Isa(AnyType parent) => Type.Isa(parent);
    public void Say(VM vm, StringBuilder result) => Type.Say(vm, this, result);

    public string Say(VM vm)
    {
        var result = new StringBuilder();
        Say(vm, result);
        return result.ToString();
    }

    public string ToJson(Loc loc) => Type.ToJson(this, loc);

    public T? TryCastUnbox<T>(Type<T> type) where T : struct =>
        Type == type ? Unsafe.Unbox<T>(Data) : default(T?);

    public Form Unquote(VM vm, Loc loc) => Type.Unquote(vm, this, loc);

    [DoesNotReturn, StackTraceHidden]
    static T TypeMismatch<T>(AnyType lhs, Type<T> rhs) =>
        throw new InvalidCastException($"Type mismatch: {lhs}/{rhs}");

    [DoesNotReturn, StackTraceHidden]
    static T TypeMismatch<T>(Loc loc, AnyType lhs, Type<T> rhs) =>
        throw new EvalError($"Type mismatch: {lhs}/{rhs}", loc);
}