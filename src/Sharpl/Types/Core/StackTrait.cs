namespace Sharpl.Types.Core;

public interface StackTrait
{
    Value Peek(Loc loc, VM vm, Value srcVal);
    Value Pop(Loc loc, VM vm, Register src, Value srcVal);
    void Push(Loc loc, VM vm, Register dst, Value dstVal, Value val);
};