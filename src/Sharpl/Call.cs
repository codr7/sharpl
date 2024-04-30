namespace Sharpl;

using PC = int;

public readonly record struct Call(Loc Loc, Value Target, PC ReturnPC);