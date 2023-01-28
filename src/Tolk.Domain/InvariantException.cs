namespace Tolk.Domain;

public class InvariantException : Exception
{
    public InvariantException(string message) : base(message)
    {
    }
}
