namespace Sharpl.Libs;

public class Time : Lib
{
    public Time() : base("time", null, [])
    {
        BindMethod("from-utc", ["t"], (loc, target, vm, stack, arity) =>
           stack.Push(Core.Timestamp, stack.Pop().CastUnbox(loc, Libs.Core.Timestamp).ToLocalTime()));

        BindMethod("now", [], (loc, target, vm, stack, arity) =>
              stack.Push(Core.Timestamp, DateTime.Now));

        BindMethod("today", [], (loc, target, vm, stack, arity) =>
            stack.Push(Core.Timestamp, DateTime.Now.Date));

        BindMethod("to-utc", ["t"], (loc, target, vm, stack, arity) =>
           stack.Push(Core.Timestamp, stack.Pop().CastUnbox(loc, Libs.Core.Timestamp).ToUniversalTime()));
    }
}