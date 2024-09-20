using System.Collections;
using Sharpl.Ops;

namespace Sharpl;

public abstract class Form(Loc loc)
{
    public readonly Loc Loc = loc;

    public virtual void CollectIds(HashSet<string> result) { }

    public T Cast<T>() where T : Form
    {
        if (this is T result) { return result; }
        throw new EvalError($"Type mismatch: {this}", Loc);
    }

    public abstract string Dump(VM vm);
    public virtual void Emit(VM vm, Queue args) { }

    public virtual void EmitCall(VM vm, Queue args)
    {
        var arity = args.Count;
        args.Emit(vm);
        vm.Emit(this);
        vm.Emit(CallStack.Make(Loc, arity, args.IsSplat, vm.NextRegisterIndex));
    }

    public abstract bool Equals(Form other);

    public virtual bool Expand(VM vm, Queue args)
    {
        args.Push(this);
        return false;
    }

    public virtual Value? GetValue(VM vm) => null;
    public virtual bool IsSplat => false;
    public virtual Form Quote(Loc loc, VM vm) => this;
    public virtual Form Unquote(Loc loc, VM vm) => this;

    public class Queue: IEnumerable<Form>
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

        public void Emit(VM vm)
        {
            Expand(vm);

            while (Count > 0)
            {
                if (TryPop() is Form v) { v.Emit(vm, this); }
            }
        }

        public void Expand(VM vm)
        {
            var done = false;
            
            while (!done)
            {
                var input = new Queue([.. items]);
                items.Clear();
                done = true;

                while (input.Count > 0) { 
                    if (input.Pop().Expand(vm, this)) { done = false; }; 
                }
            }
        }

        public bool Empty => items.Count == 0;

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

        // TODO: Idealy, this needs to match List.TryPop for consistency and to
        // avoid accidental expensive type tests where null checks were supposed to go.
        public Form? TryPop()
        {
            var f = items.First?.Value;
            if (f != null)
            {
                items.RemoveFirst();
                return f;
            }

            return null;
        }

        public Form Pop() => TryPop() ?? throw new InvalidOperationException("There is no first element");

        public Form? TryPopLast()
        {
            var f = items.Last?.Value;
            if (f != null)
            {
                items.RemoveLast();
                return f;
            }

            return null;
        }

        public Form PopLast() => TryPopLast() ?? throw new InvalidOperationException("There is no last element");

        public void Push(Form form) => items.AddLast(form);
        public void PushFirst(Form form) => items.AddFirst(form);

        public bool IsSplat => items.Any(f => f.IsSplat);

        public string Dump(VM vm) => string.Join(' ', items.Select(it => it.Dump(vm)));

        public IEnumerator<Form> GetEnumerator() => items.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}