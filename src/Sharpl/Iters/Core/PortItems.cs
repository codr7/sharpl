namespace Sharpl.Iters.Core;

public class PortItems(Port source, VM vm, Loc loc) : BasicIter
{
    public override Value? Next() => Task.Run(async () => await source.Read(vm, loc)).Result;
}