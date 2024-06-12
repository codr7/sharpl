namespace Sharpl;

public readonly struct Op
{
    public enum T
    {
        BeginFrame,
        CallDirect, CallIndirect, CallMethod, CallUserMethod, Check, CreateArray,
        EndFrame, EnterMethod,
        GetRegister, Goto,
        Push,
        ExitMethod,
        SetArrayItem, SetLoadPath, SetRegister, Stop
    };

    public readonly dynamic Data;
    public T Type { get; }

    public Op(T type, dynamic data)
    {
        this.Type = type;
        this.Data = data;
    }

    public override string ToString() {
        if (Data is null) {
            return $"({Type})"; 
            
        }
        return Data.ToString(); 
    }
}