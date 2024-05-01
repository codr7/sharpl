namespace Sharpl.Forms;

public class Id : Form
{
    public readonly string Name;

    public Id(Loc loc, string name) : base(loc)
    {
        Name = name;
    }

    public override void Emit(VM vm, Lib lib, EmitArgs args)
    {
        if (lib[Name] is Value v)
        {
            v.EmitId(Loc, vm, lib, args);
        }
        else
        {
            throw new EmitError(Loc, $"Unknown id: {Name}");
        }
    }

    public override void EmitCall(VM vm, Lib lib, EmitArgs args)
    {
        if (lib[Name] is Value v)
        {
            v.EmitCall(Loc, vm, lib, args);
        }
        else
        {
            throw new EmitError(Loc, $"Unknown id: {Name}");
        }
    }
}