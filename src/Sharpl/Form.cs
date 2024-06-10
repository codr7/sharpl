using System.Reflection.Metadata.Ecma335;

namespace Sharpl;

using System.Text;

public abstract class Form
{
    public readonly Loc Loc;

    protected Form(Loc loc)
    {
        Loc = loc;
    }

    public abstract void Emit(VM vm, Form.Queue args);

    public virtual void EmitCall(VM vm, Form.Queue args)
    {
        var arity = args.Count;
        args.Emit(vm);
        Emit(vm, new Form.Queue());
        vm.Emit(Ops.CallIndirect.Make(Loc, arity));
    }

    public class Queue
    {
        private LinkedList<Form> items = new LinkedList<Form>();

        public Queue() : this([]) { }

        public Queue(Form[] items)
        {
            foreach (var it in items)
            {
                Push(it);
            }
        }

        public int Count { get { return items.Count; } }


        public void Emit(VM vm)
        {
            while (Count > 0)
            {
                if (Pop() is Form v)
                {
                    v.Emit(vm, this);
                }
            }
        }

        public bool Empty { get => items.Count == 0; }

        public Form[] Items
        {
            get
            {
                var res = new Form[items.Count];
                var i = 0;

                foreach (var f in items)
                {
                    res[i] = f;
                    i++;
                }

                return res;
            }
        }

        public Form? Peek()
        {
            if (items.First?.Value is Form f)
            {
                return f;
            }

            return null;
        }

        public Form? Pop()
        {
            if (items.First?.Value is Form f)
            {
                items.RemoveFirst();
                return f;
            }

            return null;
        }

        public Form? PopLast()
        {
            if (items.Last?.Value is Form f)
            {
                items.RemoveLast();
                return f;
            }

            return null;
        }

        public void Push(Form form)
        {
            items.AddLast(form);
        }

        public override string ToString()
        {
            var res = new StringBuilder();
            var i = 0;

            foreach (var f in items)
            {
                if (i > 0)
                {
                    res.Append(' ');
                }

                res.Append(f.ToString());
                i++;
            }

            return res.ToString();
        }
    }
}