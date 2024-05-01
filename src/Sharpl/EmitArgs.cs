namespace Sharpl;

public class EmitArgs
{
    private LinkedList<Form> items = new LinkedList<Form>();

    public int Count { get { return items.Count; } }

    public Form? Pop()
    {
           if (items.First?.Value is Form f) {
                return f;
            } 

            return null;
    }

    public void Push(Form form) {
        items.Append(form);
    }
}