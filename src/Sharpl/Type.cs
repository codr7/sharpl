using Sharpl.Forms;
using Sharpl.Libs;
using System.Text;

namespace Sharpl;

public abstract class AnyType: IComparable<AnyType>
{
    public readonly string Name;
    private readonly List<AnyType> parents = new List<AnyType>();
    private readonly Dictionary<AnyType, int> parentLookup = new Dictionary<AnyType, int>();

    public AnyType(string name, AnyType[] parents)
    {
        Name = name;
        AddParent(this);

        foreach (var pt in parents)
        {
            foreach (var (ppt, _) in pt.parentLookup) { AddParent(ppt); }
        }
    }

    private void AddParent(AnyType type)
    {
        if (parentLookup.ContainsKey(type)) { parentLookup[type] += 1; }
        else {
            parents.Add(type);
            parentLookup[type] = 1; 
        }
    }
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

    public T? Cast<T>() where T : class
    {
        foreach (var pt in parents)
        {
            if (pt is T t) { return t; }
        }

        return null;
    }

    public int CompareTo(AnyType? other)
    {
        var o = other!;
        if (parentLookup.ContainsKey(o)) { return 1; }
        if (o.parentLookup.ContainsKey(this)) { return -1; }
        return 0;
    }

    public virtual Value Copy(Value value) => value;
    public virtual void Dump(VM vm, Value value, StringBuilder result) => result.Append(value.Data.ToString());
    public virtual void Emit(VM vm, Value value, Form.Queue args, Loc loc) => vm.Emit(Ops.Push.Make(value));

    public virtual void EmitCall(VM vm, Value target, Form.Queue args, Loc loc)
    {
        var arity = args.Count;
        var splat = args.IsSplat;
        if (splat) { vm.Emit(Ops.PushSplat.Make()); }
        UserMethod? um = null;

        if (target.Type == Core.UserMethod) { um = target.Cast(Core.UserMethod); }

        for (int i = 0; i < args.Count; i++)
        {
            vm.Emit(args.Items[i]);
            if (um is not null && i < um.Args.Length && um.Args[i].Unzip) { vm.Emit(Ops.Unzip.Make(loc)); }
        }

        args.Clear();
        vm.Emit(Ops.CallDirect.Make(target, arity, splat, vm.NextRegisterIndex, loc));
    }

    public abstract bool Equals(Value left, Value right);

    public virtual bool Isa(AnyType type) =>
        GetType().IsAssignableFrom(type.GetType()) || parentLookup.ContainsKey(type);

    public AnyType[] Parents => parents.ToArray(); 
    public virtual void Say(VM vm, Value value, StringBuilder result) => Dump(vm, value, result);
    public virtual string ToJson(Value value, Loc loc) => throw new EvalError($"Not supported: {value}", loc);
    public override string ToString() => Name;
    public virtual Form Unquote(VM vm, Value value, Loc loc) => new Literal(value, loc);
}

public class Type<T> : AnyType
{
    public Type(string name, AnyType[] parents): base(name, parents) { }
    public override bool Equals(Value left, Value right) => left.CastSlow(this).Equals(right.CastSlow(this));
}

public class BasicType: Type<object>
{
    public BasicType(string name, AnyType[] parents): base(name, parents) {}
}

public class UserTrait: BasicType
{
    public UserTrait(string name, UserTrait[] parents): base(name, parents) {}
}