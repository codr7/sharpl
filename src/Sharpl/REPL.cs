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
        var stack = new Stack();

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
                    var v = Eval(vm, buffer.ToString(), stack, ref loc);
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

    public virtual Value Eval(VM vm, string input, Stack stack, ref Loc loc)
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

        Value result = Value._;

        try
        {
            vm.Eval(startPC, stack);
            if (stack.TryPop(out var rv)) result = rv;
        }
        catch (EvalError e)
        {
            vm.Term.WriteLine(e);
            e.AddRestarts(vm);
            vm.AddRestart(vm.Intern("stop"), 0, (vm, stack, target, arity, loc) => { vm.PC = vm.EmitPC - 1; });
            var rs = vm.Restarts;
            for (var i = 0; i < rs.Length; i++) { vm.Term.WriteLine($"{i + 1} {rs[i].Item1.Cast(Libs.Core.Sym).Name}"); }
            var n = int.Parse((string)vm.Term.Ask($"Pick an alternative (1-{rs.Length}) and press ⏎: ")!);
            rs[n-1].Item2.Call(vm, stack, 0, vm.NextRegisterIndex, false, loc);
            if (stack.TryPop(out var rv)) result = rv;
        }

        return result;
    }
}