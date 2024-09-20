using Sharpl.Iters.IO;
using Sharpl.Types.IO;

namespace Sharpl.Libs;

public class IO : Lib
{
    public static readonly InputStreamType InputStream = new InputStreamType("InputStream");

    public IO(VM vm) : base("io", null, [])
    {
        BindType(InputStream);

        Bind("IN", Value.Make(IO.InputStream, Console.In));

        BindMacro("do-read", ["path"], (loc, target, vm, args) =>
         {
             if (args.TryPop() is Form afs)
             {
                 if (afs is Forms.Array af)
                 {
                     if (af.Items.Length < 2)
                     {
                         throw new EmitError("Missing args", loc);
                     }

                     vm.DoEnv(new Env(vm.Env, args.CollectIds()), () =>
                     {
                         var reg = vm.AllocRegister();
                         var a0 = af.Items[0];

                         if (a0 is Forms.Id id)
                         {
                             vm.Env.Bind(id.Name, Value.Make(Core.Binding, new Register(0, reg)));
                         }
                         else
                         {
                             throw new EmitError("Expected identifier: {a0}", a0.Loc);
                         }

                         var startPC = vm.EmitPC;
                         vm.Emit(af.Items[1]);
                         vm.Emit(Ops.OpenInputStream.Make(loc, 0, reg));
                         args.Emit(vm);
                     });
                 }
                 else
                 {
                     throw new EmitError("Invalid args", loc);
                 }
             }
             else
             {
                 throw new EmitError("Missing args", loc);
             }
         });

        BindMethod("lines", ["in"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(InputStream);
            stack.Push(Value.Make(Core.Iter, new StreamLines(s)));
        });
    }

    protected override void OnInit(VM vm)
    {
        Import(vm.CoreLib);

        vm.Eval("""
          (^read-lines [path]
            (do-read [in path] 
              (lines in)))
        """);
    }
}