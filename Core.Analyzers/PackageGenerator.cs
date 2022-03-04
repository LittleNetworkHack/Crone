using Microsoft.CodeAnalysis;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Analyzers;
static class PackageGenerator
{
	public static string GeneratePackage(this INamedTypeSymbol package, IEnumerable<IMethodSymbol> procedures)
	{
		using var writer = new StringWriter();
		using var builder = new IndentedTextWriter(writer);



		return writer.GetStringBuilder().ToString();
	}
}
