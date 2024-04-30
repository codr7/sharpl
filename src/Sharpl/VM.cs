namespace Sharpl;

using System.Text;

using PC = int;
using Stack = ArrayStack<Value>;

public class VM
{
    public static readonly int VERSION = 1;

    public readonly Libs.Core CoreLib = new Libs.Core();
    public readonly Lib UserLib = new Lib("user", null);

    public PC PC = 0;

    private ArrayStack<Op> code = new ArrayStack<Op>(1024);

    public VM()
    {
        UserLib.Bind("core", Value.Make(Libs.Core.Lib, CoreLib));
        UserLib.Bind("user", Value.Make(Libs.Core.Lib, UserLib));
    }

    public PC Emit(Op op)
    {
        var result = code.Len;
        code.Push(op);
        return result;
    }

    public PC EmitPC
    {
        get { return code.Len; }
    }

    public void Eval(PC startPC, Stack stack)
    {
        PC = startPC;

        while (true)
        {
            var op = code[PC];

            switch (op.Type)
            {
                case Op.T.Push:
                    var pushOp = (Ops.Push)op.Data;
                    stack.Push(pushOp.Value);
                    PC++;
                    break;
                case Op.T.Stop:
                    PC++;
                    return;
            }
        }
    }

    public void REPL()
    {
        Console.Write($"Sharpl v{VERSION} - may the src be with you\n\n");
        var buffer = new StringBuilder();
        var stack = new Stack(32);

        while (true)
        {
            Console.Write("> ");
            var line = Console.In.ReadLine();

            if (line is null)
            {
                break;
            }

            if (line == "")
            {
                var startPC = EmitPC;
                Emit(Ops.Stop.Make());
                Eval(startPC, stack);
                Console.WriteLine(stack.Empty ? Value.Nil : stack.Pop());
                Console.WriteLine("");
            }

            buffer.Append(line);
            buffer.AppendLine();
        }
    }
};