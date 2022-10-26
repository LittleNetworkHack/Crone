namespace System.Runtime.CompilerServices;
#if NETSTANDARD || NETCOREAPP3_1 || (NET && !NET7_0_OR_GREATER)
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class RequiredMemberAttribute : Attribute { }
#endif