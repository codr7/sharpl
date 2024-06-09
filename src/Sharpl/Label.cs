namespace Sharpl;

using PC = int;

public class Label
{
    public PC? PC;

    public Label(PC? pc = null)
    {
        PC = pc;
    }

    public override string ToString() {
        return $"{PC}";
    }
}