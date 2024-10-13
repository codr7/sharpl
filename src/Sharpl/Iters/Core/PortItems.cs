namespace Sharpl.Iters.Core;

public class PortItems(Port source) : Iter
{
    public override bool Next(VM vm, Register result, Loc loc)
    {
        if (Task.Run(async () => await source.Read(vm, loc)).Result is Value v)
        {
            vm.Set(result, v);
            return true;
        }

        return false;
    }
}