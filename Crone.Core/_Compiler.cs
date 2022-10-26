//namespace System.Runtime.CompilerServices
//{
//    #region IsExternalInit

//    internal static class IsExternalInit { }

//    #endregion IsExternalInit

//    #region CallerArgumentExpression

//    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
//    internal sealed class CallerArgumentExpressionAttribute : Attribute
//    {
//        public string ParameterName { get; }

//        public CallerArgumentExpressionAttribute(string parameterName) => ParameterName = parameterName;
//    }

//    #endregion CallerArgumentExpression

//    #region InterpolatedStringHandler

//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
//    internal sealed class InterpolatedStringHandlerAttribute : Attribute { }

//    #endregion InterpolatedStringHandler

//    #region RequiredMember

//    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
//    internal sealed class RequiredMemberAttribute : Attribute { }

//    #endregion RequiredMember

//    #region CompilerFeatureRequired

//    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
//    internal sealed class CompilerFeatureRequiredAttribute : Attribute
//    {
//        public const string RefStructs = nameof(RefStructs);
//        public const string RequiredMembers = nameof(RequiredMembers);

//        public string FeatureName { get; }
//        public bool IsOptional { get; init; }

//        public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;
//    }

//    #endregion CompilerFeatureRequired
//}

//namespace System.Diagnostics.CodeAnalysis
//{
//    #region SetsRequiredMembers

//    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
//    internal sealed class SetsRequiredMembersAttribute : Attribute { }

//    #endregion SetsRequiredMembers
//}