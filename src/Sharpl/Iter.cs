namespace Sharpl;

using System.Collections;

public interface Iter : IEnumerable<Value>
{
    public struct E : IEnumerator<Value>
    {
#pragma warning disable CS8629
        public Value Current => (Value)current;
        object IEnumerator.Current => (Value)current;
#pragma warning restore CS8629

        private Value? current;
        private readonly Iter source;

        public E(Iter source)
        {
            this.source = source;
        }

        public void Dispose() { }
        public bool MoveNext()
        {
            current = source.Next();
            return current != null;
        }

        public void Reset() => throw new NotImplementedException();
    }
    string Dump(VM vm);
    Value? Next();
}

public abstract class BasicIter : Iter
{
    IEnumerator IEnumerable.GetEnumerator() => new Iter.E(this);
    public IEnumerator<Value> GetEnumerator() => new Iter.E(this);
    public virtual string Dump(VM vm) => $"{this}";
    public abstract Value? Next();
}