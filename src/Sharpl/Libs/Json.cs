namespace Sharpl.Libs;

public class Json : Lib
{
    public Json() : base("json", null, [])
    {
        BindMethod("decode", ["value"], (vm, stack, target, arity, loc) =>
        {
            var jsLoc = new Loc("json");
            var v = Sharpl.Json.ReadValue(vm, new StringReader(stack.Pop().Cast(Core.String)), ref jsLoc);
            stack.Push((v is null) ? Value._ : (Value)v);
        });

        BindMethod("encode", ["value"], (vm, stack, target, arity, loc) =>
            stack.Push(Core.String, stack.Pop().ToJson(loc))
        );
    }
}