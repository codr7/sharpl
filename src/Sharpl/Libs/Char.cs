namespace Sharpl.Libs;

public class Char : Lib
{
    public Char() : base("char", null, [])
    {
        BindMethod("digit", ["ch"], (vm, stack, target, arity, loc) =>
            stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Char, loc) - '0'));

        BindMethod("is-digit", ["ch"], (vm, stack, target, arity, loc) =>
            stack.Push(Core.Bit, char.IsDigit(stack.Pop().CastUnbox(Core.Char, loc))));
    }
}