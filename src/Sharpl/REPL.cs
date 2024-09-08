using System.Drawing;
using System.Text;

namespace Sharpl;

public class REPL
{
    public void Run(VM vm)
    {
        vm.Term
            .SetFg(Color.FromArgb(255, 252, 173, 3))
            .Write($"sharpl v{VM.VERSION}\n\n")
            .Reset();

        var buffer = new StringBuilder();
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
                try
                {
                    vm.Term
                        .SetFg(Color.FromArgb(255, 0, 255, 0))
                        .WriteLine(Eval(vm, buffer.ToString(), ref loc).Dump(vm))
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

    public virtual Value Eval(VM vm, string input, ref Loc loc)
    {
        var startPC = vm.EmitPC;
        vm.ReadForms(new Source(new StringReader(input)), ref loc).Emit(vm);
        vm.Emit(Ops.Stop.Make());
        return vm.Eval(startPC) ?? Value.Nil;
    }
}