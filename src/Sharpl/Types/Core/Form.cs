namespace Sharpl.Types.Core;

public class FormType : Type<Form>
{
    public FormType(string name) : base(name) { }


    public override bool Equals(Value left, Value right)
    {
        return left.Cast(this).Equals(right.Cast(this));
    }
}