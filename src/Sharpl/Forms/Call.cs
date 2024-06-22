namespace Sharpl.Forms;

using System.Text;

public class Call : Form
{
    private readonly Form[] args;
    private readonly Form target;


    public Call(Loc loc, Form target, Form[] args) : base(loc)
    {
        this.target = target;
        this.args = args;
    }

    public override void CollectIds(HashSet<string> result)
    {
        target.CollectIds(result);

        foreach (var f in args)
        {
            f.CollectIds(result);
        }
    }


    public override void Emit(VM vm, Form.Queue args)
    {
        var cas =  new Form.Queue(this.args);
        target.EmitCall(vm,cas);

        foreach (var a in cas) {
            args.Push(a);
        }
    }

    public override string ToString()
    {
        var b = new StringBuilder();
        b.Append('(');
        b.Append(target);

        foreach (var a in args)
        {
            b.Append($" {a}");
        }

        b.Append(')');
        return b.ToString();
    }
}