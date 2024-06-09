using System.Text;

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

    public override void Emit(VM vm, Env env, Form.Queue args)
    {
        target.EmitCall(vm, env, new Form.Queue(this.args));
    }

    public override string ToString() {
        var b = new StringBuilder();
        b.Append('(');
        b.Append(target);
        
        foreach (var a in args) {
            b.Append($" {a}");
        }

        b.Append(')');
        return b.ToString();
    }     
}