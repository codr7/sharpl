using System.Text;

namespace Sharpl.Forms;

public class Quote : Form
{
    public readonly Form Target;
    public readonly int Depth;


    public Quote(Loc loc, Form target, int depth = 1) : base(loc)
    {
        Target = target;
        Depth = depth;
    }

    public override void CollectIds(HashSet<string> result)
    {
        Target.CollectIds(result);
    }

    public override void Emit(VM vm, Queue args, int quoted)
    {
        Target.Emit(vm, args, quoted + Depth);
    }

    public override bool Equals(Form other)
    {
        return (other is Quote f) ? f.Depth == Depth : false;
    }

    public override Form Expand(VM vm, int quoted)
    {
        return new Quote(Loc, Target.Expand(vm, quoted + Depth));
    }

    public override bool IsSplat => Target.IsSplat;

    public override string ToString()
    {
        var buf = new StringBuilder();

        for (var i = 0; i < Depth; i++)
        {
            buf.Append('\'');
        }

        buf.Append(Target);
        return buf.ToString();
    }
}