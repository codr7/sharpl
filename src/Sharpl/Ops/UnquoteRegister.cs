namespace Sharpl.Ops;

public readonly record struct UnquoteRegister(Loc Loc, Register Register)
{
    public static Op Make(Loc loc, Register register) =>
        new Op(Op.T.UnquoteRegister, new UnquoteRegister(loc, register));

    public override string ToString() => $"UnquoteRegister {Loc} {Register}";
}