
namespace Sharpl.Types.Core;

using System.Text;

public class SymbolType : Type<Symbol>
{
    public SymbolType(string name) : base(name) { }
    
    public override void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        stack.Reverse(arity);
        var res = new StringBuilder();

        while (arity > 0)
        {
            stack.Pop().Say(res);
            arity--;
        }

        stack.Push(Value.Make(this, vm.GetSymbol(res.ToString())));
    }

    public override bool Equals(Value left, Value right)
    {
        return left.Cast(this) == right.Cast(this);
    }

    public override void Say(Value value, StringBuilder result)
    {
        result.Append(value.Cast(this).Name);
    }
}
