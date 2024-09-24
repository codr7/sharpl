namespace Sharpl.Iters;

public class MapItems : Iter
{
    public readonly Value Result;
    public readonly Iter[] Sources;

    public MapItems(Value result, Iter[] sources)
    {
        Result = result;
        Sources = sources;
    }

    public override Value? Next(VM vm, Loc loc)
    {
        var stack = new Stack();

        for (int i = 0; i < Sources.Length; i++)
        {
            if (Sources[i].Next(vm, loc) is Value v) { stack.Push(v); }
            else { return null;  }    
        }

        Result.Call(vm, stack, stack.Count, vm.NextRegisterIndex, true, loc);
        return stack.Pop();
    }

    public override string Dump(VM vm) => 
        $"(map {Result.Dump(vm)} [{string.Join(' ', Sources.Select(s => s.Dump(vm)).ToArray())}])";
}