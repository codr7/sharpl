
using System.Text;
using Sharpl.Forms;

namespace Sharpl.Types.Core;

public class SymType(string name) : Type<Sym>(name), ComparableTrait
{
    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        stack.Reverse(arity);
        var res = new StringBuilder();

        while (arity > 0)
        {
            stack.Pop().Say(res);
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
    public override void Say(Value value, StringBuilder result) => result.Append(value.Cast(this).Name);

    public override Form Unquote(Loc loc, VM vm, Value value)
    {
        var id = value.Cast(this).Name;
        var v = vm.Env[id];
        if (v is null) { throw new EmitError(loc, "Missing unquoted value"); }
        return new Literal(loc, (Value)v);
    }
}
