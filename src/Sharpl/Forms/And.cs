using System.Runtime.CompilerServices;

namespace Sharpl.Forms;

public class And : Form
{
    public readonly Form Left;
    public readonly Form Right;

    public And(Loc loc, Form left, Form right) : base(loc)
    {
        Left = left;
        Right = right;
    }

    public override void CollectIds(HashSet<string> result)
    {
        Left.CollectIds(result);
        Right.CollectIds(result);
    }

    public override void Emit(VM vm, Queue args, int quoted)
    {
        vm.Emit(Ops.Push.Make(vm.Compose(Loc, Left, Right, new Queue())));
    }

    public override void EmitCall(VM vm, Queue args, int quoted)
    {
        vm.Compose(Loc, Left, Right, args).EmitCall(Loc, vm, args, quoted);
    }

    public override bool Equals(Form other)
    {
        if (other is And f) {
            return f.Left.Equals(Left) && f.Right.Equals(Right);
        }

        return false;
    }

    public override string ToString()
    {
        return $"{Left} & {Right}";
    }
}