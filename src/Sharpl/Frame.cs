namespace Sharpl;

public class Frame
{
    public readonly int RegisterIndex;
    public readonly int RegisterCount;
    private List<Value>? deferrred = null;

    public Frame(int registerIndex, int registerCount)
    {
        RegisterIndex = registerIndex;
        RegisterCount = registerCount;
    }

    public void Defer(Value target)
    {
        if (deferrred == null) { deferrred = new List<Value>(); }
        deferrred.Add(target);
    }

    public void RunDeferred(VM vm, Loc loc)
    {
        if (deferrred is not null)
        {
            foreach (var d in deferrred)
            {
                var stack = new Stack();
                d.Call(vm, stack, 0, vm.NextRegisterIndex, true, loc);
            }
        }
    }
}

