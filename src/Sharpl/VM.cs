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
        public int MaxRegisters = 1024;
        public int MaxVars = 128;
        public C() { }
    };

    public static readonly C DEFAULT = new C();
    public static readonly int VERSION = 17;

    public readonly Core CoreLib = new Core();
    public readonly IO IOLib;
    public readonly String StringLib = new String();
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
    private readonly Value[] registers;
    private readonly List<int> splats = [];
    private readonly Dictionary<string, Sym> syms = [];

    private Reader[] readers = [
        Readers.WhiteSpace.Instance,

        Readers.And.Instance,
        Readers.Array.Instance,
        Readers.Call.Instance,
        Readers.Fix.Instance,
        Readers.Int.Instance,
        Readers.Map.Instance,
        Readers.Pair.Instance,
        Readers.Quote.Instance,
        Readers.Splat.Instance,
        Readers.String.Instance,
        Readers.Unquote.Instance,

        Readers.Id.Instance
    ];

    public VM(C config)
    {
        Config = config;
        registers = new Value[config.MaxRegisters];
        nextRegisterIndex = 0;

        UserLib.Init(this);
        UserLib.Import(CoreLib);
        Env = UserLib;
        BeginFrame(config.MaxVars);

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

        if (frames.Count > 0)
        {
            total += frames[^1].Item2;
        }

        frames.Push((registerCount, total));
        nextRegisterIndex = 0;
    }

    public void CallUserMethod(Loc loc, Stack stack, UserMethod target, int arity, int registerCount)
    {
        BeginFrame(registerCount);
        calls.Push(new Call(loc, target, PC, frames.Count));
        target.BindArgs(this, arity, stack);
#pragma warning disable CS8629
        PC = (PC)target.StartPC;
#pragma warning restore CS8629
    }

    public Value Compose(Loc loc, Form left, Form right, Form.Queue args)
    {
        var m = new UserMethod(loc, this, $"{left} & {right}", [], [("values*", -1)], false);
        var skip = new Label();
        Emit(Ops.Goto.Make(skip));
        m.StartPC = EmitPC;
        Emit($"(return ({right} ({left} {args})))", loc);
        Emit(Ops.ExitMethod.Make());
        skip.PC = EmitPC;
        return Value.Make(Libs.Core.UserMethod, m);
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

    public void Emit(Form form, int quoted)
    {
        var fs = new Form.Queue();
        fs.Push(form);
        fs.Emit(this, quoted);
    }

    public void Emit(string code, Loc loc) => 
        ReadForms(new StringReader(code), ref loc).Emit(this, 0);

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
                    {
                        var benchmarkOp = (Ops.Benchmark)op.Data;
                        var bodyPC = PC + 1;
                        var s = new Stack();

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
                            PC = (PC)branchOp.Right.PC;
#pragma warning restore CS8629
                        }

                        break;
                    }
                case Op.T.CallDirect:
                    {
                        var callOp = (Ops.CallDirect)op.Data;
                        var recursive = calls.Count > 0 && calls.Peek().Target.Equals(callOp.Target);
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
                        var callOp = (Ops.CallTail)op.Data;
                        var arity = callOp.Arity;

                        if (callOp.Splat)
                        {
                            arity += splats.Pop();
                        }

                        if (arity < callOp.Target.MinArgCount)
                        {
                            throw new EvalError(callOp.Loc, $"Not enough arguments: {callOp.Target} {arity}");
                        }

                        var call = calls.Peek();
                        frames.Trunc(call.FrameOffset);
                        callOp.Target.BindArgs(this, callOp.Arity, stack);
#pragma warning disable CS8629
                        PC = (int)callOp.Target.StartPC;
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

                        if (v.Type is IterTrait it)
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
                        stack.Push(Value.Make(Core.Map, new OrderedMap<Value, Value>()));
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
                        var v = Value.Make(Core.Int, registers[i].CastUnbox(Core.Int) - 1);
                        registers[i] = v;
                        stack.Push(v);
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

                        foreach (var (id, s, _) in c.Target.Closure)
                        {
                            c.Target.ClosureValues[s] = GetRegister(0, s);
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
                case Op.T.Or:
                    {
                        var orOp = (Ops.Or)op.Data;

                        if ((bool)stack.Peek())
                        {
#pragma warning disable CS8629
                            PC = (PC)orOp.Done.PC;
#pragma warning restore CS8629
                        }
                        else
                        {
                            PC++;
                        }

                        break;
                    }
                case Op.T.PrepareClosure:
                    {
                        var closureOp = (Ops.PrepareClosure)op.Data;
                        var m = closureOp.Target;

                        foreach (var (id, d, s) in m.Closure)
                        {
                            m.ClosureValues[d] = Get(s);
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
                case Op.T.QuoteCall:
                    {
                        var quoteOp = (Ops.QuoteCall)op.Data;
                        var t = new Forms.Literal(quoteOp.Loc, stack.Pop());
                        var args = new Form[quoteOp.Arity];
                        for (int i = args.Length-1; i >= 0; i--) { args[i] = new Forms.Literal(quoteOp.Loc, stack.Pop()); }
                        stack.Push(Value.Make(Core.Form, (new Forms.Call(quoteOp.Loc, t, args), 0)));
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

                            if (tv.Type is Types.Core.IterTrait tt)
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

    public void Eval(PC startPC) => Eval(startPC, new Stack());

    public void Eval(Emitter target, Form.Queue args, Stack stack, int quoted)
    {
        var skipLabel = new Label();
        Emit(Ops.Goto.Make(skipLabel));
        var startPC = EmitPC;
        target.Emit(this, args, quoted);
        Emit(Ops.Stop.Make());
        skipLabel.PC = EmitPC;
        Eval(startPC, stack);
    }

    public Value? Eval(Emitter target, Form.Queue args, int quoted)
    {
        var stack = new Stack();
        Eval(target, args, stack, quoted);
        return (stack.Count == 0) ? null : stack.Pop();
    }

    public void Eval(Emitter target, Stack stack, int quoted) => 
        Eval(target, new Form.Queue(), stack, quoted);

    public Value? Eval(Emitter target, int quoted) => 
        Eval(target, new Form.Queue(), quoted);

    public Value? Eval(string code)
    {
        var loc = new Loc("Eval");
        var forms = ReadForms(new StringReader(code), ref loc);
        return Eval(forms, 0);
    }

    public int FrameCount => frames.Count;

    public Value Get(Register register) => GetRegister(register.FrameOffset, register.Index);

    public Value GetRegister(int frameOffset, int index) => 
        registers[RegisterIndex(frameOffset, index)];

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
                forms.Emit(this, 0);
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

    public bool ReadForm(TextReader source, ref Loc loc, Form.Queue forms)
    {
        foreach (var r in readers)
        {
            if (r.Read(source, this, ref loc, forms)) { return true; }
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

    public int RegisterIndex(int frameOffset, int index) => 
        (frameOffset == -1) ? index : index + frames.Peek(frameOffset).Item2;

    public void REPL()
    {
        Term.SetFg(Color.FromArgb(255, 252, 173, 3));
        Term.Write($"sharpl v{VERSION}\n\n");
        Term.Reset();

        var buffer = new StringBuilder();
        var stack = new Stack();
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
                    ReadForms(new StringReader(buffer.ToString()), ref loc).Emit(this, 0);
                    Emit(Ops.Stop.Make());
                    Eval(startPC, stack);

                    Term.SetFg(Color.FromArgb(255, 0, 255, 0));
                    Term.WriteLine(stack is [] ? Value.Nil : stack.Pop());
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

    public void SetRegister(int frameOffset, int index, Value value) =>
        registers[RegisterIndex(frameOffset, index)] = value;

    public void Set(Register register, Value value) =>
        SetRegister(register.FrameOffset, register.Index, value);
}