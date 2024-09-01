namespace Sharpl;

public interface Port {
    public Value Read();
    public void Write(Value value);
}