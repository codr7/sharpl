using Sharpl.Libs;
using Sharpl.Types.Core;
using System.Diagnostics;
using System.Text;

namespace Sharpl;

public class VM
{
    public struct C
    {
        public int MaxArgs = 32;
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
    public static readonly int VERSION = 29;

    public readonly Libs.Char CharLib;
    public readonly Libs.Core CoreLib;
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
    private readonly List<Value> deferred = new List<Value>();
    private readonly List<(Sym, Value)> restarts = new List<(Sym, Value)>();
    private Env? env;
    private readonly List<Frame> frames = [];
    private readonly List<Label> labels = [];
    private string loadPath = "";
    private int nextRegisterIndex = 0;
    private int nextVarIndex = 0;
    private Dictionary<object, int> objectIds = new Dictionary<object, int>();
    private readonly Value[] registers;
    private readonly List<int> splats = [];
    private readonly Dictionary<string, Sym> syms = [];

    public VM(C config)
    {
        Config = config;
        registers = new Value[config.MaxRegisters];
        nextRegisterIndex = 0;
        Result = AllocVar();

        CoreLib = new Libs.Core(this);

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

    public void AddRestart(Sym id, int arity, Method.BodyType body)
    {
        var args = new string[arity];
        for (var i = 0; i < arity; i++) { args[i] = $"value{i}"; }
        restarts.Add((id, Value.Make(Libs.Core.Method, new Method("", args, body))));
    }
    public int AllocRegister()
    {
        var res = nextRegisterIndex;
        nextRegisterIndex++;
        return res;
    }

    public Register AllocVar()
    {
        var res = nextVarIndex;
        nextVarIndex++;
        return new Register(-1, res);
    }

    public void BeginFrame(int registerCount)
    {
        var total = registerCount;
        if (frames.Count > 0) { total += frames[^1].RegisterCount; }
        frames.Push(new Frame(registerCount, total, deferred.Count, restarts.Count));
        nextRegisterIndex = Config.MaxArgs;
    }

    public void CallUserMethod(Loc loc, UserMethod target, Value?[] argMask, int arity, int registerCount, Register result)
    {
        BeginFrame(registerCount);
        calls.Push(new Call(target, PC, frames.Count, result, loc));
        target.BindArgs(this, argMask, arity);
#pragma warning disable CS8629
        PC = (PC)target.StartPC;
#pragma warning restore CS8629
    }

    public void BindVar(string name, Register value)
    {
        var i = nextVarIndex;
        Env[name] = Value.Make(Libs.Core.Binding, new Register(-1, i));
        Emit(Ops.CopyRegister.Make(value, new Register(-1, i)));
        nextVarIndex++;
    }

    public void BindVar(Form f, Register value)
    {
        switch (f)
        {
            case Forms.Id idf:
                BindVar(idf.Name, value);
                break;
            case Forms.Pair pf:
                var l = new Register(0, AllocRegister());
                var r = new Register(0, AllocRegister());
                Emit(Ops.Unzip.Make(value, l, r, f.Loc));
                BindVar(pf.Left, l);
                BindVar(pf.Right, r);
                break;
            default:
                throw new EmitError($"Invalid lvalue: {f}", f.Loc);
        }
    }

    public void Defer(Value target) =>
        deferred.Add(target);

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

    public void Emit(Form form, Register result) => new Form.Queue([form]).Emit(this, result);

    public void Emit(string code, Register result, Loc loc) =>
        ReadForms(new Source(new StringReader(code)), ref loc).Emit(this, result);

    public PC EmitPC => code.Count;

    public Frame EndFrame(Loc loc)
    {
        var f = frames.Pop();
        RunDeferred(f.DeferOffset, loc);
        restarts.Trunc(f.RestartOffset);
        return f;
    }

    public Env Env
    {
        get => env ?? UserLib;
        set => env = value;
    }

    public void Eval(PC startPC)
    {
        PC = startPC;

        while (true)
        {
            var op = code[PC];

            switch (op.Code)
            {
                case OpCode.And:
                    {
                        var andOp = (Ops.And)op;
                        if (!(bool)Get(andOp.Target)) PC = ((Ops.And)op).Done.PC;
                        else PC++;
                        break;
                    }
                case OpCode.BeginFrame:
                    {
                        PC++;
                        BeginFrame(((Ops.BeginFrame)op).RegisterCount);
                        break;
                    }
                case OpCode.Benchmark:
                    Eval((Ops.Benchmark)op);
                    break;
                case OpCode.Branch:
                    {
                        var branchOp = (Ops.Branch)op;
                        if ((bool)Get(branchOp.Cond)) { PC++; }
                        else { PC = branchOp.Right.PC; }
                        break;
                    }
                case OpCode.CallDirect:
                    {
                        var callOp = (Ops.CallDirect)op;
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        callOp.Target.Call(this, arity, callOp.RegisterCount, false, callOp.Result, callOp.Loc);
                        break;
                    }
                case OpCode.CallMethod:
                    {
                        var callOp = (Ops.CallMethod)op;
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        callOp.Target.Call(this, arity, callOp.Result, callOp.Loc);
                        break;
                    }
                case OpCode.CallRegister:
                    {
                        var callOp = (Ops.CallRegister)op;
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        var target = Get(callOp.Target);
                        target.Call(this, arity, callOp.RegisterCount, false, callOp.Result, callOp.Loc);
                        break;
                    }
                case OpCode.CallTail:
                    {
                        var callOp = (Ops.CallTail)op;
                        var arity = callOp.ArgMask.Length;
                        if (callOp.Splat) { arity += splats.Pop(); }
                        if (arity < callOp.Target.MinArgCount) { throw new EvalError($"Not enough arguments: {callOp.Target} {arity}", callOp.Loc); }
                        var call = calls.Pop();
                        calls.Push(new Call(call.Target, call.ReturnPC, call.FrameOffset, callOp.Result, callOp.Loc));
                        frames.Trunc(call.FrameOffset);
                        callOp.Target.BindArgs(this, callOp.ArgMask, arity);
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
                        CallUserMethod(callOp.Loc, callOp.Target, callOp.ArgMask, arity, callOp.RegisterCount, callOp.Result);
                        break;
                    }
                case OpCode.Check:
                    Eval((Ops.Check)op);
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
                        Set(createOp.Target, (Value.Make(Core.Array, new Value[createOp.Length])));
                        PC++;
                        break;
                    }
                case OpCode.CreateIter:
                    {
                        var createOp = (Ops.CreateIter)op;
                        var v = Get(createOp.Target);
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
                        var createOp = (Ops.CreateMap)op;
                        Set(createOp.Target, Value.Make(Core.Map, new OrderedMap<Value, Value>()));
                        PC++;
                        break;
                    }
                case OpCode.CreatePair:
                    {
                        var createOp = (Ops.CreatePair)op;
                        Set(createOp.Target, Value.Make(Core.Pair, (Get(createOp.Left), Get(createOp.Right))));
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
                            Set(iterOp.Value, v);
                            PC++;
                        }
                        else PC = iterOp.Done.PC;

                        break;
                    }
                case OpCode.OpenInputStream:
                    Eval((Ops.OpenInputStream)op);
                    break;
                case OpCode.Or:
                    {
                        var orOp = (Ops.Or)op;
                        if ((bool)Get(orOp.Target)) { PC = orOp.Done.PC; }
                        else { PC++; }
                        break;
                    }
                case OpCode.PopItem:
                    {
                        var popOp = (Ops.PopItem)op;
                        var t = Get(popOp.Target);
                        if (t.Type is StackTrait st) { Set(popOp.Result, st.Pop(popOp.Loc, this, popOp.Target, t)); }
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
                case OpCode.PushItem:
                    {
                        var pushOp = (Ops.PushItem)op;
                        var t = Get(pushOp.Target);
                        if (t.Type is StackTrait st) st.Push(pushOp.Loc, this, pushOp.Target, t, Get(pushOp.Value));
                        else { throw new EvalError($"Invalid target: {t}", pushOp.Loc); }
                        PC++;
                        break;
                    }
                case OpCode.PushSplat:
                    {
                        splats.Push(0);
                        PC++;
                        break;
                    }
                case OpCode.SetArrayItem:
                    {
                        var setOp = (Ops.SetArrayItem)op;
                        Get(setOp.Target).Cast(Core.Array)[setOp.Index] = Get(setOp.Value);
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
                        var setOp = (Ops.SetMapItem)op;
                        Get(setOp.Target).Cast(Core.Map)[Get(setOp.Key)] = Get(setOp.Value);
                        PC++;
                        break;
                    }
                case OpCode.SetRegister:
                    {
                        var setOp = (Ops.SetRegister)op;
                        Set(setOp.Target, Get(setOp.Value));
                        PC++;
                        break;
                    }
                case OpCode.SetRegisterDirect:
                    {
                        var setOp = (Ops.SetRegisterDirect)op;
                        Set(setOp.Target, setOp.Value);
                        PC++;
                        break;
                    }
                case OpCode.Splat:
                    {
                        var splatOp = (Ops.Splat)op;
                        var tv = Get(splatOp.Target);

                        if (tv.Type is IterTrait tt)
                        {
                            if (splats.Count == 0) { throw new EvalError("Splat outside context", splatOp.Loc); }
                            var arity = splats.Pop();
                            var it = tt.CreateIter(tv, this, splatOp.Loc);
                            var its = new List<Value>();
                            while (it.Next(this, splatOp.Loc) is Value v) its.Add(v);
                            Set(splatOp.Result, Value.Make(Core.Array, its.ToArray()));
                            splats.Push(arity);
                        }
                        else { throw new EvalError($"Invalid splat target: {tv}", splatOp.Loc); }

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
                        var swapOp = (Ops.Swap)op;
                        var x = Get(swapOp.X);
                        var y = Get(swapOp.Y);
                        Set(swapOp.Y, x);
                        Set(swapOp.X, y);
                        PC++;
                        break;
                    }
                case OpCode.Try:
                    Eval((Ops.Try)op);
                    break;
                case OpCode.Unquote:
                    {
                        var unquoteOp = (Ops.Unquote)op;
                        var f = Get(unquoteOp.Target).Unquote(this, unquoteOp.Loc);
                        Eval(f, unquoteOp.Target);
                        PC++;
                        break;
                    }
                case OpCode.Unzip:
                    {
                        var unzipOp = (Ops.Unzip)op;
                        var pv = Get(unzipOp.Target).CastUnbox(Libs.Core.Pair, unzipOp.Loc);
                        if (unzipOp.Left is Register lr) Set(lr, pv.Item1);
                        if (unzipOp.Right is Register rr) Set(rr, pv.Item1);
                        PC++;
                        break;
                    }
            }
        }
    }

