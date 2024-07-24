namespace Sharpl.Forms;

using Sharpl.Libs;

public class Id : Form
{
    public static Value? GetId(string name, Env env, Loc loc)
    {
        while (true)
        {
            var i = name.IndexOf('/');
            if (i <= 0) { break; }
            var ln = name.Substring(0, i);
            var lv = env[ln];

            if (lv is null)
            {
                throw new EmitError(loc, $"Unknown id: {ln}");
            }

            env = ((Value)lv).TryCast(loc, Core.Lib);
            name = name.Substring(i + 1);
        }

        return env[name];
    }

    public readonly string Name;

    public Id(Loc loc, string name) : base(loc)
    {
        Name = name;
    }

    public override void CollectIds(HashSet<string> result)
    {
        result.Add(Name);
    }

    public override void Emit(VM vm, Form.Queue args, int quoted)
    {
        if (quoted == 0)
        {
            if (GetId(Name, vm.Env, Loc) is Value v)
            {
                v.EmitId(Loc, vm, args);
            }
            else
            {
                throw new EmitError(Loc, $"Unknown id: {Name}");
            }
        }
        else
        {
            vm.Emit(Ops.Push.Make(Value.Make(Core.Symbol, vm.GetSymbol(Name))));
        }
    }

    public override void EmitCall(VM vm, Form.Queue args)
    {
        if (GetId(Name, vm.Env, Loc) is Value v)
        {
            v.EmitCall(Loc, vm, args);
        }
        else
        {
            throw new EmitError(Loc, $"Unknown id: {Name}");
        }
    }

    public override Value? GetValue(VM vm)
    {
        return GetId(Name, vm.Env, Loc);
    }

    public override string ToString()
    {
        return Name;
    }
}