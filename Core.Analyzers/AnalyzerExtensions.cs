using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Analyzers;

static class AnalyzerExtensions
{
	public static bool HasAttribute(this ISymbol symbol, string name)
	{
		return symbol.GetAttributes().Any(e => e.AttributeClass?.ToDisplayString() == name);
	}
}
