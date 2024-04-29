namespace Sharpl {
public class AnyType {
    public string Name { get; }

    protected AnyType(string name) {
        Name = name;
    }

}

public class Type<T>: AnyType {
    public Type(string name): base(name) {}    
}
}