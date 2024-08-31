using System.Diagnostics;
using System.Drawing;
using System.Text;
using Sharpl.Libs;
using Sharpl.Types.Core;

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
            Readers.Quote.Instance,
            Readers.Splat.Instance,
            Readers.String.Instance,
            Readers.Unquote.Instance,

            Readers.Id.Instance
        ]);

        public C() {}
    };

    public static readonly C DEFAULT = new C();
    public static readonly int VERSION = 23;

    public readonly Libs.Char CharLib;
    public readonly Libs.Core CoreLib = new Libs.Core();
    public readonly Libs.IO IOLib;
    public readonly Libs.Iter IterLib;
    public readonly Libs.Json JsonLib;
    public readonly Libs.Net NetLib;
    public readonly Libs.String StringLib;
    public readonly Libs.Term TermLib;
    public readonly Lib UserLib = new Lib("user", null, []);

    public readonly C Config;
    public PC PC = 0;
    public readonly Term Term = new Term();

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

        JsonLib = new Libs.Json();
        JsonLib.Init(this);

        IOLib = new Libs.IO(this);
        IOLib.Init(this);

        NetLib = new Libs.Net();
        NetLib.Init(this);

        TermLib = new Libs.Term(this);
        TermLib.Init(this);
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
        calls.Push(new Call(loc, target, PC, frames.Count));
        target.BindArgs(this, argMask, arity, stack);
#pragma warning disable CS8629
        PC = (PC)target.StartPC;
