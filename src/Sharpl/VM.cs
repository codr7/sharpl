namespace Sharpl;

using System.Text;

using PC = int;

public class VM
{
    public static readonly int VERSION = 1;

    public readonly Libs.Core CoreLib = new Libs.Core();
    public readonly Lib UserLib = new Lib("user", null);

    public PC PC = 0;

    private ArrayStack<Call> calls = new ArrayStack<Call>(32);
    private ArrayStack<Op> code = new ArrayStack<Op>(1024);
    private List<Label> labels = new List<Label>();    

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

    public Label Label(PC pc = -1) {
        var l = new Label(pc);
        labels.Append(l);
        return l;
    }

    public PC EmitPC
    {
        get { return code.Len; }
    }

    public void Eval(PC startPC, S stack)
    {
        PC = startPC;

        while (true)
        {
            var op = code[PC];

            switch (op.Type)
            {
                case Op.T.CallIndirect: {
                    var target = stack.Pop();
                    var callOp = (Ops.CallIndirect)op.Data;
                    var recursive = !calls.Empty && calls.Peek().Target.Equals(target);
                    PC++;
                    target.Call(callOp.Loc, this, stack, callOp.Arity, recursive);
                    break;
                }
                case Op.T.CallPrim: {
                    var callOp = (Ops.CallPrim)op.Data;
                    PC++;
                    callOp.Target.Call(callOp.Loc, this, stack, callOp.Arity, false);
                    break;
                }
                case Op.T.Goto: {
                    var gotoOp = (Ops.Goto)op.Data;
                    PC = gotoOp.Target.PC;
                    break;
                }
                case Op.T.Push: {
                    var pushOp = (Ops.Push)op.Data;
                    stack.Push(pushOp.Value.Copy());
                    PC++;
                    break;
                }
                case Op.T.Stop: {
                    PC++;
                    return;
                }
            }
        }
    }

    public void REPL()
    {
        Console.Write($"Sharpl v{VERSION} - may the src be with you\n\n");
        var buffer = new StringBuilder();
        var stack = new S(32);

        while (true)
        {
            Console.Write("  ");
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