namespace System.Diagnostics.CodeAnalysis;
#if NETSTANDARD || NETCOREAPP3_1 || (NET && !NET7_0_OR_GREATER)
[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class SetsRequiredMembersAttribute : Attribute { }
#endif