    public void Eval(Form.Queue target, Register result)
    {
        var skipLabel = new Label();
        Emit(Ops.Goto.Make(skipLabel));
        var startPC = EmitPC;
        target.Emit(this, result);
        Emit(Ops.Stop.Make());
        skipLabel.PC = EmitPC;
        var prevPC = PC;
        Eval(startPC);
        PC = prevPC;
    }

    public Value Eval(Form target)
    {
        Set(Result, Value._);
        Eval(target, Result);
        return Get(Result);
    }

    public void Eval(Form target, Register result) => Eval(new Form.Queue([target]), result);

    public Value Eval(string code)
    {
        var loc = new Loc("Eval");
        var forms = ReadForms(new Source(new StringReader(code)), ref loc);
        Set(Result, Value._);
        Eval(forms, Result);
        return Get(Result);
    }

    public void EvalUntil(PC endPC)
    {
        var prev = code[endPC];
        code[endPC] = Ops.Stop.Make();
        try { Eval(PC); }
        finally { code[endPC] = prev; }
        PC = Math.Min(PC, EmitPC - 1);
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
            if (Path.GetDirectoryName(p) is string d) loadPath = d;
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
                forms.Emit(this, Result);
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

    public (Value, Value)[] Restarts => restarts.Select(r => (Value.Make(Libs.Core.Sym, r.Item1), r.Item2)).ToArray();

    public readonly Register Result;
    public void RunDeferred(int offset, Loc loc)
    {
        foreach (var d in deferred[offset..].ToArray().Reverse())
            d.Call(this, 0, NextRegisterIndex, true, Result, loc);

        deferred.Trunc(offset);
    }

    public int Index(Register reg) => RegisterIndex(reg.FrameOffset, reg.Index);

    public void SetRegister(int frameOffset, int index, Value value) =>
        registers[RegisterIndex(frameOffset, index)] = value;

    public void Set(Register register, Value value) =>
        SetRegister(register.FrameOffset, register.Index, value);

    private void Eval(Ops.Benchmark op)
    {
        var bodyPC = PC + 1;
        for (var i = 0; i < op.Reps; i++) Eval(bodyPC);
        var t = Stopwatch.GetTimestamp();
        for (var i = 0; i < op.Reps; i++) Eval(bodyPC);
        var e = Stopwatch.GetElapsedTime(t).TotalMilliseconds;
        Set(op.Result, Value.Make(Core.Int, (int)e));
    }

    private void Eval(Ops.Check op)
    {
        var ev = Get(op.Expected);
        var av = Get(op.Actual);
        var dav = av;
        if (ev.Type == Core.Bit && av.Type != Core.Bit) { av = Value.Make(Core.Bit, (bool)av); }
        if (!av.Equals(ev)) { throw new EvalError($"Check failed: expected {ev.Dump(this)}, actual {dav.Dump(this)}!", op.Loc); }
        PC++;
    }

    private void Eval(Ops.OpenInputStream op)
    {
        StreamReader sr;
        sr = new StreamReader(Path.Combine(loadPath, op.Path));
        Set(op.Result, Value.Make(IO.InputStream, sr));
        PC++;
    }

    private void Eval(Ops.Try op)
    {
        BeginFrame(op.RegisterCount);
        PC++;

        try { EvalUntil(op.End.PC); }
        catch (UserError e)
        {
            if (!op.HandleError(this, e.Value, e.Loc)) { throw; }
        }
        catch (EvalError e)
        {
            if (!op.HandleError(this, Value.Make(Core.Error, e), e.Loc)) { throw; }
        }
    }
}