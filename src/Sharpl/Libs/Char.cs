namespace Sharpl.Libs;

public class Char : Lib
{
    public Char() : base("char", null, [])
    {
        BindMethod("digit", ["ch"], (loc, target, vm, stack, arity) =>
            stack.Push(Core.Int, stack.Pop().CastUnbox(loc, Core.Char) - '0'));
        
        BindMethod("is-digit", ["ch"], (loc, target, vm, stack, arity) =>
            stack.Push(Core.Bit, char.IsDigit(stack.Pop().CastUnbox(loc, Core.Char))));
    }
}