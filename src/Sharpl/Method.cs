namespace Sharpl;

public readonly struct Method
{
    public delegate void BodyType(VM vm, Stack stack, Method target, int arity, Loc loc);

    public readonly string[] Args;
    public readonly BodyType Body;
    public readonly string Name;
    public readonly int MinArgCount;

    public Method(string name, string[] args, BodyType body)
    {
        Name = name;
        Args = args;
        MinArgCount = args.Count((a) => !a.EndsWith('?'));
        Body = body;

    }

    public void Call(VM vm, Stack stack, int arity, Loc loc)
    {
        if (arity < MinArgCount) { throw new EvalError($"Not enough arguments: {this}", loc); }
        Body(vm, stack, this, arity, loc);
    }

    public override string ToString() =>
        $"(^{Name} [{string.Join(' ', Args)}])";
}