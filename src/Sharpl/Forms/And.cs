namespace Sharpl.Forms;

public class And : Form
{
    public readonly Form Left;
    public readonly Form Right;

    public And(Form left, Form right, Loc loc) : base(loc)
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

    public override bool Expand(VM vm, Queue args) {
        var result = false;
        if (Left.Expand(vm, args)) { result = true; }
        var l = args.PopLast();
        if (Right.Expand(vm, args)) { result = true; }
        var r = args.PopLast();
        args.Push(new And(l, r, Loc));
        return result;
    }

    public override Form Quote(Loc loc, VM vm) => 
        new And(Left.Quote(loc, vm), Right.Quote(loc, vm), loc);

    public override string Dump(VM vm) => $"{Left.Dump(vm)} & {Right.Dump(vm)}";

    public override Form Unquote(Loc loc, VM vm) => 
        new And(Left.Unquote(loc, vm), Right.Unquote(loc, vm), loc);
}