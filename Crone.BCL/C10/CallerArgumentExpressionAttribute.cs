namespace System.Runtime.CompilerServices;
#if NETSTANDARD
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class CallerArgumentExpressionAttribute : Attribute
{
    public string ParameterName { get; }

    public CallerArgumentExpressionAttribute(string parameterName) => ParameterName = parameterName;
}
#endif