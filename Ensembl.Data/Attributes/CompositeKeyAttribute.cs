
namespace Ensembl.Data.Attributes;

// https://learn.microsoft.com/en-us/answers/questions/1490921/how-to-set-up-a-composite-primary-key-in-a-depende
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class CompositeKeyAttribute : Attribute
{
    private string name;
    public int Order;

    public CompositeKeyAttribute(string name)
    {
        this.name = name;
        Order = 1;
    }
}
