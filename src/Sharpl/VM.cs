using Sharpl.Libs;
using Sharpl.Types.Core;
using System.Diagnostics;
using System.Text;

namespace Sharpl;

using PC = int;

public class VM
{
    public struct C
    {
        public int MaxRegisters = 1024;
        public int MaxVars = 128;

        public Reader Reader = new Readers.OneOf([
            Readers.WhiteSpace.Instance,

            Readers.And.Instance,
            Readers.Array.Instance,
            Readers.Call.Instance,
            Readers.Char.Instance,
            Readers.Fix.Instance,
            Readers.Int.Instance,
            Readers.Length.Instance,
            Readers.Map.Instance,
            Readers.Pair.Instance,
            Readers.Range.Instance,
            Readers.Quote.Instance,
            Readers.Splat.Instance,
            Readers.String.Instance,
            Readers.Unquote.Instance,

            Readers.Id.Instance
        ]);

        public C() { }
    };

    public static readonly C DEFAULT = new C();
    public static readonly int VERSION = 26;

    public readonly Libs.Char CharLib;
    public readonly Libs.Core CoreLib = new Libs.Core();
    public readonly Libs.Fix FixLib;
    public readonly Libs.IO IOLib;
    public readonly Libs.Iter IterLib;
    public readonly Libs.Json JsonLib;
    public readonly Libs.Net NetLib;
    public readonly Libs.String StringLib;
    public readonly Libs.Term TermLib;
    public readonly Libs.Time TimeLib;
    public readonly Lib UserLib = new Lib("user", null, []);

    public readonly C Config;
    public PC PC = 0;
    public readonly Term Term = new Term();
    public readonly Random Random = new Random();

    private readonly List<Call> calls = [];
    private readonly List<Op> code = [];
    private int definitionCount = 0;
    private Env? env;
    private readonly List<(int, int)> frames = [];
    private readonly List<Label> labels = [];
    private string loadPath = "";
    private int nextRegisterIndex = 0;
    private Dictionary<object, int> objectIds = new Dictionary<object, int>();
    private readonly Value[] registers;
    private readonly List<int> splats = [];
    private readonly Dictionary<string, Sym> syms = [];

    public VM(C config)
    {
        Config = config;
        registers = new Value[config.MaxRegisters];
        nextRegisterIndex = 0;

        UserLib.Init(this);
        UserLib.Import(CoreLib);
        Env = UserLib;
        BeginFrame(config.MaxVars);

        IterLib = new Libs.Iter();
        IterLib.Init(this);
        UserLib.Import(IterLib);

        CharLib = new Libs.Char();
        CharLib.Init(this);

        StringLib = new Libs.String();
        StringLib.Init(this);

        FixLib = new Libs.Fix();
        FixLib.Init(this);

        JsonLib = new Libs.Json();
        JsonLib.Init(this);

        IOLib = new Libs.IO(this);
        IOLib.Init(this);

        NetLib = new Libs.Net();
        NetLib.Init(this);

        TermLib = new Libs.Term(this);
        TermLib.Init(this);

        TimeLib = new Libs.Time(this);
        TimeLib.Init(this);
    }

    public int AllocRegister()
    {
        var res = nextRegisterIndex;
        nextRegisterIndex++;
        return res;
    }

    public void BeginFrame(int registerCount)
    {
        var total = registerCount;
        if (frames.Count > 0) { total += frames[^1].Item2; }
        frames.Push((registerCount, total));
        nextRegisterIndex = 0;
    }

    public void CallUserMethod(Loc loc, Stack stack, UserMethod target, Value?[] argMask, int arity, int registerCount)
    {
        BeginFrame(registerCount);
        calls.Push(new Call(target, PC, frames.Count, loc));
        target.BindArgs(this, argMask, arity, stack);
#pragma warning disable CS8629
        PC = (PC)target.StartPC;
#pragma warning restore CS8629
    }

    public void Define(string name)
    {
        var i = definitionCount;
        Env[name] = Value.Make(Core.Binding, new Register(-1, i));
        Emit(Ops.SetRegister.Make(new Register(-1, i)));
        definitionCount++;
    }

