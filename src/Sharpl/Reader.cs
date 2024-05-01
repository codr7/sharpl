namespace Sharpl;

public interface Reader {
    bool Read(TextReader source, VM vm, ref Loc loc, Form.Queue forms);
}