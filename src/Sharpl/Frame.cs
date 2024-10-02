namespace Sharpl;

public class Frame
{
    public readonly int RegisterIndex;
    public readonly int RegisterCount;
    private List<Value>? deferred = null;

    public Frame(int registerIndex, int registerCount)
    {
        RegisterIndex = registerIndex;
        RegisterCount = registerCount;
    }

    public void Defer(Value target)
    {
        if (deferred == null) { deferred = new List<Value>(); }
        deferred.Add(target);
    }

    public void RunDeferred(VM vm, Loc loc)
    {
        if (deferred is not null)
        {
            foreach (var d in deferred.ToArray().Reverse())
            {
                var stack = new Stack();
                d.Call(vm, stack, 0, vm.NextRegisterIndex, true, loc);
            }
        }
    }
}

