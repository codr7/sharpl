namespace Sharpl.Libs;

public class Char : Lib
{
    public Char() : base("char", null, [])
    {
        BindMethod("digit", ["ch"], (vm, stack, target, arity, loc) =>
            stack.Push(Core.Int, stack.Pop().CastUnbox(Core.Char, loc) - '0'));

        BindMethod("down", ["in"], (vm, stack, target, arity, loc) =>
        {
            var c = stack.Pop().CastUnbox(Core.Char, loc);
            stack.Push(Core.Char, char.ToLower(c));
        });

        BindMethod("is-digit", ["ch"], (vm, stack, target, arity, loc) =>
            stack.Push(Core.Bit, char.IsDigit(stack.Pop().CastUnbox(Core.Char, loc))));

        BindMethod("up", ["in"], (vm, stack, target, arity, loc) =>
        {
            var c = stack.Pop().CastUnbox(Core.Char, loc);
            stack.Push(Core.Char, char.ToUpper(c));
        });
    }
}