namespace Sharpl.Iters.Core;

public class PortItems(Port Source) : BasicIter
{
    public override Value? Next() => Task.Run(Source.Read).Result;
}