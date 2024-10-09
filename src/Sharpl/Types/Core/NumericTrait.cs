namespace Sharpl.Types.Core;

public interface NumericTrait
{
    void Add(VM vm, int arity, Register result, Loc loc);
    void Divide(VM vm, int arity, Register result, Loc loc);
    void Multiply(VM vm, int arity, Register result, Loc loc);
    void Subtract(VM vm, int arity, Register result, Loc loc);
};