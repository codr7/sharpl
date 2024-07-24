using System.Collections;
using Sharpl;

public interface Iter : IEnumerable<Value>
{
    public struct E : IEnumerator<Value>
    {
#pragma warning disable CS8629
        public Value Current => (Value)current;
        object IEnumerator.Current => (Value)current;
#pragma warning restore CS8629

        private Value? current;
        public readonly Iter Source;

        public E(Iter source)
        {
            Source = source;
        }

        public void Dispose() { }

        public bool MoveNext()
        {
            current = Source.Next();
            return current != null;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    Value? Next();
}

public abstract class BasicIter : Iter
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Iter.E(this);
    }

    public IEnumerator<Value> GetEnumerator()
    {
        return new Iter.E(this);
    }

    public abstract Value? Next();
}