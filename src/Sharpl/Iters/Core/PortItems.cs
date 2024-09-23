namespace Sharpl.Iters.Core;

public class PortItems(Port source) : Iter
{
    public override Value? Next(VM vm, Loc loc) => Task.Run(async () => await source.Read(vm, loc)).Result;
}