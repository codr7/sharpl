namespace Sharpl.Libs;

using Sharpl.Iters.IO;
using Sharpl.Types.IO;

using System.Text;

public class IO : Lib
{
    public static readonly StreamReaderType StreamReader = new StreamReaderType("StreamReader");

    public IO() : base("io", null, [])
    {
        BindType(StreamReader);

        Bind("IN", Value.Make(IO.StreamReader, Console.In));

        BindMacro("do-read", ["path"], (loc, target, vm, args) =>
         {
            if (args.Pop() is Form f) {                
                var startPC = vm.EmitPC;
                f.Emit(vm, args);
                var reg = vm.AllocRegister();
                vm.Emit(Ops.OpenStreamReader.Make(loc, 0, reg));
                args.Emit(vm);
                vm.Emit(Ops.Stop.Make());
                var stack = new Stack(vm.Config.MaxStackSize);
                
                try {
                    vm.Eval(startPC, stack);
                } finally {
                    vm.GetRegister(0, reg).Cast(IO.StreamReader).Close();
                }
            }
         });

        BindMethod("lines", ["in"], (loc, target, vm, stack, arity) =>
        {
            var s = stack.Pop().Cast(StreamReader);
            stack.Push(Value.Make(Core.Iter, new StreamLines(s)));
        });
    }
}