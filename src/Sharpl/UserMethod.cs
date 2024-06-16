using System.Data;
using System.Runtime.Intrinsics.X86;
using System.Text;
using Sharpl.Libs;

namespace Sharpl;

public class UserMethod
{
    public readonly (string, int)[] Args;
    public readonly Dictionary<Binding, (int, Value?)> Closure = new Dictionary<Binding, (int, Value?)>();
    public readonly Loc Loc;
    public readonly string Name;
    public int? StartPC;

    public UserMethod(Loc loc, VM vm, string name, string[] closure, (string, int)[] args)
    {
        Loc = loc;
        Name = name;

        Closure = closure.Select<string, (Binding, (int, Value?))>((id) =>
        {
#pragma warning disable CS8629
            var b = ((Value)vm.Env[id]).Cast(Core.Binding);
#pragma warning restore CS8629
            var r = vm.AllocRegister();
            vm.Env[id] = Value.Make(Core.Binding, new Binding(0, r));
            return (b, (r, null));
        }).ToDictionary();

        Args = args;
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append($"(Method {Name} [");

        for (var i = 0; i < Args.Length; i++)
        {
            if (i > 0)
            {
                result.Append(' ');
            }

            result.Append(Args[i].Item1);
        }

        result.Append("])");
        return result.ToString();
    }
}