    public void Dmit(PC startPC)
    {
        for (var pc = startPC; pc < code.Count; pc++) { Term.Write($"{pc,-4} {code[pc]}\n"); }
        Term.Flush();
    }

    public void DoEnv(Env env, Action action)
    {
        var prevEnv = Env;
        Env = env;
        BeginFrame(nextRegisterIndex);

        try { action(); }
        finally
        {
            Env = prevEnv;
            nextRegisterIndex = EndFrame().Item1;
        }
    }

    public PC Emit(Op op)
    {
        var result = code.Count;
        code.Push(op);
        return result;
    }

    public void Emit(Form form) => new Form.Queue([form]).Emit(this);

    public void Emit(string code, Loc loc) =>
        ReadForms(new Source(new StringReader(code)), ref loc).Emit(this);

    public PC EmitPC => code.Count;

    public (int, int) EndFrame() => frames.Pop();

    public Env Env
    {
        get => env ?? UserLib;
        set => env = value;
    }

    public void Eval(PC startPC, Stack stack)
    {
        PC = startPC;

        while (true)
        {
            switch (code[PC])
            {
                case Ops.And op:
                    {
                        if (!(bool)stack.Peek()) { PC = op.Done.PC; }
                        else { PC++; }
                        break;
                    }
                case Ops.BeginFrame op:
                    {
                        BeginFrame(op.RegisterCount);
                        PC++;
                        break;
                    }
                case Ops.Benchmark op:
                    Eval(op, stack);
                    break;
                case Ops.Branch op:
                    {
                        if (stack.TryPop(out var v))
                        {
                            if ((bool)v) { PC++; }
                            else { PC = op.Right.PC; }
                        }
                        else { throw new EvalError("Missing condition", op.Loc); }

                        break;
                    }
                case Ops.CallDirect op:
                    {
                        var arity = op.Arity;
                        if (op.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        op.Target.Call(this, stack, arity, op.RegisterCount, op.Loc);
                        break;
                    }
                case Ops.CallMethod op:
                    {
                        var arity = op.Arity;
                        if (op.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        op.Target.Call(op.Loc, this, stack, arity);
                        break;
                    }
                case Ops.CallRegister op:
                    {
                        var arity = op.Arity;
                        if (op.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        var target = Get(op.Target);
                        target.Call(this, stack, arity, op.RegisterCount, op.Loc);
                        break;
                    }
                case Ops.CallStack op:
                    {
                        var target = stack.Pop();
                        var arity = op.Arity;
                        if (op.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        target.Call(this, stack, arity, op.RegisterCount, op.Loc);
                        break;
                    }
                case Ops.CallTail op:
                    {
                        var arity = op.ArgMask.Length;
                        if (op.Splat) { arity += splats.Pop(); }
                        if (arity < op.Target.MinArgCount) { throw new EvalError($"Not enough arguments: {op.Target} {arity}", op.Loc); }
                        var call = calls.Peek();
                        frames.Trunc(call.FrameOffset);
                        op.Target.BindArgs(this, op.ArgMask, arity, stack);
#pragma warning disable CS8629 
                        PC = (int)op.Target.StartPC;
#pragma warning restore CS8629
                        break;
                    }
                case Ops.CallUserMethod op:
                    {
                        var arity = op.ArgMask.Length;
                        if (op.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        CallUserMethod(op.Loc, stack, op.Target, op.ArgMask, arity, op.RegisterCount);
                        break;
                    }
                case Ops.Check op:
                    Eval(op, stack);
                    break;
                case Ops.CopyRegister op:
                    {
                        var v = GetRegister(op.FromFrameOffset, op.FromIndex);
                        SetRegister(op.ToFrameOffset, op.ToIndex, v);
                        PC++;
                        break;
                    }
                case Ops.CreateArray op:
                    {
                        stack.Push(Value.Make(Core.Array, new Value[op.Length]));
                        PC++;
                        break;
                    }
                case Ops.CreateIter op:
                    {
                        var v = stack.Pop();
                        if (v.Type is IterTrait it) { Set(op.Target, Value.Make(Core.Iter, it.CreateIter(v, this, op.Loc))); }
                        else { throw new EvalError($"Not iterable: {v}", op.Loc); }
                        PC++;
                        break;
                    }
                case Ops.CreateList op:
                    {
                        Set(op.Target, Value.Make(Core.List, new List<Value>()));
                        PC++;
                        break;
                    }
                case Ops.CreateMap:
                    {
                        stack.Push(Value.Make(Core.Map, new OrderedMap<Value, Value>()));
                        PC++;
                        break;
                    }
                case Ops.CreatePair:
                    {
                        var r = stack.Pop();
                        var l = stack.Pop();
                        stack.Push(Value.Make(Core.Pair, (l, r)));
                        PC++;
                        break;
                    }
                case Ops.Decrement op:
                    {
                        var i = Index(op.Target);
                        var v = Value.Make(Core.Int, registers[i].CastUnbox(Core.Int) - op.Delta);
                        registers[i] = v;
                        PC++;
                        break;
                    }
                case Ops.Drop op:
                    {
                        stack.Drop(op.Count);
                        PC++;
                        break;
                    }
                case Ops.EndFrame:
                    {
                        EndFrame();
                        PC++;
                        break;
                    }
                case Ops.ExitMethod:
                    {
                        var c = calls.Pop();
                        foreach (var (_, s, _) in c.Target.Closure) { c.Target.ClosureValues[s] = GetRegister(0, s); }
                        EndFrame();
                        PC = c.ReturnPC;
                        break;
                    }
                case Ops.GetRegister op:
                    {
                        stack.Push(Get(op.Target));
                        PC++;
                        break;
                    }
                case Ops.Goto op:
                    {
                        PC = op.Target.PC;
                        break;
                    }
                case Ops.Increment op:
                    {
                        var i = Index(op.Target);
                        var v = Value.Make(Core.Int, registers[i].CastUnbox(Core.Int) + op.Delta);
                        registers[i] = v;
                        PC++;
                        break;
                    }
                case Ops.IterNext op:
                    {
                        if (Get(op.Iter).Cast(Core.Iter).Next() is Value v)
                        {
                            if (op.Push) { stack.Push(v); }
                            PC++;
                        }
                        else { PC = op.Done.PC; }

                        break;
                    }
                case Ops.OpenInputStream op:
                    Eval(op, stack);
                    break;
                case Ops.Or op:
                    {
                        if ((bool)stack.Peek()) { PC = op.Done.PC; }
                        else { PC++; }
                        break;
                    }
                case Ops.PopItem op:
                    {
                        var t = Get(op.Target);
                        if (t.Type is StackTrait st) { stack.Push(st.Pop(op.Loc, this, op.Target, t)); }
                        else { throw new EvalError($"Invalid target: {t}", op.Loc); }
                        PC++;
                        break;
                    }
                case Ops.PrepareClosure op:
                    {
                        var m = op.Target;
                        foreach (var (_, d, s) in m.Closure) { m.ClosureValues[d] = Get(s); }
                        PC = op.Skip.PC;
                        break;
                    }
                case Ops.Push op:
                    {
                        stack.Push(op.Value.Copy());
                        PC++;
                        break;
                    }
                case Ops.PushItem op:
                    {
                        if (stack.TryPop(out var v))
                        {
                            var t = Get(op.Target);
                            if (t.Type is StackTrait st) { st.Push(op.Loc, this, op.Target, t, v); }
                            else { throw new EvalError($"Invalid target: {t}", op.Loc); }
                        }
                        else { throw new EvalError("Missing value", op.Loc); }

                        PC++;
                        break;
                    }
                case Ops.PushSplat:
                    {
                        splats.Push(0);
                        PC++;
                        break;
                    }
                case Ops.Repush op:
                    {
                        var v = stack.Peek();
                        for (var i = 0; i < op.N; i++) { stack.Push(v); }
                        PC++;
                        break;
                    }
                case Ops.SetArrayItem op:
                    {
                        var v = stack.Pop();
                        stack.Peek().Cast(Core.Array)[op.Index] = v;
                        PC++;
                        break;
                    }
                case Ops.SetLoadPath op:
                    {
                        loadPath = op.Path;
                        PC++;
                        break;
                    }
                case Ops.SetMapItem:
                    {
                        var v = stack.Pop();
                        var k = stack.Pop();
                        stack.Peek().Cast(Core.Map)[k] = v;
                        PC++;
                        break;
                    }
                case Ops.SetRegister op:
                    {
                        Set(op.Target, stack.Pop());
                        PC++;
                        break;
                    }
                case Ops.Splat op:
                    {
                        if (stack.Count == 0) { throw new EvalError("Missing splat target", op.Loc); }
                        else
                        {
                            var tv = stack.Pop();

                            if (tv.Type is Types.Core.IterTrait tt)
                            {
                                if (splats.Count == 0) { throw new EvalError("Splat outside context", op.Loc); }
                                var arity = splats.Pop();

                                foreach (var v in tt.CreateIter(tv, this, op.Loc))
                                {
                                    stack.Push(v);
                                    arity++;
                                }

                                splats.Push(arity);
                            }
                            else { throw new EvalError($"Invalid splat target: {tv}", op.Loc); }
                        }

                        PC++;
                        break;
                    }
                case Ops.Stop:
                    {
                        PC++;
                        return;
                    }
                case Ops.Swap:
                    {
                        var x = stack.Pop();
                        var y = stack.Pop();
                        stack.Push(x);
                        stack.Push(y);
                        PC++;
                        break;
                    }
                case Ops.UnquoteRegister op:
                    {
                        var f = Get(op.Register).Unquote(this, op.Loc);
                        Eval(f, stack);
                        PC++;
                        break;
                    }
                case Ops.Unzip op:
                    {
                        if (stack.TryPop(out var p))
                        {
                            var pv = p.CastUnbox(Core.Pair);
                            stack.Push(pv.Item1);
                            stack.Push(pv.Item2);
                        }
                        else { throw new EvalError("Missing target", op.Loc); }

                        PC++;
                        break;
                    }
            }
        }
    }

    public Value? Eval(PC startPC)
    {
        var s = new Stack();
        Eval(startPC, s);
        return (s.Count == 0) ? null : s.Pop();
    }

    public void Eval(Form.Queue target, Stack stack)
    {
        var skipLabel = new Label();
        Emit(Ops.Goto.Make(skipLabel));
        var startPC = EmitPC;
        target.Emit(this);
        Emit(Ops.Stop.Make());
        skipLabel.PC = EmitPC;
        var prevPC = PC;
        Eval(startPC, stack);
        PC = prevPC;
    }

    public Value? Eval(Form.Queue target)
    {
        var stack = new Stack();
        Eval(target, stack);
        return (stack.Count == 0) ? null : stack.Pop();
    }

    public Value? Eval(Form target) => Eval(new Form.Queue([target]));
    public void Eval(Form target, Stack stack) => Eval(new Form.Queue([target]), stack);

    public Value? Eval(string code)
    {
        var loc = new Loc("Eval");
        var forms = ReadForms(new Source(new StringReader(code)), ref loc);
        return Eval(forms);
    }

    public int FrameCount => frames.Count;

    public Value Get(Register register) => GetRegister(register.FrameOffset, register.Index);

    public int GetObjectId(object it)
    {
        if (!objectIds.ContainsKey(it))
        {
            var id = objectIds.Count + 1;
            objectIds[it] = id;
            return id;
        }

        return objectIds[it];
    }

    public Value GetRegister(int frameOffset, int index) => registers[RegisterIndex(frameOffset, index)];

    public Sym Intern(string name)
    {
        if (syms.TryGetValue(name, out var sym))
        {
            return sym;
        }

        return syms[name] = new Sym(name);
    }

    public Sym Gensym(string suffix) => Intern($"{syms.Count}{suffix}");

    public Label Label(PC pc = -1)
    {
        var l = new Label(pc);
        labels.Add(l);
        return l;
    }

    public Lib Lib
    {
        get
        {
            for (var e = Env; e != null; e = e.Parent)
            {
                if (e is Lib l)
                {
                    return l;
                }
            }

            return UserLib;
        }
    }

    public void Load(string path)
    {
        var prevEnv = Env;
        var prevLoadPath = loadPath;
        var p = Path.Combine(loadPath, path);

        try
        {
            if (Path.GetDirectoryName(p) is string d) { loadPath = d; }
            var loc = new Loc(path);

            using (StreamReader source = new StreamReader(p, Encoding.UTF8))
            {
                var c = source.Peek();

                if (c == '#')
                {
                    source.ReadLine();
                    loc.NewLine();
                }

                var forms = ReadForms(new Source(source), ref loc);
                Emit(Ops.SetLoadPath.Make(loadPath));
                forms.Emit(this);
                Emit(Ops.SetLoadPath.Make(prevLoadPath));
            }
        }
        finally
        {
            Env = prevEnv;
            loadPath = prevLoadPath;
        }

    }

    public int NextRegisterIndex => nextRegisterIndex;

    public bool ReadForm(Source source, ref Loc loc, Form.Queue forms) =>
        Config.Reader.Read(source, this, forms, ref loc);

    public Form? ReadForm(Source source, ref Loc loc)
    {
        var forms = new Form.Queue();
        ReadForm(source, ref loc, forms);
        return forms.TryPop();
    }

    public void ReadForms(Source source, ref Loc loc, Form.Queue forms)
    {
        while (ReadForm(source, ref loc, forms)) { }
    }

    public Form.Queue ReadForms(Source source, ref Loc loc)
    {
        var forms = new Form.Queue();
        ReadForms(source, ref loc, forms);
        return forms;
    }

    public int RegisterIndex(int frameOffset, int index) =>
        (frameOffset == -1) ? index : index + frames.Peek(frameOffset).Item2;

    public int Index(Register reg) => RegisterIndex(reg.FrameOffset, reg.Index);

    public void SetRegister(int frameOffset, int index, Value value) =>
        registers[RegisterIndex(frameOffset, index)] = value;

    public void Set(Register register, Value value) =>
        SetRegister(register.FrameOffset, register.Index, value);

    private void Eval(Ops.Benchmark op, Stack stack)
    {
        var bodyPC = PC + 1;
        var s = new Stack();

        for (var i = 0; i < op.N; i++)
        {
            Eval(bodyPC, s);
            s.Clear();
        }

        var t = Stopwatch.GetTimestamp();

        for (var i = 0; i < op.N; i++)
        {
            Eval(bodyPC, s);
            s.Clear();
        }

        var e = Stopwatch.GetElapsedTime(t).TotalMilliseconds;
        stack.Push(Value.Make(Core.Int, (int)e));
    }

    private void Eval(Ops.Check op, Stack stack)
    {
        if (stack.Pop() is Value ev)
        {
            if (stack.Pop() is Value av)
            {
                var dav = av;
                if (ev.Type == Core.Bit && av.Type != Core.Bit) { av = Value.Make(Core.Bit, (bool)av); }
                if (!av.Equals(ev)) { throw new EvalError($"Check failed: expected {ev.Dump(this)}, actual {dav.Dump(this)}!", op.Loc); }
            }
            else { throw new EvalError("Missing actual value", op.Loc); }
        }
        else { throw new EvalError("Missing expected value", op.Loc); }

        PC++;
    }

    private void Eval(Ops.OpenInputStream op, Stack stack)
    {
        StreamReader sr;

        if (stack.Pop() is Value p)
        {
            sr = new StreamReader(Path.Combine(loadPath, p.Cast(Core.String, op.Loc)));
            SetRegister(op.FrameOffset, op.Index, Value.Make(IO.InputStream, sr));
        }
        else { throw new EvalError("Missing path", op.Loc); }

        PC++;
    }
}