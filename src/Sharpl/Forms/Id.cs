namespace Sharpl.Forms;

using EmitArgs = LinkedList<Form>;

public class Id : Form
{
    public readonly string Name;

    public Id(Loc loc, string name): base(loc) {
        Name = name;
    }

    public override void Emit(VM vm, Lib lib, EmitArgs args) {
        if (lib[Name] is Value v) {
            vm.Emit(Ops.Push.Make(v));
        } else {
            throw new EmitError(Loc, $"Unknown id: {Name}");
        }
    }
}