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
    private int arg;

    public NonNumericError(VM vm, Value value, PC retryPC, int arg, Loc loc) :
        base($"Expected numeric value: {value.Dump(vm)}", loc)
    {
        this.retryPC = retryPC;
        this.arg = arg;
    }

    public override void AddRestarts(VM vm)
    {
        vm.AddRestart(vm.Intern("use-value"), 0, (vm, target, arity, result, loc) =>
        {
            var nv = vm.Term.Ask(vm, "Enter new value: ");
            vm.Eval(nv!, new Register(0, arg));            
            vm.Eval(retryPC, result);
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