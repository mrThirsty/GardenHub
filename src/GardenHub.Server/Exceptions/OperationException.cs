namespace GardenHub.Server.Exceptions;

public class OperationException : Exception
{
    public OperationException(string message, string operation) : base(message)
    {
        Operation = operation;
    }
    
    public string Operation { get; private set; }
}