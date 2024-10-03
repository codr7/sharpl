namespace Sharpl.Types.Core;

public class ErrorType(string name, AnyType[] parents) : Type<Exception>(name, parents) { }