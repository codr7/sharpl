namespace Sharpl.Iters;

public class FilterItems : Iter
{
    public readonly Value Predicate;
    public readonly Iter Source;

    public FilterItems(Value predicate, Iter source)
    {
        Predicate = predicate;
        Source = source;
    }

    public override bool Next(VM vm, Register result, Loc loc)
    {
        while (Source.Next(vm, result, loc))
        {
            var v = vm.Get(result);
            Predicate.Call(vm, [v], vm.NextRegisterIndex, true, result, loc);

            if ((bool)vm.Get(result))
            {
                vm.Set(result, v);
                return true;
            }
        }

        return false;
    }

    public override string Dump(VM vm) => $"(filter {Predicate.Dump(vm)} {Source.Dump(vm)})";
}