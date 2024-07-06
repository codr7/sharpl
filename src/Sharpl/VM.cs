namespace Sharpl;

using System.Diagnostics;
using System.Drawing;
using System.Text;
using Sharpl.Libs;
using Sharpl.Types.Core;
using PC = int;

public class VM
{
    public struct C
    {
        public int MaxArgs = 16;
        public int MaxCalls = 128;
        public int MaxDefinitions = 128;
        public int MaxFrames = 248;
        public int MaxOps = 1024;
        public int MaxRegisters = 1024;
        public int MaxSplats = 16;
        public int MaxStackSize = 32;

        public C() { }
    };

    public static readonly C DEFAULT_CONFIG = new C();
    public static readonly int VERSION = 7;

    public readonly Libs.Core CoreLib = new Libs.Core();
    public readonly Libs.IO IOLib;
    public readonly Libs.String StringLib = new Libs.String();
    public readonly Libs.Term TermLib;
    public readonly Lib UserLib = new Lib("user", null, []);

    public readonly C Config;
    public PC PC = 0;
    public readonly Term Term = new Term();

    private readonly ArrayStack<Call> calls;
    private readonly ArrayStack<Op> code;
    private int definitionCount = 0;
    private Env? env;
    private readonly ArrayStack<(int, int)> frames;
    private readonly List<Label> labels = new List<Label>();
    private string loadPath = "";
    private int nextRegisterIndex = 0;
    private Value[] registers;
    private ArrayStack<int> splats;
    private Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

    private Reader[] readers = [
        Readers.WhiteSpace.Instance,

        Readers.Array.Instance,
        Readers.Call.Instance,
        Readers.Int.Instance,
        Readers.Pair.Instance,
        Readers.Quote.Instance,
        Readers.Splat.Instance,
        Readers.String.Instance,

        Readers.Id.Instance
    ];

    public VM(C config)
    {
        Config = config;
        calls = new ArrayStack<Call>(config.MaxCalls);
        code = new ArrayStack<Op>(config.MaxOps);
        frames = new ArrayStack<(int, int)>(config.MaxFrames);
        registers = new Value[config.MaxRegisters];
        splats = new ArrayStack<int>(config.MaxSplats);
        nextRegisterIndex = 0;

        UserLib.Init(this);
        UserLib.Import(CoreLib);
        Env = UserLib;
        BeginFrame(config.MaxDefinitions);

        IOLib = new IO(this);
        IOLib.Init(this);

        StringLib.Init(this);

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

        if (!frames.Empty)
        {
            total += frames.Last().Item2;
        }

        frames.Push((registerCount, total));
        nextRegisterIndex = 0;
    }

    public void CallUserMethod(Loc loc, Stack stack, UserMethod target, int arity, int registerCount)
    {
        if (arity < target.Args.Length)
        {
            throw new EvalError(loc, $"Not enough arguments: {target}");
        }

        BeginFrame(registerCount);
        calls.Push(new Call(loc, target, PC, frames.Count));
        target.BindArgs(this, arity, stack);
#pragma warning disable CS8629
        PC = (PC)target.StartPC;
#pragma warning restore CS8629
    }

    public void Decode(PC startPC)
    {
        for (var pc = startPC; pc < code.Count; pc++)
        {
            Term.SetFg(Color.FromArgb(255, 128, 128, 255));
            Term.Write($"{pc - startPC + 1,-4} {code[pc]}\n");
        }
    }

    public void Define(string name)
    {
        var i = definitionCount;
        Env[name] = Value.Make(Core.Binding, new Register(-1, i));
        Emit(Ops.SetRegister.Make(-1, i));
        definitionCount++;
    }

