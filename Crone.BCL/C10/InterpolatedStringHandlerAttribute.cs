namespace System.Runtime.CompilerServices;
#if NETSTANDARD || NETCOREAPP3_1 || (NET && !NET6_0_OR_GREATER)
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class InterpolatedStringHandlerAttribute : Attribute { }
#endif