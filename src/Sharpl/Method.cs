namespace Sharpl;

public readonly struct Method {
    public delegate void BodyType(Loc loc, Method target, VM vm, S stack, int arity, bool recursive);

    public readonly string[] Args;
    public readonly BodyType Body;
    public readonly string Name;

    public Method(string name, string[] args, BodyType body) {
        Name = name;
        Args = args;
        Body = body;
    }

    public override string ToString() {
        return $"(Method {Name} {Args})";
    }

    public void Call(Loc loc, VM vm, S stack, int arity, bool recursive) {
        Body(loc, this, vm, stack, arity, recursive);
    }
}