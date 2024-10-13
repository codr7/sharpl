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

    public override bool Next(VM vm, Register result, Loc loc)
    {
        var args = new List<Value>();

        for (int i = 0; i < Sources.Length; i++)
        {
            if (Sources[i].Next(vm, result, loc)) { args.Push(vm.Get(result)); }
            else { return false;  }    
        }

        Result.Call(vm, args.ToArray(), vm.NextRegisterIndex, true, result, loc);
        return true;
    }

    public override string Dump(VM vm) => 
        $"(map {Result.Dump(vm)} [{string.Join(' ', Sources.Select(s => s.Dump(vm)).ToArray())}])";
}