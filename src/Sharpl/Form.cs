using System.Collections;
using System.Text;
using Sharpl.Ops;

namespace Sharpl;

public abstract class Form : Emitter
{
    public readonly Loc Loc;

    protected Form(Loc loc)
    {
        Loc = loc;
    }

    public virtual void CollectIds(HashSet<string> result) { }

    public abstract void Emit(VM vm, Queue args);

    public virtual void EmitCall(VM vm, Queue args)
    {
        var arity = args.Count;
        args.Emit(vm, new Queue());
        Emit(vm, new Queue());
        vm.Emit(CallStack.Make(Loc, arity, args.IsSplat, vm.NextRegisterIndex));
    }

    public abstract bool Equals(Form other);
    public virtual Value? GetValue(VM vm) => null;
    public virtual bool IsSplat => false;
    public virtual Form Quote(Loc loc, VM vm) => this;
    public virtual Form Unquote(Loc loc, VM vm) => this;


    public class Queue : Emitter, IEnumerable<Form>
    {
        private LinkedList<Form> items = new LinkedList<Form>();

        public Queue() : this([]) { }

        public Queue(Form[] items)
        {
            foreach (var it in items) { Push(it); }
        }

        public HashSet<string> CollectIds()
        {
            var res = new HashSet<string>();
            foreach (var it in items) { it.CollectIds(res); }
            return res;
        }

        public void Clear() => items.Clear();
        public int Count => items.Count;

        public void Emit(VM vm, Queue args)
        {
            while (Count > 0)
            {
                if (TryPop() is Form v) { v.Emit(vm, this); }
            }
        }

        public void Emit(VM vm) => Emit(vm, new Form.Queue());

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

        public Form? Peek() => (items.First?.Value is Form f) ? f : null;

        public Form? TryPop()
        {
            if (items.First?.Value is Form f)
            {
                items.RemoveFirst();
                return f;
            }

            return null;
        }

#pragma warning disable CS8603
        public Form Pop() =>  TryPop();
#pragma warning restore CS8603

        public Form? TryPopLast()
        {
            if (items.Last?.Value is Form f)
            {
                items.RemoveLast();
                return f;
            }

            return null;
        }

#pragma warning disable CS8603
        public Form PopLast() => TryPopLast();
#pragma warning restore CS8603
 
        public void Push(Form form) => items.AddLast(form);
        public void PushFirst(Form form) => items.AddFirst(form);

#pragma warning disable CS8602
        public bool IsSplat => items.FirstOrDefault(f => f.IsSplat, null) != null;
#pragma warning restore CS8602

        public override string ToString()
        {
            var res = new StringBuilder();
            var i = 0;

            foreach (var f in items)
            {
                if (i > 0) { res.Append(' ');}
                res.Append(f.ToString());
                i++;
            }

            return res.ToString();
        }

        public IEnumerator<Form> GetEnumerator() => items.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}