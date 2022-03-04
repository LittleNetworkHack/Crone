using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Analyzers;

class OracleContextReceiver : ISyntaxContextReceiver
{
	public List<IMethodSymbol> Methods { get; } = new List<IMethodSymbol>();

	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (context.Node is not MethodDeclarationSyntax declaration || declaration.AttributeLists.Count == 0)
			return;

		var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node) as IMethodSymbol;
		if (symbol?.HasAttribute(AttributeGenerator.ProcedureAttribute) != true)
			return;

		Methods.Add(symbol);
	}
}