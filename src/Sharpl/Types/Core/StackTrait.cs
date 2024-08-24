namespace Sharpl.Types.Core;

public interface StackTrait {
    void Push(Loc loc, VM vm, Register dst, Value dstVal, Value val);
};