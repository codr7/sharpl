using Sharpl.Libs;

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

    public bool HandleError(VM vm, Value value, Stack stack, Loc loc)
    {
        var ev = value;
        vm.Set(LocReg, Value.Make(Core.Loc, loc));
        var handled = false;

        foreach (var (k, v) in Handlers)
        {
            if (k == Value._ ||
                ev.Isa(k.Type) ||
                (k.Type == Core.Meta && ev.Isa(k.Cast(Core.Meta))))
            {
                stack.Push(ev);
                vm.PC = End.PC;
                v.Call(vm, stack, 1, RegisterCount, false, loc);
                handled = true;
                break;
            }
        }

        if (!handled) { vm.PC = End.PC; }
        return handled;
    }
}