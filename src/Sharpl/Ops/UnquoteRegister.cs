namespace Sharpl.Ops;

public readonly record struct UnquoteRegister(Loc Loc, Register Register) : Op
{
    public static Op Make(Loc loc, Register register) => new UnquoteRegister(loc, register);
    public override string ToString() => $"UnquoteRegister {Loc} {Register}";
}