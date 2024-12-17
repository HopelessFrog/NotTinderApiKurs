using System.Runtime.Serialization;

namespace StartupsApi.Exceptions;

public class MinioException : Exception
{
    public MinioException()
    {
    }

    protected MinioException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public MinioException(string? message) : base(message)
    {
    }

    public MinioException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}