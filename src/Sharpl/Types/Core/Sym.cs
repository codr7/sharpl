
using Sharpl.Forms;
using System.Text;

namespace Sharpl.Types.Core;

public class SymType(string name) : Type<Sym>(name), ComparableTrait
{
    public override void Call(VM vm, Stack stack, int arity, Loc loc)
    {
        stack.Reverse(arity);
        var res = new StringBuilder();

        while (arity > 0)
        {
            stack.Pop().Say(vm, res);
            arity--;
        }

        stack.Push(Value.Make(this, vm.Intern(res.ToString())));
    }

    public Order Compare(Value left, Value right)
    {
        var lv = left.Cast(this);
        var rv = right.Cast(this);
        return ComparableTrait.IntOrder(lv.Name.CompareTo(rv.Name));
    }

    public override bool Equals(Value left, Value right) => left.Cast(this) == right.Cast(this);
    public override void Say(Value value, VM vm, StringBuilder result) => result.Append(value.Cast(this).Name);
    public override string ToJson(Value value, Loc loc) => $"\"{value.Cast(this).Name}\"";

    public override Form Unquote(VM vm, Value value, Loc loc)
    {
        var id = value.Cast(this).Name;
        var v = vm.Env[id];
        if (v is null) { throw new EmitError("Missing unquoted value", loc); }
        return new Literal((Value)v, loc);
    }
}
