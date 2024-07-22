namespace Sharpl.Forms;

public class Literal : Form
{
    public readonly Value Value;

    public Literal(Loc loc, Value value) : base(loc)
    {
        Value = value;
    }

    public override void Emit(VM vm, Form.Queue args, int quoted)
    {
        Value.Emit(Loc, vm, args);
    }

    public override void EmitCall(VM vm, Form.Queue args)
    {
        Value.EmitCall(Loc, vm, args);
    }

    public override Value? GetValue(VM vm) { 
        return Value;
    }

    public override string ToString() {
        return Value.ToString();
    }
}