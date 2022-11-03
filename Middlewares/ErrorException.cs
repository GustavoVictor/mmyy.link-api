using System.Collections;
using System.Runtime.Serialization;

public class ErrorException : Exception
{
    public override string Message => base.Message;

    public ErrorCode ErrorCode;

    public ErrorException(string message) : base(message)
    {   
    }

    public ErrorException(ErrorCode errorCode) : base(errorCode.Message)
    {   
        ErrorCode = errorCode;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override Exception GetBaseException()
    {
        return base.GetBaseException();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}