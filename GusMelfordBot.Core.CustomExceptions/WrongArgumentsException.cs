namespace GusMelfordBot.Core.Exception;

[Serializable]
public class WrongArgumentsException : System.Exception
{
    public WrongArgumentsException() { }

    public WrongArgumentsException(string message) : base(message) { }

    public WrongArgumentsException(string message, System.Exception inner) : base(message, inner) { }
}