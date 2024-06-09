namespace Sharpl;

public readonly struct Op
{
    public enum T
    {
        CallDirect, CallIndirect, CallMethod, CallPrim, Check,
        GetRegister, Goto,
        Push,
        SetLoadPath, Stop
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