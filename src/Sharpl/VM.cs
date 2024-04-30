namespace Sharpl;

using System.Text;

using PC = int;
using Stack = ArrayStack<Value>;

public class VM
{
    public readonly Libs.Core CoreLib = new Libs.Core();
    public readonly Lib UserLib = new Lib("user", null);

    private ArrayStack<Op> code = new ArrayStack<Op>(1024);
    private PC pc;

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
        pc = startPC;

        while (true)
        {
            var op = code[pc];

            switch (op.Type)
            {
                case Op.T.Push:
                    var pushOp = (Ops.Push)op.Data;
                    stack.Push(pushOp.Value);
                    pc++;
                    break;
                case Op.T.Stop:
                    pc++;
                    return;
            }
        }
    }
    public PC PC
    {
        get { return pc; }
        set { pc = value; }
    }

    public void REPL()
    {
        var buffer = new StringBuilder();
        var stack = new Stack(32);

        while (true)
        {
            var line = Console.In.ReadLine();
            
            if (line is null) {
                break;
            }

            if (line == "") {
                var startPC = EmitPC;
                Eval(startPC, stack);
                Console.WriteLine(stack.Empty ? Value.Nil : stack.Pop());
            }

            buffer.Append(line);
            buffer.AppendLine();
        }
    }
};