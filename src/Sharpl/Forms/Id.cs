using Sharpl.Libs;

namespace Sharpl.Forms;

public class Id : Form
{
    public static Value? Find(string name, Lib lib, Loc loc)
    {
        while (true)
        {
            var i = name.IndexOf('/');
            if (i <= 0) { break; }
            var ln = name.Substring(0, i);
            var lv = lib[ln];

            if (lv is null)
            {
                throw new EmitError(loc, $"Unknown id: {ln}");
            }

            lib = ((Value)lv).Cast(loc, Core.Lib);
            name = name.Substring(i + 1);
        }

        return lib[name];
    }

    public readonly string Name;

    public Id(Loc loc, string name) : base(loc)
    {
        Name = name;
    }

    public override void Emit(VM vm, Lib lib, Form.Queue args)
    {
        if (Find(Name, lib, Loc) is Value v)
        {
            v.EmitId(Loc, vm, lib, args);
        }
        else
        {
            throw new EmitError(Loc, $"Unknown id: {Name}");
        }
    }

    public override void EmitCall(VM vm, Lib lib, Form.Queue args)
    {
        if (Find(Name, lib, Loc) is Value v)
        {
            v.EmitCall(Loc, vm, lib, args);
        }
        else
        {
            throw new EmitError(Loc, $"Unknown id: {Name}");
        }
    }

    public override string ToString()
    {
        return Name;
    }
}