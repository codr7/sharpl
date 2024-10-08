namespace Sharpl;

public class Error : Exception
{
    public readonly Loc Loc;

    public Error(string message, Loc loc) : base($"{loc} {message}")
    {
        Loc = loc;
    }
};

public class EmitError : Error
{
    public EmitError(string message, Loc loc) : base(message, loc) { }
}

public class EvalError : Error
{
    public EvalError(string message, Loc loc) : base(message, loc) { }
    public virtual void AddRestarts(VM vm) { }
}

public class NonNumericError : EvalError
{
    private PC retryPC;
    private Value[] stack;

    public NonNumericError(VM vm, Value value, PC retryPC, Value[] stack, Loc loc) :
        base($"Expected numeric value: {value.Dump(vm)}", loc)
    {
        this.retryPC = retryPC;
        this.stack = stack;
    }

    public override void AddRestarts(VM vm)
    {
        vm.AddRestart(vm.Intern("use-value"), 0, (vm, stack, target, arity, loc) =>
        {
            var nv = vm.Term.Ask(vm, "Enter new value: ");
            stack.Push((Value)vm.Eval(nv!)!);
            stack.AddRange(this.stack);
            vm.Eval(retryPC, stack);
        });
    }
}

public class ReadError : Error
{
    public ReadError(string message, Loc loc) : base(message, loc) { }
}
public class UserError : EvalError
{
    public readonly Value Value;

    public UserError(VM vm, Value value, Loc loc) : base(value.Say(vm), loc)
    {
        Value = value;
    }
}