namespace Sharpl;

public readonly struct Op
{
    public enum T
    {
        BeginFrame, Benchmark, Branch,
        CallDirect, CallIndirect, CallMethod, CallUserMethod, Check, CopyRegister, CreateArray,
        Decrement,
        EndFrame, ExitMethod,
        GetRegister, Goto,
        OpenInputStream, 
        PrepareClosure, Push, PushSplat,
        SetArrayItem, SetLoadPath, SetRegister, Splat, Stop
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