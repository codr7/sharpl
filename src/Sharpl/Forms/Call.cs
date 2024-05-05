namespace Sharpl.Forms;

public class Call : Form
{
    private readonly Form[] args;
    private readonly Form target;


    public Call(Loc loc, Form target, Form[] args) : base(loc)
    {
        this.target = target;
        this.args = args;
    }

    public override void Emit(VM vm, Lib lib, Form.Queue args)
    {
        target.EmitCall(vm, lib, new Form.Queue(this.args));
    }

    public override string ToString() {
        return $"({target} {args})";
    }     
}