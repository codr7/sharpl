namespace Sharpl.Forms;

using System.Text;

public class Call : Form
{
    private readonly Form[] Args;
    private readonly Form Target;


    public Call(Loc loc, Form target, Form[] args) : base(loc)
    {
        this.Target = target;
        this.Args = args;
    }

    public override void CollectIds(HashSet<string> result)
    {
        Target.CollectIds(result);

        foreach (var f in Args)
        {
            f.CollectIds(result);
        }
    }


    public override void Emit(VM vm, Form.Queue args)
    {
        var cas =  new Form.Queue(Args);
        Target.EmitCall(vm,cas);

        foreach (var a in cas) {
            args.Push(a);
        }
    }

    public override string ToString()
    {
        var b = new StringBuilder();
        b.Append('(');
        b.Append(Target);

        foreach (var a in Args)
        {
            b.Append($" {a}");
        }

        b.Append(')');
        return b.ToString();
    }
}