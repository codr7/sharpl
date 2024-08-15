using Sharpl;

public interface Emitter
{
    void Emit(VM vm, Form.Queue args);
}