namespace Sharpl.Libs;

public class Fix : Lib
{
    public Fix() : base("fix", null, [])
    {
        BindMethod("to-int", ["value"], (loc, target, vm, stack, arity) =>
            stack.Push(Core.Int, (int)Sharpl.Fix.Trunc(stack.Pop().CastUnbox(loc, Core.Fix))));
    }
}