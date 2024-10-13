namespace Sharpl;

using System.Collections;

public abstract class Iter
{
    public virtual string Dump(VM vm) => $"{this}";
    public abstract bool Next(VM vm, Register result, Loc loc);
}