using System.Drawing;
using System.Text;

namespace Sharpl;

public class Term
{
    private readonly StringBuilder buffer = new StringBuilder();

    public void Flush()
    {
        Console.Write(buffer.ToString());
        buffer.Clear();
    }

    // TODO: Switch to .NET 9 and replace with
    // params ReadOnlySpan<object> args
    private void CSI(params object[] args)
    {
        buffer.Append((char)27);
        buffer.Append('[');
        foreach (var a in args) { buffer.Append(a); }
    }

    public void ClearLine() => CSI(0, 'K');
    public void ClearScreen() => CSI(2, 'J');
    public int Height { get => Console.BufferHeight; }


    public void MoveTo(int x, int? y = null)
    {
        if (y == null) { CSI(x, 'G'); }
        else { CSI(y, ';', x, 'H'); }
    }

    public void Reset() => CSI("0m");
    public void Restore() => CSI('u');
    public void Save() => CSI('s');

    public void SetRegion((int, int) min, (int, int) max) =>
        CSI(min.Item2, ';', max.Item2, ';', min.Item1, ';', max.Item1, 'r');

    public void SetRegion() => CSI('r');
    public void ScrollUp(int lines = 1) => CSI(lines, 'S');
    public void ScrollDown(int lines = 1) => CSI(lines, 'T');

    public void SetBg(Color color) =>
        CSI("48;2;", color.R, ';', color.G, ';', color.B, 'm');

    public void SetFg(Color color) =>
        CSI("38;2;", color.R, ';', color.G, ';', color.B, 'm');

    public int Width => Console.BufferWidth;
    public void Write(object value) => buffer.Append(value);
    public void WriteLine(object value) => Write($"{value}\n");
}