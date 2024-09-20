namespace Sharpl;

public readonly struct Method
{
    public delegate void BodyType(Loc loc, Method target, VM vm, Stack stack, int arity);

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

    public void Call(Loc loc, VM vm, Stack stack, int arity)
    {
        if (arity < MinArgCount) { throw new EvalError($"Not enough arguments: {this}", loc); }
        Body(loc, this, vm, stack, arity);
    }

    public override string ToString() =>
        $"(^{Name} [{string.Join(' ', Args)}])";
}