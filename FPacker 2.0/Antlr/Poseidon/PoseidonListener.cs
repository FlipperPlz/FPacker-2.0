//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.10.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:/Users/dev/Desktop/FPacker 2.0/FPacker 2.0/Antlr/Poseidon\Poseidon.g4 by ANTLR 4.10.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

 namespace FPacker.Antlr.Poseidon; 
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="PoseidonParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.10.1")]
[System.CLSCompliant(false)]
public interface IPoseidonListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.computationalUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComputationalUnit([NotNull] PoseidonParser.ComputationalUnitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.computationalUnit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComputationalUnit([NotNull] PoseidonParser.ComputationalUnitContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.classDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassDefinition([NotNull] PoseidonParser.ClassDefinitionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.classDefinition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassDefinition([NotNull] PoseidonParser.ClassDefinitionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.classBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassBlock([NotNull] PoseidonParser.ClassBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.classBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassBlock([NotNull] PoseidonParser.ClassBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.classExtension"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassExtension([NotNull] PoseidonParser.ClassExtensionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.classExtension"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassExtension([NotNull] PoseidonParser.ClassExtensionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] PoseidonParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] PoseidonParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.variableInitializer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariableInitializer([NotNull] PoseidonParser.VariableInitializerContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.variableInitializer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariableInitializer([NotNull] PoseidonParser.VariableInitializerContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.variableAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariableAssignment([NotNull] PoseidonParser.VariableAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.variableAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariableAssignment([NotNull] PoseidonParser.VariableAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.variableDeclaratorId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVariableDeclaratorId([NotNull] PoseidonParser.VariableDeclaratorIdContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.variableDeclaratorId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVariableDeclaratorId([NotNull] PoseidonParser.VariableDeclaratorIdContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.arrayInitializer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArrayInitializer([NotNull] PoseidonParser.ArrayInitializerContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.arrayInitializer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArrayInitializer([NotNull] PoseidonParser.ArrayInitializerContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteral([NotNull] PoseidonParser.LiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteral([NotNull] PoseidonParser.LiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.literalString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteralString([NotNull] PoseidonParser.LiteralStringContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.literalString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteralString([NotNull] PoseidonParser.LiteralStringContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.literalFloat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteralFloat([NotNull] PoseidonParser.LiteralFloatContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.literalFloat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteralFloat([NotNull] PoseidonParser.LiteralFloatContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.literalInteger"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteralInteger([NotNull] PoseidonParser.LiteralIntegerContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.literalInteger"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteralInteger([NotNull] PoseidonParser.LiteralIntegerContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.deleteStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDeleteStatement([NotNull] PoseidonParser.DeleteStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.deleteStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDeleteStatement([NotNull] PoseidonParser.DeleteStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="PoseidonParser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIdentifier([NotNull] PoseidonParser.IdentifierContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="PoseidonParser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIdentifier([NotNull] PoseidonParser.IdentifierContext context);
}
