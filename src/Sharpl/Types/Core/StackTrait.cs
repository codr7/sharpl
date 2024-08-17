using Sharpl.Ops;

namespace Sharpl.Types.Core;

public interface StackTrait {
    Value Push(Loc loc, Value dst, Value val);
};