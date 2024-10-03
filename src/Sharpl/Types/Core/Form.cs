using System.Text;

namespace Sharpl.Types.Core;

public class FormType(string name, AnyType[] parents) : Type<Form>(name, parents)
{
    public override void Dump(Value value, VM vm, StringBuilder result) => result.Append(value.Cast(this));
    public override bool Equals(Value left, Value right) => left.Cast(this).Equals(right.Cast(this));
    public override Form Unquote(VM vm, Value value, Loc loc) => value.Cast(this).Unquote(vm, loc);
}