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

    public override void Emit(VM vm, Queue args) =>
        vm.Emit(Ops.Push.Make(vm.Compose(Loc, Left, Right, new Queue())));

    public override void EmitCall(VM vm, Queue args) =>
        vm.Compose(Loc, Left, Right, args).EmitCall(Loc, vm, args);

    public override bool Equals(Form other) =>
        (other is And f) && f.Left.Equals(Left) && f.Right.Equals(Right);

    public override Form Quote(Loc loc, VM vm) => 
        new And(loc, Left.Quote(loc, vm), Right.Quote(loc, vm));

    public override string ToString() => $"{Left} & {Right}";

    public override Form Unquote(Loc loc, VM vm) => 
        new And(loc, Left.Unquote(loc, vm), Right.Unquote(loc, vm));
}