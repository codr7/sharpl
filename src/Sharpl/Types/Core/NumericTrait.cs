namespace Sharpl.Types.Core;

public interface NumericTrait
{

    void Add(VM vm, Stack stack, int arity, Loc loc);
    void Divide(VM vm, Stack stack, int arity, Loc loc);
    void Multiply(VM vm, Stack stack, int arity, Loc loc);
    void Subtract(VM vm, Stack stack, int arity, Loc loc);
};