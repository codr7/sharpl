using Sharpl.Libs;

namespace Sharpl.Forms;

public class Id : Form
{
    public static Value? FindId(string name, Env env, Loc loc)
    {
        while (true)
        {
            var i = name.IndexOf('/');
            if (i <= 0) { break; }
            var ln = name.Substring(0, i);
            var lv = env[ln];
            if (lv is null) { return null; }
            env = ((Value)lv).Cast(Core.Lib, loc);
            name = name.Substring(i + 1);
        }

        return env[name];
    }

    public static Value GetId(string name, Env env, Loc loc)
    {
        if (FindId(name, env, loc) is Value v) { return v; }
        throw new EmitError($"Unknown id: {name}", loc);
    }

    public readonly string Name;

    public Id(string name, Loc loc) : base(loc)
    {
        Name = name;
    }

    public override void CollectIds(HashSet<string> result) =>
        result.Add(Name);

    public override void Emit(VM vm, Queue args)
    {
        if (GetId(Name, vm.Env, Loc) is Value v) { args.PushFirst(new Literal(v, Loc)); }
        else { throw new EmitError($"Unknown id: {Name}", Loc); }
    }

    public override void EmitCall(VM vm, Queue args)
    {
        if (GetId(Name, vm.Env, Loc) is Value v) { v.EmitCall(vm, args, Loc); }
        else { throw new EmitError($"Unknown id: {Name}", Loc); }
    }

    public override bool Equals(Form other) =>
        (other is Id f) ? f.Name.Equals(Name) : false;

    public override Value? GetValue(VM vm) => FindId(Name, vm.Env, Loc);

    public override Form Quote(VM vm, Loc loc) =>
        new Literal(Value.Make(Core.Sym, vm.Intern(Name)), loc);

    public override string Dump(VM vm) => Name;
    public override Form Unquote(VM vm, Loc loc) => GetId(Name, vm.Env, loc).Unquote(vm, loc);
}