namespace Sharpl;

public readonly struct Op
{
    public enum T
    {
        CallIndirect,
        CallPrim,
        Push,
        Stop
    };

    public readonly object Data;
    public T Type { get; }

    public Op(T type, object data)
    {
        this.Type = type;
        this.Data = data;
    }
}