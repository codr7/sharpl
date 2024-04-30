namespace Sharpl;

public readonly struct Op
{
    public enum T
    {
        Push,
        Stop
    };

    public readonly dynamic Data;
    public T Type { get; }

    public Op(T type, dynamic data)
    {
        this.Type = type;
        this.Data = data;
    }
}