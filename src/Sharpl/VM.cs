using System.Buffers;
using System.Runtime.Intrinsics.X86;

namespace Sharpl
{
    using PC = int;

    public class VM
    {
        private Stack<Operation> code = new Stack<Operation>(1024);
        private PC pc;

        public PC PC
        {
            get { return pc; }
            set { pc = value; }
        }

        public PC Emit(Operation op)
        {
            var result = code.Len;
            code.Push(op);
            return result;
        }

        public void Eval(PC startPC, Stack<Value> stack)
        {
            pc = startPC;

            while (true)
            {
                var op = code[pc];

                switch (op.Type)
                {
                    case Operation.T.Push:
                        var pushOp = (Operations.Push)op.Data;
                        stack.Push(pushOp.Value);
                        pc++;
                        break;
                    case Operation.T.Stop:
                        pc++;
                        return;
                }
            }
        }
    };
}