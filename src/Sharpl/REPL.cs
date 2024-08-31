using System.Drawing;
using System.Text;

namespace Sharpl;

public static class REPL
{
    public static void Run(VM vm)
    {
        vm.Term
            .SetFg(Color.FromArgb(255, 252, 173, 3))
            .Write($"sharpl v{VM.VERSION}\n\n")
            .Reset();

        var buffer = new StringBuilder();
        var stack = new Stack();
        var loc = new Loc("repl");
        var bufferLines = 0;

        while (true)
        {
            vm.Term
                .SetFg(Color.FromArgb(255, 128, 128, 128))
                .Write($"{loc.Line + bufferLines,4} ")
                .Reset()
                .Flush();

            var line = Console.In.ReadLine();

            if (line is null)
            {
                break;
            }

            if (line == "")
            {
                var startPC = vm.EmitPC;

                try
                {
                    vm.ReadForms(new StringReader(buffer.ToString()), ref loc).Emit(vm);
                    vm.Emit(Ops.Stop.Make());
                    vm.Eval(startPC, stack);

                    vm.Term
                        .SetFg(Color.FromArgb(255, 0, 255, 0))
                        .WriteLine((stack is [] ? Value.Nil : stack.Pop()).Dump(vm))
                        .Reset();
                }
                catch (Exception e)
                {
                    vm.Term
                        .SetFg(Color.FromArgb(255, 255, 0, 0))
                        .WriteLine(e)
                        .Reset();
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
}