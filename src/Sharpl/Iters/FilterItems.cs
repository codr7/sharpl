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

    public override Value? Next(VM vm, Loc loc)
    {
        while (Source.Next(vm, loc) is Value v)
        {
            var stack = new Stack();
            stack.Push(v);
            Predicate.Call(vm, stack, 1, vm.NextRegisterIndex, true, loc);
            if ((bool)stack.Pop()) { return v; }
        }

        return null;
    }

    public override string Dump(VM vm) => $"(filter {Predicate.Dump(vm)} {Source.Dump(vm)})";
}