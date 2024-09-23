namespace Sharpl;

using System.Collections;

public abstract class Iter
{
    public virtual string Dump(VM vm) => $"{this}";
    public abstract Value? Next(VM vm, Loc loc);
}