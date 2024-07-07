namespace Sharpl.Types.Core;

public interface NumericTrait {
    
    void Add(Loc loc, VM vm, Stack stack, int arity);
    void Divide(Loc loc, VM vm, Stack stack, int arity);
    void Multiply(Loc loc, VM vm, Stack stack, int arity);
    void Subtract(Loc loc, VM vm, Stack stack, int arity);
};