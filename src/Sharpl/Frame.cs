namespace Sharpl;

public class Frame
{
    public readonly int RegisterIndex;
    public readonly int RegisterCount;
    public readonly int DeferOffset;
    public readonly int RestartOffset;

    public Frame(int registerIndex, int registerCount, int deferOffset, int restartOffset)
    {
        RegisterIndex = registerIndex;
        RegisterCount = registerCount;
        DeferOffset = deferOffset;
        RestartOffset = restartOffset;
    }
}

