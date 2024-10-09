using Sharpl.Iters.IO;
using Sharpl.Types.IO;

namespace Sharpl.Libs;

public class IO : Lib
{
    public static readonly InputStreamType InputStream = new InputStreamType("InputStream", [Core.Any]);

    public IO(VM vm) : base("io", null, [])
    {
        BindType(InputStream);

        Bind("IN", Value.Make(IO.InputStream, Console.In));

        BindMacro("do-read", ["path"], (vm, target, args, result, loc) =>
        {
            if (args.TryPop() is Form afs)
            {
                if (afs is Forms.Array af)
                {
                    if (af.Items.Length < 2) throw new EmitError("Missing args", loc);

                    vm.DoEnv(new Env(vm.Env, args.CollectIds()), loc, () =>
                    {
                        var reg = new Register(0, vm.AllocRegister());
                        var a0 = af.Items[0];

                        if (a0 is Forms.Id id) vm.Env.Bind(id.Name, Value.Make(Core.Binding, reg));
                        else throw new EmitError("Expected identifier: {a0}", a0.Loc);

                        var startPC = vm.EmitPC;
                        var p = vm.Eval(af.Items[1]).Cast(Core.String);
                        vm.Emit(Ops.OpenInputStream.Make(p, reg, loc));
                        args.Emit(vm, result);
                    });
                }
                else throw new EmitError("Invalid args", loc);
            }
            else throw new EmitError("Missing args", loc);
        });

        BindMethod("lines", ["in"], (vm, target, arity, result, loc) =>
        {
            var s = vm.GetRegister(0, 0).Cast(InputStream);
            vm.Set(result, Value.Make(Core.Iter, new StreamLines(s)));
        });
    }
}