namespace Sharpl;

using System.Collections;
using System.Text;
using Sharpl.Ops;

public abstract class Form : Emitter
{
    public readonly Loc Loc;

    protected Form(Loc loc)
    {
        Loc = loc;
    }

    public virtual void CollectIds(HashSet<string> result) { }

    public abstract void Emit(VM vm, Queue args, int quoted);

    public void Emit(VM vm, Queue args)
    {
        Emit(vm, args, 0);
    }

    public virtual void EmitCall(VM vm, Queue args)
    {
        var arity = args.Count;
        args.Emit(vm);
        Emit(vm, new Queue());
        vm.Emit(CallStack.Make(Loc, arity, args.IsSplat, vm.NextRegisterIndex));
    }

    public abstract bool Equals(Form other);

    public virtual Value? GetValue(VM vm)
    {
        return null;
    }

    public virtual bool IsSplat => false;

    public class Queue : Emitter, IEnumerable<Form>
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

        public HashSet<string> CollectIds()
        {
            var res = new HashSet<string>();

            foreach (var it in items)
            {
                it.CollectIds(res);
            }

            return res;
        }

        public void Clear()
        {
            items.Clear();
        }

        public int Count { get { return items.Count; } }


        public void Emit(VM vm, int quoted = 0)
        {
            while (Count > 0)
            {
                if (TryPop() is Form v)
                {
                    v.Emit(vm, this, quoted);
                }
            }
        }

        public void Emit(VM vm, Queue args)
        {
            Emit(vm);
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

        public Form? TryPop()
        {
            if (items.First?.Value is Form f)
            {
                items.RemoveFirst();
                return f;
            }

            return null;
        }

        public Form Pop()
        {
#pragma warning disable CS8603 // Possible null reference return.
            return TryPop();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public Form? TryPopLast()
        {
            if (items.Last?.Value is Form f)
            {
                items.RemoveLast();
                return f;
            }

            return null;
        }

        public Form PopLast()
        {
#pragma warning disable CS8603 // Possible null reference return.
            return TryPopLast();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public void Push(Form form)
        {
            items.AddLast(form);
        }

        public void PushFirst(Form form)
        {
            items.AddFirst(form);
        }

        public bool IsSplat
        {
            get
            {
                foreach (var f in items)
                {
                    if (f.IsSplat)
                    {
                        return true;
                    }
                }

                return false;
            }
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

        public IEnumerator<Form> GetEnumerator()
        {
            return items.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}