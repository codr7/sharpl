using Sharpl.Libs;
using System.Data;
using System.Text;

namespace Sharpl;

public class UserMethod
{
    public record struct Arg(string Name, int RegisterIndex = -1, bool Unzip = false) { }

    public static void Bind(VM vm, Form f, List<Arg> fas, bool unzip = false)
    {
        switch (f)
        {
            case Forms.Id id:
                {
                    var r = vm.AllocRegister();
                    vm.Env.Bind(id.Name, Value.Make(Core.Binding, new Register(0, r)));
                    fas.Add(new Arg(id.Name, r, unzip));
                    break;
                }
            case Forms.Nil:
                fas.Add(new Arg("_", -1, unzip));
                break;
            case Forms.Pair p:
                Bind(vm, p.Left, fas, unzip = true);
                Bind(vm, p.Right, fas);
                break;
        }
    }

    public readonly Arg[] Args;
    public readonly int MinArgCount;
    public readonly (string, int, Register)[] Closure;
    public readonly Dictionary<int, Value> ClosureValues = new Dictionary<int, Value>();
    public readonly Loc Loc;
    public readonly string Name;
    public int? StartPC;
    public int? EndPC;
    public readonly bool Vararg;

    public UserMethod(VM vm, string name, string[] ids, Arg[] args, bool vararg, Loc loc)
    {
        Loc = loc;
        Name = name;

        Closure = ids.AsEnumerable().Select<string, (string, int, Register)>(id =>
        {
#pragma warning disable CS8629
            var b = ((Value)vm.Env[id]).CastUnbox(Core.Binding);
#pragma warning restore CS8629
            var r = vm.AllocRegister();
            vm.Env[id] = Value.Make(Core.Binding, new Register(0, r));
            return (id, r, b);
        }).ToArray();

        Args = args;
        MinArgCount = args[0..(vararg ? ^1 : ^0)].Count((a) => !a.Name.EndsWith('?') && !a.Unzip);
        Vararg = vararg;
    }

    public void BindArgs(VM vm, Value?[] argMask, int arity, Stack stack)
    {
        for (var i = Args.Length - 1; i >= 0; i--)
        {
            if (i >= argMask.Length) { break; }
            var ar = Args[i].RegisterIndex;

            if (Vararg && i == Args.Length - 1)
            {
                var n = arity - Args.Length + 1;
                var vs = new Value[n];

                for (var j = n - 1; j >= 0; j--)
                {
                    vs[j] = (argMask.Length > i + j && argMask[i + j] is Value v) ? v : stack.Pop();
                }

                if (ar != -1) { vm.SetRegister(0, ar, Value.Make(Core.Array, vs)); }
            }
            else
            {
                if (argMask[i] is Value v)
                {
                    if (v.Type == Core.Binding)
                    {
                        var r = v.CastUnbox(Core.Binding);
                        if (r.FrameOffset != 0 || r.Index != ar) { vm.SetRegister(0, ar, vm.Get(r)); }
                    }
                    else if (ar != -1) { vm.SetRegister(0, ar, v.Copy()); }
                }
                else {
                    v = stack.Pop();
                    if (ar != -1) { vm.SetRegister(0, ar, v); } 
                }
            }
        }

        foreach (var (r, v) in ClosureValues) { vm.SetRegister(0, r, v); }
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append($"(^{Name} [");

        for (var i = 0; i < Args.Length; i++)
        {
            if (i > 0) { result.Append(' '); }
            result.Append(Args[i].Name);
        }

        if (Vararg) { result.Append('*'); }
        result.Append("])");
        return result.ToString();
    }
}