    public void DoEnv(Env env, Action action)
    {
        var prevEnv = Env;
        Env = env;
        BeginFrame(nextRegisterIndex);

        try
        {
            action();
        }
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

    public PC EmitPC
    {
        get { return code.Count; }
    }

    public (int, int) EndFrame()
    {
        return frames.Pop();
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
            //Console.WriteLine(op);

            switch (op.Type)
            {
                case Op.T.AddMapItem:
                    {
                        var v = stack.Pop();
                        var k = stack.Pop();
                        stack.Peek().Cast(Core.Map)[k] = v;
                        PC++;
                        break;
                    }

                case Op.T.BeginFrame:
                    {
                        var beginOp = (Ops.BeginFrame)op.Data;
                        BeginFrame(beginOp.RegisterCount);
                        PC++;
                        break;
                    }
                case Op.T.Benchmark:
                    {
                        var benchmarkOp = (Ops.Benchmark)op.Data;
                        var bodyPC = PC + 1;
                        var s = new Stack(Config.MaxStackSize);

                        for (var i = 0; i < benchmarkOp.N; i++)
                        {
                            Eval(bodyPC, s);
                            s.Clear();
                        }

                        var t = new Stopwatch();
                        t.Start();

                        for (var i = 0; i < benchmarkOp.N; i++)
                        {
                            Eval(bodyPC, s);
                            s.Clear();
                        }

                        t.Stop();
                        stack.Push(Value.Make(Core.Int, (int)t.ElapsedMilliseconds));
                        break;
                    }
                case Op.T.Branch:
                    {
                        var branchOp = (Ops.Branch)op.Data;

                        if ((bool)stack.Pop())
                        {
                            PC++;
                        }
                        else
                        {
#pragma warning disable CS8629
                            PC = (PC)branchOp.Target.PC;
#pragma warning restore CS8629
                        }

                        break;
                    }
                case Op.T.CallDirect:
                    {
                        var callOp = (Ops.CallDirect)op.Data;
                        var recursive = !calls.Empty && calls.Peek().Target.Equals(callOp.Target);
                        var arity = callOp.Arity;

                        if (callOp.Splat)
                        {
                            arity = arity + splats.Pop() - 1;
                        }

                        PC++;
                        callOp.Target.Call(callOp.Loc, this, stack, arity, callOp.RegisterCount);
                        break;
                    }
                case Op.T.CallMethod:
                    {
                        var callOp = (Ops.CallMethod)op.Data;
                        var arity = callOp.Arity;

                        if (callOp.Splat)
                        {
                            arity = arity + splats.Pop() - 1;
                        }

                        PC++;
                        callOp.Target.Call(callOp.Loc, this, stack, arity);
                        break;
                    }
                case Op.T.CallRegister:
                    {
                        var callOp = (Ops.CallRegister)op.Data;
                        var arity = callOp.Arity;

                        if (callOp.Splat)
                        {
                            arity = arity + splats.Pop() - 1;
                        }

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

                        if (callOp.Splat)
                        {
                            arity = arity + splats.Pop() - 1;
                        }

                        PC++;
                        target.Call(callOp.Loc, this, stack, arity, callOp.RegisterCount);
                        break;
                    }
                case Op.T.CallTail:
                    {
                        var bindOp = (Ops.CallTail)op.Data;
                        var arity = bindOp.Arity;

                        if (bindOp.Splat)
                        {
                            arity += splats.Pop();
                        }

                        if (arity < bindOp.Target.Args.Length)
                        {
                            throw new EvalError(bindOp.Loc, $"Not enough arguments: {bindOp.Target} {arity}");
                        }

                        var frameCount = frames.Count - calls.Peek().FrameOffset;
                        var call = calls.Peek();
                        frames.Trunc(call.FrameOffset);
                        bindOp.Target.BindArgs(this, bindOp.Arity, stack);
#pragma warning disable CS8629
                        PC = (int)bindOp.Target.StartPC;
#pragma warning restore CS8629
                        break;
                    }
                case Op.T.CallUserMethod:
                    {
                        var callOp = (Ops.CallUserMethod)op.Data;
                        var arity = callOp.Arity;

                        if (callOp.Splat)
                        {
                            arity = arity + splats.Pop() - 1;
                        }

                        PC++;
                        CallUserMethod(callOp.Loc, stack, callOp.Target, arity, callOp.RegisterCount);
                        break;
                    }
                case Op.T.Check:
                    {
                        var checkOp = (Ops.Check)op.Data;

                        if (stack.Pop() is Value ev)
                        {
                            if (stack.Pop() is Value av)
                            {
                                if (!av.Equals(ev))
                                {
                                    throw new EvalError(checkOp.Loc, $"Check failed: expected {checkOp.Expected}, actual {av}!");
                                }
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
                        break;
                    }
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

                        if (v.Type is IterableTrait it)
                        {
                            Set(createOp.Target, Value.Make(Core.Iter, it.CreateIter(v)));
                        }
                        else
                        {
                            throw new EvalError(createOp.Loc, $"Not iterable: {v}");
                        }

                        PC++;
                        break;
                    }
                case Op.T.CreateMap:
                    {
                        var createOp = (Ops.CreateMap)op.Data;
                        stack.Push(Value.Make(Core.Map, new OrderedMap<Value, Value>(createOp.Length)));
                        PC++;
                        break;
                    }
                case Op.T.CreatePair:
                    {
                        var createOp = (Ops.CreatePair)op.Data;
                        var r = stack.Pop();
                        var l = stack.Pop();
                        stack.Push(Value.Make(Core.Pair, (l, r)));
                        PC++;
                        break;
                    }
                case Op.T.Decrement:
                    {
                        var decrementOp = (Ops.Decrement)op.Data;
                        var i = RegisterIndex(decrementOp.FrameOffset, decrementOp.Index);
                        var v = Value.Make(Core.Int, registers[i].Cast(Core.Int) - 1);
                        registers[i] = v;
                        stack.Push(v);
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

                        foreach (var (s, (d, v)) in c.Target.Closure)
                        {
                            SetRegister(s.FrameOffset, s.Index, GetRegister(0, d));
                        }

                        EndFrame();
                        PC = c.ReturnPC;
                        break;
                    }
                case Op.T.GetRegister:
                    {
                        var getOp = (Ops.GetRegister)op.Data;
                        stack.Push(GetRegister(getOp.FrameOffset, getOp.Index));
                        PC++;
                        break;
                    }
                case Op.T.Goto:
                    {
                        var gotoOp = (Ops.Goto)op.Data;
#pragma warning disable CS8629
                        PC = (PC)gotoOp.Target.PC;
#pragma warning restore CS8629
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
                        else
                        {
#pragma warning disable CS8629 // Nullable value type may be null.
                            PC = (int)iterOp.Done.PC;
#pragma warning restore CS8629 // Nullable value type may be null.
                        }

                        break;
                    }
                case Op.T.OpenInputStream:
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
                        break;
                    }
                case Op.T.PrepareClosure:
                    {
                        var closureOp = (Ops.PrepareClosure)op.Data;
                        var m = closureOp.Target;

                        foreach (var (s, (d, v)) in m.Closure)
                        {
                            var rv = GetRegister(s.FrameOffset - 1, s.Index);
                            m.Closure[s] = (d, rv);
                        }

                        PC++;
                        break;
                    }

                case Op.T.Push:
                    {
                        var pushOp = (Ops.Push)op.Data;
                        stack.Push(pushOp.Value.Copy());
                        PC++;
                        break;
                    }
                case Op.T.PushSplat:
                    {
                        splats.Push(0);
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
                case Op.T.SetRegister:
                    {
                        var setOp = (Ops.SetRegister)op.Data;
                        SetRegister(setOp.FrameOffset, setOp.Index, stack.Pop());
                        PC++;
                        break;
                    }
                case Op.T.Splat:
                    {
                        var splatOp = (Ops.Splat)op.Data;

                        if (stack.Count == 0)
                        {
                            throw new EvalError(splatOp.Loc, "Missing splat target");
                        }
                        else
                        {
                            var tv = stack.Pop();

                            if (tv.Type is Types.Core.IterableTrait tt)
                            {
                                if (splats.Count == 0)
                                {
                                    throw new EvalError(splatOp.Loc, "Splat outside context");
                                }

                                var arity = splats.Pop();

                                foreach (var v in tt.CreateIter(tv))
                                {
                                    stack.Push(v);
                                    arity++;
                                }

                                splats.Push(arity);
                            }
                            else
                            {
                                throw new EvalError(splatOp.Loc, $"Invalid splat target: {tv}");
                            }
                        }

                        PC++;
                        break;
                    }
                case Op.T.Stop:
                    {
                        PC++;
                        return;
                    }
            }
        }
    }

    public void Eval(PC startPC)
    {
        Eval(startPC, new Stack(Config.MaxStackSize));
    }

    public void Eval(Emitter target, Form.Queue args, Stack stack)
    {
        var skipLabel = new Label();
        Emit(Ops.Goto.Make(skipLabel));
        var startPC = EmitPC;
        target.Emit(this, args);
        Emit(Ops.Stop.Make());
        skipLabel.PC = EmitPC;
        Eval(startPC, stack);
    }

    public Value? Eval(Emitter target, Form.Queue args)
    {
        var stack = new Stack(Config.MaxStackSize);
        Eval(target, args, stack);
        return (stack.Count == 0) ? null : stack.Pop();
    }

    public void Eval(Emitter target, Stack stack)
    {
        Eval(target, new Form.Queue(), stack);
    }

    public Value? Eval(Emitter target)
    {
        return Eval(target, new Form.Queue());
    }

    public Value? Eval(string code)
    {
        var loc = new Loc("Eval");
        var forms = ReadForms(new StringReader(code), ref loc);
        return Eval(forms);
    }

    public int FrameCount
    {
        get => frames.Count;
    }

    public Value Get(Register register)
    {
        return GetRegister(register.FrameOffset, register.Index);
    }

    public Value GetRegister(int frameOffset, int index)
    {
        return registers[RegisterIndex(frameOffset, index)];
    }

    public Symbol GetSymbol(string name)
    {
        if (symbols.ContainsKey(name))
        {
            return symbols[name];
        }

        var s = new Symbol(name);
        symbols[name] = s;
        return s;
    }

    public Label Label(PC pc = -1)
    {
        var l = new Label(pc);
        labels.Append(l);
        return l;
    }

    public Lib Lib
    {
        get
        {
            for (Env? e = Env; e is Env; e = e.Parent)
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

    public int NextRegisterIndex
    {
        get => nextRegisterIndex;
    }

    public bool ReadForm(TextReader source, ref Loc loc, Form.Queue forms)
    {
        foreach (var r in readers)
        {
            if (r.Read(source, this, ref loc, forms))
            {
                return true;
            }
        }

        return false;
    }

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

    public int RegisterIndex(int frameOffset, int index)
    {
        if (frameOffset == -1)
        {
            return index;
        }

        return index + frames.Peek(frameOffset).Item2;
    }

    public void REPL()
    {
        Term.SetFg(Color.FromArgb(255, 252, 173, 3));
        Term.Write($"sharpl v{VERSION}\n\n");
        Term.Reset();

        var buffer = new StringBuilder();
        var stack = new Stack(32);
        var loc = new Loc("repl");
        var bufferLines = 0;

        while (true)
        {
            Term.SetFg(Color.FromArgb(255, 128, 128, 128));
            Term.Write($"{(loc.Line + bufferLines),4} ");
            Term.Reset();
            Term.Flush();

            var line = Console.In.ReadLine();

            if (line is null)
            {
                break;
            }

            if (line == "")
            {
                var startPC = EmitPC;

                try
                {
                    ReadForms(new StringReader(buffer.ToString()), ref loc).Emit(this);
                    Emit(Ops.Stop.Make());
                    Eval(startPC, stack);

                    Term.SetFg(Color.FromArgb(255, 0, 255, 0));
                    Term.WriteLine(stack.Empty ? Value.Nil : stack.Pop());
                    Term.Reset();
                }
                catch (Exception e)
                {
                    Term.SetFg(Color.FromArgb(255, 255, 0, 0));
                    Term.WriteLine(e);
                    Term.Reset();
                }
                finally
                {
                    buffer.Clear();
                    bufferLines = 0;
                }

                Term.Write("\n");
            }
            else
            {
                buffer.Append(line);
                buffer.AppendLine();
                bufferLines++;
            }
        }
    }

    public void SetRegister(int frameOffset, int index, Value value)
    {
        registers[RegisterIndex(frameOffset, index)] = value;
    }

    public void Set(Register register, Value value)
    {
        SetRegister(register.FrameOffset, register.Index, value);
    }
}