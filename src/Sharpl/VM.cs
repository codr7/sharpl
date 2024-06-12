namespace Sharpl;

using System.Drawing;
using System.Text;
using Sharpl.Libs;
using PC = int;

public class VM
{
    public struct C
    {
        public int MaxArgs = 16;
        public int MaxCalls = 128;
        public int MaxFrames = 248;
        public int MaxOps = 1024;
        public int MaxRegisters = 512;
        public int MaxStackSize = 32;

        public C() { }
    };

    public static readonly C DEFAULT_CONFIG = new C();
    public static readonly int VERSION = 1;

    public readonly Libs.Core CoreLib = new Libs.Core();
    public readonly Libs.String StringLib = new Libs.String();
    public readonly Libs.Term TermLib;
    public readonly Lib UserLib = new Lib("user", null);

    public readonly C Config;
    public PC PC = 0;
    public readonly Term Term = new Term();

    private readonly ArrayStack<Call> calls;
    private readonly ArrayStack<Op> code;
    private Env? env;
    private readonly ArrayStack<int> frames;
    private readonly List<Label> labels = new List<Label>();
    private string loadPath = "";
    private ArrayStack<Value> registers;

    private Reader[] readers = [
        Readers.WhiteSpace.Instance,

        Readers.Array.Instance,
        Readers.Call.Instance,
        Readers.Int.Instance,
        Readers.String.Instance,

        Readers.Id.Instance
    ];

    public VM(C config)
    {
        Config = config;
        calls = new ArrayStack<Call>(config.MaxCalls);
        code = new ArrayStack<Op>(config.MaxOps);
        frames = new ArrayStack<int>(config.MaxFrames);
        registers = new ArrayStack<Value>(config.MaxRegisters);

        TermLib = new Libs.Term(this);
        UserLib.BindLib(CoreLib);
        UserLib.BindLib(StringLib);
        UserLib.BindLib(TermLib);
        UserLib.BindLib(UserLib);

        Env = UserLib;
    }

    public int AllocRegister()
    {
        var result = registers.Count;
        registers.Push(Value.Nil);
        return result;
    }


    public void BeginFrame()
    {
        frames.Push(registers.Count);
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

    public void EndFrame()
    {
        frames.Pop();
    }

    public Env Env
    {
        get => env ?? UserLib;
        set => env = value;
    }

    public void Decode(PC startPC)
    {
        for (var pc = startPC; pc < code.Count; pc++)
        {
            Term.SetFg(Color.FromArgb(255, 128, 128, 255));
            Term.Write($"{pc,-4} {code[pc]}\n");
        }
    }

    public void Eval(PC startPC, Stack stack)
    {
        PC = startPC;

        while (true)
        {
            var op = code[PC];

            switch (op.Type)
            {
                case Op.T.BeginFrame:
                    {
                        BeginFrame();
                        PC++;
                        break;
                    }
                case Op.T.CallDirect:
                    {
                        var callOp = (Ops.CallDirect)op.Data;
                        var recursive = !calls.Empty && calls.Peek().Target.Equals(callOp.Target);
                        PC++;
                        callOp.Target.Call(callOp.Loc, this, stack, callOp.Arity);
                        break;
                    }
                case Op.T.CallIndirect:
                    {
                        var target = stack.Pop();
                        var callOp = (Ops.CallIndirect)op.Data;
                        PC++;
                        target.Call(callOp.Loc, this, stack, callOp.Arity);
                        break;
                    }
                case Op.T.CallMethod:
                    {
                        var callOp = (Ops.CallMethod)op.Data;
                        PC++;
                        callOp.Target.Call(callOp.Loc, this, stack, callOp.Arity);
                        break;
                    }
                case Op.T.CallUserMethod:
                    {
                        var callOp = (Ops.CallUserMethod)op.Data;
                        callOp.Target.Call(callOp.Loc, this, callOp.Arity, PC + 1);
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
                case Op.T.CreateArray:
                    {
                        var createOp = (Ops.CreateArray)op.Data;
                        stack.Push(Value.Make(Core.Array, new Value[createOp.Length]));
                        PC++;
                        break;
                    }
                case Op.T.EndFrame:
                    {
                        EndFrame();
                        PC++;
                        break;
                    }
                case Op.T.Enter:
                    {
                        var enterOp = (Ops.Enter)op.Data;
                        var t = enterOp.Target;

                        for (var i = t.Args.Length - 1; i >= 0; i--)
                        {
                            SetRegister(0, t.Args[i].Item2, stack.Pop());
                        }

                        PC++;
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
                case Op.T.Push:
                    {
                        var pushOp = (Ops.Push)op.Data;
                        stack.Push(pushOp.Value.Copy());
                        PC++;
                        break;
                    }
                case Op.T.Return:
                    {
                        EndFrame();
                        PC = calls.Pop().ReturnPC;
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

    public void Eval(Form form, Form.Queue args, Stack stack)
    {
        var skipLabel = new Label();
        Emit(Ops.Goto.Make(skipLabel));
        var startPC = EmitPC;
        form.Emit(this, args);
        Emit(Ops.Stop.Make());
        skipLabel.PC = EmitPC;
        Eval(startPC, stack);
    }

    public Value? Eval(Form form, Form.Queue args)
    {
        var stack = new Stack(Config.MaxStackSize);
        Eval(form, args, stack);
        return stack.Pop();
    }

    public Value GetRegister(int frameOffset, int index)
    {
        return registers[frames.Peek(frameOffset) + index];
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

            using (StreamReader source = new StreamReader(path, Encoding.UTF8))
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

    public void PopEnv()
    {
        if (env is null)
        {
            throw new Exception("No active env");
        }

        env = env.Parent;
    }

    public void PushCall(Loc loc, UserMethod target, PC returnPC)
    {
        calls.Push(new Call(loc, target, returnPC));
    }

    public void PushEnv()
    {
        env = new Env(env);
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
        return forms.Pop();
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
        registers[frames.Peek(frameOffset) + index] = value;
    }
}