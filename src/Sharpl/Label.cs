namespace Sharpl;

using PC = int;

public class Label
{
    public PC PC
    {
#pragma warning disable CS8629
        get => (PC)pc;
#pragma warning restore CS8629
        set => pc = value;
    }

    public Label(PC? pc = null)
    {
        if (pc != null) { PC = (PC)pc; }
    }

    public override string ToString() => $"@{PC}";

    private PC? pc;
}