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

    public override void Emit(VM vm, Queue args, Register result) =>
        vm.Emit(Ops.Push.Make(Compose(vm, Left, Right, new Queue(), result, Loc)));

    public override void EmitCall(VM vm, Queue args, Register result) =>
        Compose(vm, Left, Right, args, result, Loc).EmitCall(vm, args, result, Loc);

    public override bool Equals(Form other) =>
        (other is And f) && f.Left.Equals(Left) && f.Right.Equals(Right);

    public override bool Expand(VM vm, Queue args)
    {
        var result = false;
        if (Left.Expand(vm, args)) { result = true; }
        var l = args.PopLast();
        if (Right.Expand(vm, args)) { result = true; }
        var r = args.PopLast();
        args.Push(new And(l, r, Loc));
        return result;
    }

    public override Form Quote(VM vm, Loc loc) =>
        new And(Left.Quote(vm, loc), Right.Quote(vm, loc), loc);

    public override string Dump(VM vm) => $"{Left.Dump(vm)} & {Right.Dump(vm)}";

    public override Form Unquote(VM vm, Loc loc) =>
        new And(Left.Unquote(vm, loc), Right.Unquote(vm, loc), loc);
}