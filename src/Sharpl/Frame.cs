namespace Sharpl;

public class Frame
{
    public readonly int RegisterIndex;
    public readonly int RegisterCount;
    private List<Value>? Defers = null;

    public Frame(int registerIndex, int registerCount)
    {
        RegisterIndex = registerIndex;
        RegisterCount = registerCount;
    }

    public void Defer(Value target)
    {
        if (Defers == null) { Defers = new List<Value>(); }
        Defers.Add(target);
    }

    public void RunDeferred(VM vm, Loc loc)
    {
        if (Defers is not null)
        {
            foreach (var d in Defers)
            {
                var stack = new Stack();
                d.Call(vm, stack, 0, vm.NextRegisterIndex, true, loc);
            }
        }
    }
}

