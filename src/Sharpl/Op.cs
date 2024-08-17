namespace Sharpl;

public readonly struct Op
{
    public enum T
    {
        BeginFrame, Benchmark, Branch,
        CallDirect, CallStack, CallMethod, CallTail, CallUserMethod, CallRegister, 
        Check, CopyRegister, 
        CreateArray, CreateIter, CreateMap, CreatePair,
        Decrement, Drop,
        EndFrame, ExitMethod,
        GetRegister, Goto,
        Increment, IterNext,
        OpenInputStream, Or, 
        PrepareClosure, Push, PushSplat,
        SetArrayItem, SetLoadPath, SetMapItem, SetRegister, 
        Splat, Stop,
        UnquoteRegister
    };

    public readonly object Data;
    public T Type { get; }

    public Op(T type, object data)
    {
        Type = type;
        Data = data;
    }

    public override string ToString() {
        if (Data is null) {
            return $"{Type}";
            
        }

#pragma warning disable CS8603 // Possible null reference return.
        return Data.ToString();
#pragma warning restore CS8603 // Possible null reference return.
    }
}