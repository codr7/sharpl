namespace Sharpl;

using PC = int;

public readonly record struct Call(UserMethod Target, PC ReturnPC, int FrameOffset, Loc Loc);