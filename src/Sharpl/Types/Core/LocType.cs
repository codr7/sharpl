namespace Sharpl.Types.Core;

public class LocType(string name, AnyType[] parents) : ComparableType<Loc>(name, parents)
{ }