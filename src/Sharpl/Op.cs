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
    Try,
    UnquoteRegister, Unzip
};

public interface Op
{
    OpCode Code { get; }
    string Dump(VM vm);
}