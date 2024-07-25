namespace Sharpl.Forms;

public class Unquote : Form
{
    public readonly Form Target;


    public Unquote(Loc loc, Form target) : base(loc)
    {
        Target = target;
    }

    public override void CollectIds(HashSet<string> result)
    {
        Target.CollectIds(result);
    }

    public override void Emit(VM vm, Queue args, int quoted)
    {
        Target.Emit(vm, args, quoted - 1);
    }

    public override bool IsSplat => Target.IsSplat;

    public override string ToString()
    {
        return $",({Target})";
    }
}