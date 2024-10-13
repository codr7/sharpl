namespace Sharpl;

public enum OpCode
{
    And,
    BeginFrame, Benchmark, Branch,
    CallDirect, CallMethod, CallRegister, CallTail, CallUserMethod,
    Check, CopyRegister,
    CreateArray, CreateIter, CreateList, CreateMap, CreatePair,
    Decrement,
    EndFrame, ExitMethod,
    Goto,
    Increment, IterNext,
    OpenInputStream, Or,
    PopItem, PrepareClosure, PushItem, PushSplat,
    SetArrayItem, SetLoadPath, SetMapItem, SetRegister, SetRegisterDirect, Splat, Stop, Swap,
    Try,
    Unquote, Unzip
};

public interface Op
{
    OpCode Code { get; }
    string Dump(VM vm);
}