#pragma warning restore CS8629
    }

    public Value Compose(Loc loc, Form left, Form right, Form.Queue args)
    {
        var m = new UserMethod(loc, this, $"{left.Dump(this)} & {right.Dump(this)}", [], [("values*", -1)], false);
        var skip = new Label();
        Emit(Ops.Goto.Make(skip));
        m.StartPC = EmitPC;
        Emit($"(return ({right.Dump(this)} ({left.Dump(this)} {args.Dump(this)})))", loc);
        Emit(Ops.ExitMethod.Make());
        skip.PC = EmitPC;
        return Value.Make(Libs.Core.UserMethod, m);
    }

    public void Define(string name)
    {
        var i = definitionCount;
        Env[name] = Value.Make(Core.Binding, new Register(-1, i));
        Emit(Ops.SetRegister.Make(new Register(-1, i)));
        definitionCount++;
    }

    public void Demit(PC startPC)
    {
        Term.SetFg(Color.FromArgb(255, 128, 128, 255));
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

    public void Emit(Form form)
    {
        var fs = new Form.Queue();
        fs.Push(form);
        fs.Emit(this, new Form.Queue());
    }

    public void Emit(string code, Loc loc) =>
        ReadForms(new StringReader(code), ref loc).Emit(this, new Form.Queue());

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
            var op = code[PC];
            //Console.WriteLine(op);

            switch (op.Type)
            {
                case Op.T.BeginFrame:
                    {
                        var beginOp = (Ops.BeginFrame)op.Data;
                        BeginFrame(beginOp.RegisterCount);
                        PC++;
                        break;
                    }
                case Op.T.Benchmark:
                    BENCHMARK(op, stack);
                    break;
                case Op.T.Branch:
                    {
                        var branchOp = (Ops.Branch)op.Data;

                        if (stack.TryPop(out var v))
                        {
                            if ((bool)v) { PC++; }
                            else { PC = branchOp.Right.PC; }
                        }
                        else { throw new EvalError(branchOp.Loc, "Missing condition"); }

                        break;
                    }
                case Op.T.CallDirect:
                    {
                        var callOp = (Ops.CallDirect)op.Data;
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        callOp.Target.Call(callOp.Loc, this, stack, arity, callOp.RegisterCount);
                        break;
                    }
                case Op.T.CallMethod:
                    {
                        var callOp = (Ops.CallMethod)op.Data;
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        callOp.Target.Call(callOp.Loc, this, stack, arity);
                        break;
                    }
                case Op.T.CallRegister:
                    {
                        var callOp = (Ops.CallRegister)op.Data;
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        var target = Get(callOp.Target);
                        target.Call(callOp.Loc, this, stack, arity, callOp.RegisterCount);
                        break;
                    }
                case Op.T.CallStack:
                    {
                        var target = stack.Pop();
                        var callOp = (Ops.CallStack)op.Data;
                        var arity = callOp.Arity;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        target.Call(callOp.Loc, this, stack, arity, callOp.RegisterCount);
                        break;
                    }
                case Op.T.CallTail:
                    {
                        var callOp = (Ops.CallTail)op.Data;
                        var arity = callOp.ArgMask.Length;
                        if (callOp.Splat) { arity += splats.Pop(); }
                        if (arity < callOp.Target.MinArgCount) { throw new EvalError(callOp.Loc, $"Not enough arguments: {callOp.Target} {arity}"); }
                        var call = calls.Peek();
                        frames.Trunc(call.FrameOffset);
                        callOp.Target.BindArgs(this, callOp.ArgMask, arity, stack);
#pragma warning disable CS8629 
                        PC = (int)callOp.Target.StartPC;
#pragma warning restore CS8629
                        break;
                    }
                case Op.T.CallUserMethod:
                    {
                        var callOp = (Ops.CallUserMethod)op.Data;
                        var arity = callOp.ArgMask.Length;
                        if (callOp.Splat) { arity = arity + splats.Pop() - 1; }
                        PC++;
                        CallUserMethod(callOp.Loc, stack, callOp.Target, callOp.ArgMask, arity, callOp.RegisterCount);
                        break;
                    }
                case Op.T.Check:
                    CHECK(op, stack);
                    break;
                case Op.T.CopyRegister:
                    {
                        var copyOp = (Ops.CopyRegister)op.Data;
                        var v = GetRegister(copyOp.FromFrameOffset, copyOp.FromIndex);
                        SetRegister(copyOp.ToFrameOffset, copyOp.ToIndex, v);
                        PC++;
                        break;
                    }
                case Op.T.CreateArray:
                    {
                        var createOp = (Ops.CreateArray)op.Data;
                        stack.Push(Value.Make(Core.Array, new Value[createOp.Length]));
                        PC++;
                        break;
                    }
                case Op.T.CreateIter:
                    {
                        var createOp = (Ops.CreateIter)op.Data;
                        var v = stack.Pop();
                        if (v.Type is IterTrait it) { Set(createOp.Target, Value.Make(Core.Iter, it.CreateIter(v))); }
                        else { throw new EvalError(createOp.Loc, $"Not iterable: {v}"); }
                        PC++;
                        break;
                    }
                case Op.T.CreateList:
                    {
                        var createOp = (Ops.CreateList)op.Data;
                        Set(createOp.Target, Value.Make(Core.List, new List<Value>()));
                        PC++;
                        break;
                    }
                case Op.T.CreateMap:
                    {
                        stack.Push(Value.Make(Core.Map, new OrderedMap<Value, Value>()));
                        PC++;
                        break;
                    }
                case Op.T.CreatePair:
                    {
                        var r = stack.Pop();
                        var l = stack.Pop();
                        stack.Push(Value.Make(Core.Pair, (l, r)));
                        PC++;
                        break;
                    }
                case Op.T.Decrement:
                    {
                        var decrementOp = (Ops.Decrement)op.Data;
                        var i = Index(decrementOp.Target);
                        var v = Value.Make(Core.Int, registers[i].CastUnbox(Core.Int) - 1);
                        registers[i] = v;
                        PC++;
                        break;
                    }
                case Op.T.Drop:
                    {
                        var dropOp = (Ops.Drop)op.Data;
                        stack.Drop(dropOp.Count);
                        PC++;
                        break;
                    }
                case Op.T.EndFrame:
                    {
                        EndFrame();
                        PC++;
                        break;
                    }
                case Op.T.ExitMethod:
                    {
                        var c = calls.Pop();
                        foreach (var (_, s, _) in c.Target.Closure) { c.Target.ClosureValues[s] = GetRegister(0, s); }
                        EndFrame();
                        PC = c.ReturnPC;
                        break;
                    }
                case Op.T.GetRegister:
                    {
                        var getOp = (Ops.GetRegister)op.Data;
                        stack.Push(Get(getOp.Target));
                        PC++;
                        break;
                    }
                case Op.T.Goto:
                    {
                        var gotoOp = (Ops.Goto)op.Data;
                        PC = gotoOp.Target.PC;
                        break;
                    }
                case Op.T.Increment:
                    {
                        var incrementOp = (Ops.Increment)op.Data;
                        var i = Index(incrementOp.Target);
                        var v = Value.Make(Core.Int, registers[i].CastUnbox(Core.Int) + 1);
                        registers[i] = v;
                        PC++;
                        break;
                    }
                case Op.T.IterNext:
                    {
                        var iterOp = (Ops.IterNext)op.Data;

                        if (Get(iterOp.Iter).Cast(Core.Iter).Next() is Value v)
                        {
                            stack.Push(v);
                            PC++;
                        }
                        else { PC = iterOp.Done.PC; }

                        break;
                    }
                case Op.T.OpenInputStream:
                    OPEN_INPUT_STREAM(op, stack);
                    break;
                case Op.T.Or:
                    {
                        var orOp = (Ops.Or)op.Data;
                        if ((bool)stack.Peek()) { PC = orOp.Done.PC; }
                        else { PC++; }
                        break;
                    }
                case Op.T.PopItem:
                    {
                        var popOp = (Ops.PopItem)op.Data;
                        var t = Get(popOp.Target);
                        if (t.Type is StackTrait st) { stack.Push(st.Pop(popOp.Loc, this, popOp.Target, t)); }
                        else { throw new EvalError(popOp.Loc, $"Invalid target: {t}"); }
                        PC++;
                        break;
                    }
                case Op.T.PrepareClosure:
                    {
                        var closureOp = (Ops.PrepareClosure)op.Data;
                        var m = closureOp.Target;
                        foreach (var (_, d, s) in m.Closure) { m.ClosureValues[d] = Get(s); }
                        PC = closureOp.Skip.PC;
                        break;
                    }
                case Op.T.Push:
                    {
                        var pushOp = (Ops.Push)op.Data;
                        stack.Push(pushOp.Value.Copy());
                        PC++;
                        break;
                    }
                case Op.T.PushItem:
                    {
                        var pushOp = (Ops.PushItem)op.Data;

                        if (stack.TryPop(out var v))
                        {
                            var t = Get(pushOp.Target);
                            if (t.Type is StackTrait st) { st.Push(pushOp.Loc, this, pushOp.Target, t, v); }
                            else { throw new EvalError(pushOp.Loc, $"Invalid target: {t}"); }
                        }
                        else { throw new EvalError(pushOp.Loc, "Missing value"); }

                        PC++;
                        break;
                    }
                case Op.T.PushSplat:
                    {
                        splats.Push(0);
                        PC++;
                        break;
                    }
                case Op.T.Repush:
                    {
                        var pushOp = (Ops.Repush)op.Data;
                        var v = stack.Peek();
                        for (var i = 0; i < pushOp.N; i++) { stack.Push(v); }
                        PC++;
                        break;
                    }
                case Op.T.SetArrayItem:
                    {
                        var setOp = (Ops.SetArrayItem)op.Data;
                        var v = stack.Pop();
                        stack.Peek().Cast(Core.Array)[setOp.Index] = v;
                        PC++;
                        break;
                    }
                case Op.T.SetLoadPath:
                    {
                        var setOp = (Ops.SetLoadPath)op.Data;
                        loadPath = setOp.Path;
                        PC++;
                        break;
                    }
                case Op.T.SetMapItem:
                    {
                        var v = stack.Pop();
                        var k = stack.Pop();
                        stack.Peek().Cast(Core.Map)[k] = v;
                        PC++;
                        break;
                    }
                case Op.T.SetRegister:
                    {
                        var setOp = (Ops.SetRegister)op.Data;
                        Set(setOp.Target, stack.Pop());
                        PC++;
                        break;
                    }
                case Op.T.Splat:
                    {
                        var splatOp = (Ops.Splat)op.Data;

                        if (stack.Count == 0) { throw new EvalError(splatOp.Loc, "Missing splat target"); }
                        else
                        {
                            var tv = stack.Pop();

                            if (tv.Type is Types.Core.IterTrait tt)
                            {
                                if (splats.Count == 0) { throw new EvalError(splatOp.Loc, "Splat outside context"); }
                                var arity = splats.Pop();

                                foreach (var v in tt.CreateIter(tv))
                                {
                                    stack.Push(v);
                                    arity++;
                                }

                                splats.Push(arity);
                            }
                            else { throw new EvalError(splatOp.Loc, $"Invalid splat target: {tv}"); }
                        }

                        PC++;
                        break;
                    }
                case Op.T.Stop:
                    {
                        PC++;
                        return;
                    }
                case Op.T.Swap:
                    {
                        var x = stack.Pop();
                        var y = stack.Pop();
                        stack.Push(x);
                        stack.Push(y);
                        PC++;
                        break;
                    }
                case Op.T.UnquoteRegister:
                    {
                        var unquoteOp = (Ops.UnquoteRegister)op.Data;
                        var f = Get(unquoteOp.Register).Unquote(unquoteOp.Loc, this);
                        Eval(f, stack);
                        PC++;
                        break;
                    }
                case Op.T.Unzip:
                    {
                        var unzipOp = (Ops.Unzip)op.Data;

                        if (stack.TryPop(out var p))
                        {
                            var pv = p.CastUnbox(Core.Pair);
                            stack.Push(pv.Item1);
                            stack.Push(pv.Item2);
                        }
                        else { throw new EvalError(unzipOp.Loc, "Missing target"); }

                        PC++;
                        break;
                    }
            }
        }
    }

    public void Eval(PC startPC) => Eval(startPC, new Stack());

    public void Eval(Emitter target, Form.Queue args, Stack stack)
    {
        var skipLabel = new Label();
        Emit(Ops.Goto.Make(skipLabel));
        var startPC = EmitPC;
        target.Emit(this, args);
        Emit(Ops.Stop.Make());
        skipLabel.PC = EmitPC;
        var prevPC = PC;
        Eval(startPC, stack);
        PC = prevPC;
    }

    public Value? Eval(Emitter target, Form.Queue args)
    {
        var stack = new Stack();
        Eval(target, args, stack);
        return (stack.Count == 0) ? null : stack.Pop();
    }

    public void Eval(Emitter target, Stack stack) =>
        Eval(target, new Form.Queue(), stack);

    public Value? Eval(Emitter target) =>
        Eval(target, new Form.Queue());

    public Value? Eval(string code)
    {
        var loc = new Loc("Eval");
        var forms = ReadForms(new StringReader(code), ref loc);
        return Eval(forms);
    }

    public int FrameCount => frames.Count;

    public Value Get(Register register) => GetRegister(register.FrameOffset, register.Index);
    
    public int GetObjectId(object it) {
        if (!objectIds.ContainsKey(it)) {
            var id = objectIds.Count+1;
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
            if (Path.GetDirectoryName(p) is string d)
            {
                loadPath = d;
            }

            var loc = new Loc(path);

            using (StreamReader source = new StreamReader(p, Encoding.UTF8))
            {
                var c = source.Peek();

                if (c == '#')
                {
                    source.ReadLine();
                    loc.NewLine();
                }

                var forms = ReadForms(source, ref loc);
                Emit(Ops.SetLoadPath.Make(loadPath));
                forms.Emit(this, new Form.Queue());
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

    public bool ReadForm(TextReader source, ref Loc loc, Form.Queue forms) =>
        Config.Reader.Read(source, this, ref loc, forms);

    public Form? ReadForm(TextReader source, ref Loc loc)
    {
        var forms = new Form.Queue();
        ReadForm(source, ref loc, forms);
        return forms.TryPop();
    }

    public void ReadForms(TextReader source, ref Loc loc, Form.Queue forms)
    {
        while (ReadForm(source, ref loc, forms)) { }
    }

    public Form.Queue ReadForms(TextReader source, ref Loc loc)
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

    private void BENCHMARK(Op op, Stack stack)
    {
        var benchmarkOp = (Ops.Benchmark)op.Data;
        var bodyPC = PC + 1;
        var s = new Stack();

        for (var i = 0; i < benchmarkOp.N; i++)
        {
            Eval(bodyPC, s);
            s.Clear();
        }

        var t = Stopwatch.GetTimestamp();

        for (var i = 0; i < benchmarkOp.N; i++)
        {
            Eval(bodyPC, s);
            s.Clear();
        }

        var e = Stopwatch.GetElapsedTime(t).TotalMilliseconds;
        stack.Push(Value.Make(Core.Int, (int)e));
    }

    private void CHECK(Op op, Stack stack)
    {
        var checkOp = (Ops.Check)op.Data;

        if (stack.Pop() is Value ev)
        {
            if (stack.Pop() is Value av)
            {
                var dav = av;
                if (ev.Type == Core.Bit && av.Type != Core.Bit) { av = Value.Make(Core.Bit, (bool)av); }
                if (!av.Equals(ev)) { throw new EvalError(checkOp.Loc, $"Check failed: expected {ev}, actual {dav}!"); }
            }
            else
            {
                throw new EvalError(checkOp.Loc, "Missing actual value");
            }
        }
        else
        {
            throw new EvalError(checkOp.Loc, "Missing expected value");
        }

        PC++;
    }

    private void OPEN_INPUT_STREAM(Op op, Stack stack)
    {
        var openOp = (Ops.OpenInputStream)op.Data;
        StreamReader sr;

        if (stack.Pop() is Value p)
        {
            sr = new StreamReader(Path.Combine(loadPath, p.Cast(openOp.Loc, Core.String)));
            SetRegister(openOp.FrameOffset, openOp.Index, Value.Make(IO.InputStream, sr));
        }
        else
        {
            throw new EvalError(openOp.Loc, "Missing path");
        }

        PC++;
    }
}