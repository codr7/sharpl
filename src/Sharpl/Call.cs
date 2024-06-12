namespace Sharpl;

using PC = int;

public readonly record struct Call(Loc Loc, UserMethod Target, PC ReturnPC);