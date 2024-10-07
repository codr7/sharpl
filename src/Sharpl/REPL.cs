using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sharpl;

public class REPL
{
    private List<(Sym, Value)> restarts = new List<(Sym, Value)>();

    public void Run(VM vm)
    {
        vm.Term
            .Write($"sharpl v{VM.VERSION}\n\n")
            .Reset();

        var buffer = new StringBuilder();
        var loc = new Loc("repl");
        var bufferLines = 0;

        while (true)
        {
            vm.Term
                .Write($"{loc.Line + bufferLines,4} ")
                .Reset()
                .Flush();

            var line = Console.In.ReadLine();
            if (line is null) { break; }

            if (line == "")
            {
                try
                {
                    var v = Eval(vm, buffer.ToString(), ref loc);
                    vm.Term.WriteLine(v.Dump(vm));
                }
                catch (Exception e)
                {
                    vm.Term.WriteLine(e);
                }
                finally
                {
                    buffer.Clear();
                    bufferLines = 0;
                }

                vm.Term.Write("\n");
            }
            else
            {
                buffer.Append(line);
                buffer.AppendLine();
                bufferLines++;
            }
        }
    }

    public virtual Value Eval(VM vm, string input, ref Loc loc)
    {
        var startPC = vm.EmitPC;
        var fs = vm.ReadForms(new Source(new StringReader(input)), ref loc);
        var _loc = loc;

        vm.DoEnv(vm.Env, loc, () =>
        {
            vm.Env.Bind("LOC", Value.Make(Libs.Core.Binding, vm.CoreLib.LOC));
            var end = new Label();
            vm.Emit(Ops.Try.Make(vm.Restarts, vm.NextRegisterIndex, end, vm.CoreLib.LOC, _loc));
            fs.Emit(vm);
            vm.Emit(Ops.EndFrame.Make(_loc));
            end.PC = vm.EmitPC;
            vm.Emit(Ops.Stop.Make());
        });

        return vm.Eval(startPC) ?? Value._;
    }
}