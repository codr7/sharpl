namespace Sharpl.Libs;

public class Time : Lib
{
    public Time() : base("time", null, [])
    {
        BindMethod("now", [], (loc, target, vm, stack, arity) =>
            stack.Push(Core.Timestamp, DateTime.Now));

        BindMethod("today", [], (loc, target, vm, stack, arity) =>
            stack.Push(Core.Timestamp, DateTime.Now.Date));
    }
}