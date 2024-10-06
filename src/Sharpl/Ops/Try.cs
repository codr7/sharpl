namespace Sharpl.Ops;

public class Try : Op
{
    public static Op Make((Value, Value)[] handlers, int registerCount, Label end, Register locReg, Loc loc) => 
        new Try(handlers, registerCount, end, locReg, loc);

    public readonly (Value, Value)[] Handlers;
    public readonly Label End;
    public readonly int RegisterCount;
    public readonly Loc Loc;
    public readonly Register LocReg;

    public Try((Value, Value)[] handlers, int registerCount, Label end, Register locReg, Loc loc)
    {
        Handlers = handlers;
        End = end;
        RegisterCount = registerCount;
        Loc = loc;
        LocReg = locReg;
    }

    public OpCode Code => OpCode.Try;
    public string Dump(VM vm) => $"Try {Handlers} {RegisterCount} {End} {LocReg} {Loc}";
}