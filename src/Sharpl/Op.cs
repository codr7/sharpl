namespace Sharpl;

public enum OpCode
{
    And,
    BeginFrame, Benchmark, Branch,
    CallDirect, CallMethod, CallRegister, CallStack, CallTail, CallUserMethod,
    Check, CopyRegister,
    CreateArray, CreateIter, CreateList, CreateMap, CreatePair,
    Decrement, Drop,
    EndFrame, ExitMethod,
    GetRegister, Goto,
    Increment, IterNext,
    OpenInputStream, Or,
    PopItem, PrepareClosure, Push, PushItem, PushSplat,
    Repush,
    SetArrayItem, SetLoadPath, SetMapItem, SetRegister, Splat, Stop, Swap,
    UnquoteRegister, Unzip
};

public abstract class Op
{
    public readonly OpCode Code;
    public Op(OpCode code)
    {
        Code = code;
    }

    public abstract string Dump(VM vm);
}