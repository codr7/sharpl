﻿namespace Sharpl.Ops;

public class Try : Op
{
    public static Op Make(OrderedMap<Value, Value> handlers, int registerCount, Label end, Loc loc) => 
        new Try(handlers, registerCount, end, loc);

    public readonly OrderedMap<Value, Value> Handlers;
    public readonly Label End;
    public readonly int RegisterCount;
    public readonly Loc Loc;

    public Try(OrderedMap<Value, Value> handlers, int registerCount, Label end, Loc loc)
    {
        Handlers = handlers;
        End = end;
        RegisterCount = registerCount;
        Loc = loc;
    }

    public OpCode Code => OpCode.Try;
    public string Dump(VM vm) => $"Try {Handlers} {RegisterCount} {End} {Loc}";
}