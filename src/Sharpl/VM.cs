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
    public static readonly int VERSION = 28;

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
    private int varCount = 0;
    private Env? env;
    private readonly List<Frame> frames = [];
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

        var loc = new Loc("init");
        UserLib.Init(this, loc);
        UserLib.Import(CoreLib);
        Env = UserLib;
        BeginFrame(config.MaxVars);

        IterLib = new Libs.Iter();
        IterLib.Init(this, loc);
        UserLib.Import(IterLib);

        CharLib = new Libs.Char();
        CharLib.Init(this, loc);

        StringLib = new Libs.String();
        StringLib.Init(this, loc);

        FixLib = new Libs.Fix();
        FixLib.Init(this, loc);

        JsonLib = new Libs.Json();
        JsonLib.Init(this, loc);

        IOLib = new Libs.IO(this);
        IOLib.Init(this, loc);

        NetLib = new Libs.Net();
        NetLib.Init(this, loc);

        TermLib = new Libs.Term(this);
        TermLib.Init(this, loc);

        TimeLib = new Libs.Time(this);
        TimeLib.Init(this, loc);
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
        if (frames.Count > 0) { total += frames[^1].RegisterCount; }
        frames.Push(new Frame(registerCount, total));
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

    public void BindVar(string name)
    {
        var i = varCount;
        Env[name] = Value.Make(Core.Binding, new Register(-1, i));
        Emit(Ops.SetRegister.Make(new Register(-1, i)));
        varCount++;
    }

    public void BindVar(Form f)
    {
        switch (f)
        {
            case Forms.Id idf:
                BindVar(idf.Name);
                break;
            case Forms.Nil:
                Emit(Ops.Drop.Make(1));
                break;
            case Forms.Pair pf:
                Emit(Ops.Unzip.Make(pf.Loc));
                Emit(Ops.Swap.Make(pf.Loc));
                BindVar(pf.Left);
                BindVar(pf.Right);
                break;
            default:
                throw new EmitError($"Invalid lvalue: {f}", f.Loc);
        }
    }

    public void Dmit(PC startPC)
    {
        for (var pc = startPC; pc < code.Count; pc++) { Term.Write($"{pc,-4} {code[pc].Dump(this)}\n"); }
        Term.Flush();
    }

    public void DoEnv(Env env, Loc loc, Action action)
    {
        var prevEnv = Env;
        Env = env;
        BeginFrame(nextRegisterIndex);

        try { action(); }
        finally
        {
            Env = prevEnv;
            nextRegisterIndex = EndFrame(loc).RegisterIndex;
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

    public Frame EndFrame(Loc loc)
    {
        var f = frames.Pop();
        f.RunDeferred(this, loc);
        return f;
    }

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
            var op = code[PC];
            //Console.WriteLine(PC + " " + op.Dump(this));

            switch (op.Code)
            {
                case OpCode.And:
                    {
                        if (!(bool)stack.Peek()) { PC = ((Ops.And)op).Done.PC; }
                        else { PC++; }
                        break;
                    }
                case OpCode.BeginFrame:
                    {
                        PC++;
                        BeginFrame(((Ops.BeginFrame)op).RegisterCount);
                        break;
                    }
                case OpCode.Benchmark:
                    Eval((Ops.Benchmark)op, stack);
                    break;
                case OpCode.Branch:
                    {
                        var branchOp = (Ops.Branch)op;
                        Value? v = null;

                        if (branchOp.Pop && stack.TryPop(out var v1)) { v = v1; }
                        else if (!branchOp.Pop && stack.TryPeek(out var v2)) { v = v2; }

                        if (v is null) { throw new EvalError("Missing condition", branchOp.Loc); }

                        if ((bool)v) { PC++; }
                        else { PC = branchOp.Right.PC; }

                        break;
                    }
                case OpCode.CallDirect:
                    {
                        var callOp = (Ops.CallDirect)op;
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        callOp.Target.Call(this, stack, arity, callOp.RegisterCount, false, callOp.Loc);
                        break;
                    }
                case OpCode.CallMethod:
                    {
                        var callOp = (Ops.CallMethod)op;
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        callOp.Target.Call(this, stack, arity, callOp.Loc);
                        break;
                    }
                case OpCode.CallRegister:
                    {
                        var callOp = (Ops.CallRegister)op;
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        var target = Get(callOp.Target);
                        target.Call(this, stack, arity, callOp.RegisterCount, false, callOp.Loc);
                        break;
                    }
                case OpCode.CallStack:
                    {
                        var callOp = (Ops.CallStack)op;
                        var target = stack.Pop();
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        target.Call(this, stack, arity, callOp.RegisterCount, false, callOp.Loc);
                        break;
                    }
                case OpCode.CallTail:
                    {
                        var callOp = (Ops.CallTail)op;
                        var arity = callOp.ArgMask.Length;
                        if (callOp.Splat) { arity += splats.Pop(); }
                        if (arity < callOp.Target.MinArgCount) { throw new EvalError($"Not enough arguments: {callOp.Target} {arity}", callOp.Loc); }
                        var call = calls.Peek();
                        frames.Trunc(call.FrameOffset);
                        callOp.Target.BindArgs(this, callOp.ArgMask, arity, stack);
#pragma warning disable CS8629 
                        PC = (int)callOp.Target.StartPC;
#pragma warning restore CS8629
                        break;
                    }
                case OpCode.CallUserMethod:
                    {
                        var callOp = (Ops.CallUserMethod)op;
                        var arity = callOp.ArgMask.Length;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        CallUserMethod(callOp.Loc, stack, callOp.Target, callOp.ArgMask, arity, callOp.RegisterCount);
                        break;
                    }
                case OpCode.Check:
                    Eval((Ops.Check)op, stack);
                    break;
                case OpCode.CopyRegister:
                    {
                        var copyOp = (Ops.CopyRegister)op;
                        var v = Get(copyOp.From);
                        Set(copyOp.To, v);
                        PC++;
                        break;
                    }
                case OpCode.CreateArray:
                    {
                        var createOp = (Ops.CreateArray)op;
                        stack.Push(Value.Make(Core.Array, new Value[createOp.Length]));
                        PC++;
                        break;
                    }
                case OpCode.CreateIter:
                    {
                        var createOp = (Ops.CreateIter)op;
                        var v = stack.Pop();
                        if (v.Type is IterTrait it) { Set(createOp.Target, Value.Make(Core.Iter, it.CreateIter(v, this, createOp.Loc))); }
                        else { throw new EvalError($"Not iterable: {v}", createOp.Loc); }
                        PC++;
                        break;
                    }
                case OpCode.CreateList:
                    {
                        var createOp = (Ops.CreateList)op;
                        Set(createOp.Target, Value.Make(Core.List, new List<Value>()));
                        PC++;
                        break;
                    }
                case OpCode.CreateMap:
                    {
                        stack.Push(Value.Make(Core.Map, new OrderedMap<Value, Value>()));
                        PC++;
                        break;
                    }
                case OpCode.CreatePair:
                    {
                        var r = stack.Pop();
                        var l = stack.Pop();
                        stack.Push(Value.Make(Core.Pair, (l, r)));
                        PC++;
                        break;
                    }
                case OpCode.Decrement:
                    {
                        var decrementOp = (Ops.Decrement)op;
                        ref var t = ref Get(decrementOp.Target);
                        var v = Value.Make(Core.Int, t.CastUnbox(Core.Int) - decrementOp.Delta);
                        t = v;
                        PC++;
                        break;
                    }
                case OpCode.Drop:
                    {
                        var dropOp = (Ops.Drop)op;
                        stack.Drop(dropOp.Count);
                        PC++;
                        break;
                    }
                case OpCode.EndFrame:
                    {
                        EndFrame(((Ops.EndFrame)op).Loc);
                        PC++;
                        break;
                    }
                case OpCode.ExitMethod:
                    {
                        var c = calls.Pop();
                        foreach (var (_, s, _) in c.Target.Closure) { c.Target.ClosureValues[s] = GetRegister(0, s); }
                        EndFrame(c.Loc);
                        PC = c.ReturnPC;
                        break;
                    }
                case OpCode.GetRegister:
                    {
                        stack.Push(Get(((Ops.GetRegister)op).Target));
                        PC++;
                        break;
                    }
                case OpCode.Goto:
                    {
                        PC = ((Ops.Goto)op).Target.PC;
                        break;
                    }
                case OpCode.Increment:
                    {
                        var incrementOp = (Ops.Increment)op;
                        ref var t = ref Get(incrementOp.Target);
                        var v = Value.Make(Core.Int, t.CastUnbox(Core.Int) + incrementOp.Delta);
                        t = v;
                        PC++;
                        break;
                    }
                case OpCode.IterNext:
                    {
                        var iterOp = (Ops.IterNext)op;

                        if (Get(iterOp.Iter).Cast(Core.Iter).Next(this, iterOp.Loc) is Value v)
                        {
                            if (iterOp.Push) { stack.Push(v); }
                            PC++;
                        }
                        else { PC = iterOp.Done.PC; }

                        break;
                    }
                case OpCode.OpenInputStream:
                    Eval((Ops.OpenInputStream)op, stack);
                    break;
                case OpCode.Or:
                    {
                        if ((bool)stack.Peek()) { PC = ((Ops.Or)op).Done.PC; }
                        else { PC++; }
                        break;
                    }
                case OpCode.PopItem:
                    {
                        var popOp = (Ops.PopItem)op;
                        var t = Get(popOp.Target);
                        if (t.Type is StackTrait st) { stack.Push(st.Pop(popOp.Loc, this, popOp.Target, t)); }
                        else { throw new EvalError($"Invalid target: {t}", popOp.Loc); }
                        PC++;
                        break;
                    }
                case OpCode.PrepareClosure:
                    {
                        var prepareOp = (Ops.PrepareClosure)op;
                        var m = prepareOp.Target;
                        foreach (var (_, d, s) in m.Closure) { m.ClosureValues[d] = Get(s); }
                        PC = prepareOp.Skip.PC;
                        break;
                    }
                case OpCode.Push:
                    {
                        stack.Push(((Ops.Push)op).Value.Copy());
                        PC++;
                        break;
                    }
                case OpCode.PushItem:
                    {
                        var pushOp = (Ops.PushItem)op;

                        if (stack.TryPop(out var v))
                        {
                            var t = Get(pushOp.Target);
                            if (t.Type is StackTrait st) { st.Push(pushOp.Loc, this, pushOp.Target, t, v); }
                            else { throw new EvalError($"Invalid target: {t}", pushOp.Loc); }
                        }
                        else { throw new EvalError("Missing value", pushOp.Loc); }

                        PC++;
                        break;
                    }
                case OpCode.PushSplat:
                    {
                        splats.Push(0);
                        PC++;
                        break;
                    }
                case OpCode.Repush:
                    {
                        var v = stack.Peek();
                        for (var i = 0; i < ((Ops.Repush)op).Count; i++) { stack.Push(v); }
                        PC++;
                        break;
                    }
                case OpCode.SetArrayItem:
                    {
                        var v = stack.Pop();
                        stack.Peek().Cast(Core.Array)[((Ops.SetArrayItem)op).Index] = v;
                        PC++;
                        break;
                    }
                case OpCode.SetLoadPath:
                    {
                        loadPath = ((Ops.SetLoadPath)op).Path;
                        PC++;
                        break;
                    }
                case OpCode.SetMapItem:
                    {
                        var v = stack.Pop();
                        var k = stack.Pop();
                        stack.Peek().Cast(Core.Map)[k] = v;
                        PC++;
                        break;
                    }
                case OpCode.SetRegister:
                    {
                        Set(((Ops.SetRegister)op).Target, stack.Pop());
                        PC++;
                        break;
                    }
                case OpCode.Splat:
                    {
                        var splatOp = (Ops.Splat)op;

                        if (stack.Count == 0) { throw new EvalError("Missing splat target", splatOp.Loc); }
                        else
                        {
                            var tv = stack.Pop();

                            if (tv.Type is IterTrait tt)
                            {
                                if (splats.Count == 0) { throw new EvalError("Splat outside context", splatOp.Loc); }
                                var arity = splats.Pop();
                                var it = tt.CreateIter(tv, this, splatOp.Loc);

                                while (it.Next(this, splatOp.Loc) is Value v)
                                {
                                    stack.Push(v);
                                    arity++;
                                }

                                splats.Push(arity);
                            }
                            else { throw new EvalError($"Invalid splat target: {tv}", splatOp.Loc); }
                        }

                        PC++;
                        break;
                    }
                case OpCode.Stop:
                    {
                        PC++;
                        return;
                    }
                case OpCode.Swap:
                    {
                        var x = stack.Pop();
                        var y = stack.Pop();
                        stack.Push(x);
                        stack.Push(y);
                        PC++;
                        break;
                    }
                case OpCode.UnquoteRegister:
                    {
                        var unquoteOp = (Ops.UnquoteRegister)op;
                        var f = Get(unquoteOp.Register).Unquote(this, unquoteOp.Loc);
                        Eval(f, stack);
                        PC++;
                        break;
                    }
                case OpCode.Unzip:
                    {
                        var unzipOp = (Ops.Unzip)op;

                        if (stack.TryPop(out var p))
                        {
                            var pv = p.CastUnbox(Core.Pair);
                            stack.Push(pv.Item1);
                            stack.Push(pv.Item2);
                        }
                        else { throw new EvalError("Missing target", unzipOp.Loc); }

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

    public void EvalUntil(PC endPC, Stack stack)
    {
        var prev = code[endPC];
        code[endPC] = Ops.Stop.Make();
        try { Eval(PC, stack); }
        finally { code[endPC] = prev; }
    }

    public Frame Frame => frames[^1];
    public int FrameCount => frames.Count;

    public ref Value Get(Register register) => ref GetRegister(register.FrameOffset, register.Index);

    public int GetObjectId(object it)
    {
        if (objectIds.ContainsKey(it)) { return objectIds[it]; }
        var id = objectIds.Count + 1;
        objectIds[it] = id;
        return id;
    }

    public ref Value GetRegister(int frameOffset, int index) =>
        ref registers[RegisterIndex(frameOffset, index)];

    public Sym Intern(string name) =>
        syms.TryGetValue(name, out var sym) ? sym : syms[name] = new Sym(name);

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
                if (e is Lib l) { return l; }
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
        (frameOffset == -1) ? index : index + frames.Peek(frameOffset).RegisterCount;

    public int Index(Register reg) => RegisterIndex(reg.FrameOffset, reg.Index);

    public void SetRegister(int frameOffset, int index, Value value) =>
        registers[RegisterIndex(frameOffset, index)] = value;

    public void Set(Register register, Value value) =>
        SetRegister(register.FrameOffset, register.Index, value);

    private void Eval(Ops.Benchmark op, Stack stack)
    {
        var bodyPC = PC + 1;
        var s = new Stack();

        for (var i = 0; i < op.Reps; i++)
        {
            Eval(bodyPC, s);
            s.Clear();
        }

        var t = Stopwatch.GetTimestamp();

        for (var i = 0; i < op.Reps; i++)
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