using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Diagnostics;
using System.Text;

namespace Core.Analyzers
{
	[Generator]
	public class OracleGenerator : ISourceGenerator
	{
		private static readonly DiagnosticDescriptor ORG0001 = new DiagnosticDescriptor
		(
			nameof(ORG0001),
			"Missing internal generator attribute", 
			"Internal analyter error",
			"Internal",
			DiagnosticSeverity.Error,
			true
		);

		public void Initialize(GeneratorInitializationContext context)
		{
			//if (!Debugger.IsAttached && false)
			//	Debugger.Launch();

			context.RegisterForPostInitialization(AttributeGenerator.RegisterAttributes);
			context.RegisterForSyntaxNotifications(() => new OracleContextReceiver());
		}

		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxContextReceiver is not OracleContextReceiver receiver)
				return;


		}
	}
}