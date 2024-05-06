namespace Sharpl.Libs;

using Sharpl.Types.Term;

using System.Text;

public class Term : Lib
{
    public static readonly KeyType Key = new KeyType("Int");
 
    public Term() : base("term", null)
    {
        BindType(Key);

        BindMethod("read-key", [], (loc, target, vm, stack, arity, recursive) =>
        {
            stack.Push(Key, Console.ReadKey(true));
        });

        BindMethod("read-line", [], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = Console.ReadLine();

            if (res is null) {
                stack.Push(Core.Nil, false);
            } else {
                stack.Push(Core.String, res);
            }
        });

        BindMethod("say", [], (loc, target, vm, stack, arity, recursive) =>
        {
            var res = new StringBuilder();

            while (arity > 0)
            {
                stack.Pop().Say(res);
                arity--;
            }

            Console.WriteLine(res.ToString());
        });
    }


    public Method BindMethod(string name, string[] args, Method.BodyType body)
    {
        var m = new Method(name, args, body);
        Bind(m.Name, Value.Make(Core.Method, m));
        return m;
    }

    public void BindType(AnyType t)
    {
        Bind(t.Name, Value.Make(Core.Meta, t));
    }
}