
namespace Sharpl;

using System.Drawing;
using System.Text;

public class Term
{
    private readonly StringBuilder Buffer = new StringBuilder();

    public void Flush()
    {
        Console.Write(Buffer.ToString());
        Buffer.Clear();
    }
    private void CSI(params object[] args)
    {
        Buffer.Append((char)27);
        Buffer.Append('[');

        foreach (var a in args)
        {
            Buffer.Append(a);
        }
    }

    public void Clear()
    {
        CSI(2, 'J');
    }

    public int Height { get => Console.BufferHeight; }

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
}