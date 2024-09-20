namespace Sharpl;

public readonly record struct Source(TextReader Reader)
{
    public static char? ToChar(int c) => (c == -1) ? null : Convert.ToChar(c);
    public char? Peek() => (buffer.Count == 0) ? ToChar(Reader.Peek()) : buffer.Peek();
    public char? Read() => (buffer.Count == 0) ? ToChar(Reader.Read()) : buffer.Pop();
    public void Unread(char c) => buffer.Push(c);
    private readonly List<char> buffer = new List<char>();
}

public interface Reader
{
    bool Read(Source source, VM vm, Form.Queue forms, ref Loc loc);
}