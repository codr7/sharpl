namespace Sharpl;

public abstract class Form
{
    public readonly Loc Loc;

    protected Form(Loc loc)
    {
        Loc = loc;
    }

    public abstract void Emit(VM vm, Lib lib, Form.Queue args);

    public virtual void EmitCall(VM vm, Lib lib, Form.Queue args)
    {
        args.Emit(vm, lib);
        Emit(vm, lib, new Form.Queue());
        vm.Emit(Ops.CallIndirect.Make(Loc, args.Count));
    }

    public class Queue
    {
        private LinkedList<Form> items = new LinkedList<Form>();

        public int Count { get { return items.Count; } }


        public void Emit(VM vm, Lib lib)
        {
            while (Count > 0)
            {
                if (Pop() is Form v)
                {
                    v.Emit(vm, lib, this);
                }
            }
        }

        public Form? Pop()
        {
            if (items.First?.Value is Form f)
            {
                return f;
            }

            return null;
        }

        public void Push(Form form)
        {
            items.Append(form);
        }
    }
}