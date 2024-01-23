namespace Crone;

public static partial class CoreLib
{
    public static Exception AsDataException(this Exception ex, [CallerMemberName] string name = default)
    {
        return new DataException($"DataException thrown in '{name}', check inner exception.", ex);
    }

	public static Exception AsServiceException(this Exception ex, [CallerMemberName] string name = default)
	{
		return new ServiceException($"ServiceException thrown in '{name}', check inner exception.", ex);
	}
}

public class ServiceException : SystemException
{
	public ServiceException() : base() { }
	public ServiceException(string s) : base(s) { }
	public ServiceException(string s, Exception innerException) : base(s, innerException) { }
	protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
