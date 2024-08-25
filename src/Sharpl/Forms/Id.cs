using Sharpl.Libs;

namespace Sharpl.Forms;

public class Id : Form
{
    public static Value GetId(string name, Env env, Loc loc)
    {
        while (true)
        {
            var i = name.IndexOf('/');
            if (i <= 0) { break; }
            var ln = name.Substring(0, i);
            var lv = env[ln];
            if (lv is null) { throw new EmitError(loc, $"Unknown id: {ln}"); }
            env = ((Value)lv).Cast(loc, Core.Lib);
            name = name.Substring(i + 1);
        }

        var v = env[name];
        if (v is null) { throw new EmitError(loc, $"Unknown id: {name}"); }
        return (Value)v;
    }

    public readonly string Name;

    public Id(Loc loc, string name) : base(loc)
    {
        Name = name;
    }

    public override void CollectIds(HashSet<string> result) =>
        result.Add(Name);

    public override void Emit(VM vm, Form.Queue args)
    {
        if (GetId(Name, vm.Env, Loc) is Value v) { v.EmitId(Loc, vm, args); }
        else { throw new EmitError(Loc, $"Unknown id: {Name}"); }
    }

    public override void EmitCall(VM vm, Form.Queue args)
    {
        if (GetId(Name, vm.Env, Loc) is Value v) { v.EmitCall(Loc, vm, args); }
        else { throw new EmitError(Loc, $"Unknown id: {Name}"); }
    }

    public override bool Equals(Form other) =>
        (other is Id f) ? f.Name.Equals(Name) : false;

    public override Value? GetValue(VM vm) => GetId(Name, vm.Env, Loc);

    public override Form Quote(Loc loc, VM vm) =>
        new Literal(loc, Value.Make(Core.Sym, vm.Intern(Name)));

    public override string ToString() => Name;
    public override Form Unquote(Loc loc, VM vm) => GetId(Name, vm.Env, loc).Unquote(loc, vm);
}