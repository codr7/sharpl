namespace Sharpl.Libs;

public class Fix : Lib
{
    public Fix() : base("fix", null, [])
    {
        BindMethod("to-int", ["value"], (vm, stack, target, arity, loc) =>
            stack.Push(Core.Int, (int)Sharpl.Fix.Trunc(stack.Pop().CastUnbox(Core.Fix, loc))));
    }
}