
namespace Sharpl;

using System.Drawing;
using System.Text;

public class Term
{
    private readonly StringBuilder buffer = new StringBuilder();

    public void Flush()
    {
        Console.Write(buffer.ToString());
        buffer.Clear();
    }
    private void CSI(params object[] args)
    {
        buffer.Append((char)27);
        buffer.Append('[');

        foreach (var a in args)
        {
            buffer.Append(a);
        }
    }

    public void Clear()
    {
        CSI(2, 'J');
    }

    public int Height { get => Console.BufferHeight; }

    
    public void Reset() {
        CSI("0m");
    }

    public void MoveTo(Point pos)
    {
        CSI(pos.Y, ';', pos.X, 'H');
    }

    public void SetBg(Color color)
    {
        CSI("48;2;", color.R, ';', color.G, ';', color.B, 'm');
    }

    public void SetFg(Color color)
    {
        CSI("38;2;", color.R, ';', color.G, ';', color.B, 'm');
    }

    public int Width { get => Console.BufferWidth; }

    public void Write(object value) {
        buffer.Append(value);
    }

    public void WriteLine(object value) {
        Write($"{value}\n");
    }
}