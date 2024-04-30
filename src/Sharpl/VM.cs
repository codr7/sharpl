using System.Buffers;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;

namespace Sharpl
{
    using PC = int;

    public class VM
    {
        public readonly Libs.Core CoreLib = new Libs.Core();
        public readonly Lib UserLib = new Lib("user", null);
 
        private Stack<Op> code = new Stack<Op>(1024);
        private PC pc;

        public VM() {
            UserLib.Bind("core", Value.Make(CoreLib.Lib, CoreLib));
            UserLib.Bind("user", Value.Make(CoreLib.Lib, UserLib));
        }

        public PC Emit(Op op)
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
    };
}