namespace Sharpl.Iters.Core;

public class PortItems(Port Source) : BasicIter
{
    public override Value? Next() => Source.Read();
}