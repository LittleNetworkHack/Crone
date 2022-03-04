using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Analyzers;

static class AttributeGenerator
{
	public static readonly string PackageAttribute = "OraclePackageAttribute";
	public static readonly string ProcedureAttribute = "OracleProcedureAttribute";
	public static readonly string AttributeSource =
@$"using System;
#nullable disable

[AttributeUsage(AttributeTargets.Method)]
internal sealed class {PackageAttribute} : Attribute
{{
	public string Schema {{ get; set; }}
	public string Package {{ get; set; }}
}}

[AttributeUsage(AttributeTargets.Class)]
internal sealed class {ProcedureAttribute} : Attribute
{{
	public string Procedure {{ get; set; }}
}}";

	public static void RegisterAttributes(GeneratorPostInitializationContext context)
	{
		context.AddSource("OracleAttributes.cs", AttributeSource);
	}
}
