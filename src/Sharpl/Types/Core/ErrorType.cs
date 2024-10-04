namespace Sharpl.Types.Core;

public class ErrorType(string name, AnyType[] parents) : Type<UserError>(name, parents) { }