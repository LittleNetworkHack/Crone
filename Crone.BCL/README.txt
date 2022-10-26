This project is used for implementing compiler level C# features
Some attributes need to be added for compiler to properly implement newer C# features in older frameworks (targets)
Must be shipped as nuget package since it will carry with itself all versions of dll
Do not target .NET Framework since it has support for .NET Standard 2.0 (NET462+) and you should use that
Each feature is checked by version where is needed and cut off where is implemented

CallerFilePathAttribute
	Applies to: .NET Framework 4.5+, .NET Core 3.0+
	Missing in: .NET Framework 4.0

CallerLineNumberAttribute
	Applies to: .NET Framework 4.5+, .NET Core 3.0+
	Missing in: .NET Framework 4.0

CallerMemberNameAttribute
	Applies to: .NET Framework 4.5+, .NET Core 3.0+
	Missing in: .NET Framework 4.0

CallerArgumentExpressionAttribute
	Applies to: .NET Core 3.0+
	Missing in: .NET Standard 2.0

IsExternalInit
	Applies to: .NET 5.0+
	Missing in: .NET Standard 2.0, .NET Core 3.1

InterpolatedStringHandlerAttribute
	Applies to: .NET 5.0+
	Missing in: .NET Standard 2.0, .NET Core 3.1, .NET 5.0

StringSyntaxAttribute - Currently not working
	Applies to: .NET 7.0+
	Missing in: .NET Standard 2.0, .NET Core 3.1, .NET 5.0, .NET 6.0

CompilerFeatureRequiredAttribute
	Applies to: .NET 7.0+
	Missing in: .NET Standard 2.0, .NET Core 3.1, .NET 5.0, .NET 6.0

RequiredMemberAttribute
	Applies to: .NET 7.0+
	Missing in: .NET Standard 2.0, .NET Core 3.1, .NET 5.0, .NET 6.0

SetsRequiredMembersAttribute
	Applies to: .NET 7.0+
	Missing in: .NET Standard 2.0, .NET Core 3.1, .NET 5.0, .NET 6.0