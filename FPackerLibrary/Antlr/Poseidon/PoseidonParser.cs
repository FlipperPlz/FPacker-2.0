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

 namespace FPackerLibrary.Antlr.Poseidon; 
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.10.1")]
[System.CLSCompliant(false)]
public partial class PoseidonParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, SINGLE_LINE_COMMENT=11, EMPTY_DELIMITED_COMMENT=12, DELIMITED_COMMENT=13, 
		WHITESPACES=14, PREPROCESS=15, LITERAL_STRING=16, LITERAL_INTEGER=17, 
		LITERAL_FLOAT=18, IDENTIFIER=19, SCIENTIFIC=20;
	public const int
		RULE_computationalUnit = 0, RULE_classDefinition = 1, RULE_classBlock = 2, 
		RULE_classExtension = 3, RULE_statement = 4, RULE_variableInitializer = 5, 
		RULE_variableAssignment = 6, RULE_variableDeclaratorId = 7, RULE_arrayInitializer = 8, 
		RULE_literal = 9, RULE_literalString = 10, RULE_literalFloat = 11, RULE_literalInteger = 12, 
		RULE_deleteStatement = 13, RULE_identifier = 14;
	public static readonly string[] ruleNames = {
		"computationalUnit", "classDefinition", "classBlock", "classExtension", 
		"statement", "variableInitializer", "variableAssignment", "variableDeclaratorId", 
		"arrayInitializer", "literal", "literalString", "literalFloat", "literalInteger", 
		"deleteStatement", "identifier"
	};

	private static readonly string[] _LiteralNames = {
		null, "'class'", "';'", "'{'", "'}'", "':'", "'='", "'['", "']'", "','", 
		"'delete'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, "SINGLE_LINE_COMMENT", 
		"EMPTY_DELIMITED_COMMENT", "DELIMITED_COMMENT", "WHITESPACES", "PREPROCESS", 
		"LITERAL_STRING", "LITERAL_INTEGER", "LITERAL_FLOAT", "IDENTIFIER", "SCIENTIFIC"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "Poseidon.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static PoseidonParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public PoseidonParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public PoseidonParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class ComputationalUnitContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ClassDefinitionContext[] classDefinition() {
			return GetRuleContexts<ClassDefinitionContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public ClassDefinitionContext classDefinition(int i) {
			return GetRuleContext<ClassDefinitionContext>(i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public StatementContext[] statement() {
			return GetRuleContexts<StatementContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public StatementContext statement(int i) {
			return GetRuleContext<StatementContext>(i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Eof() { return GetToken(PoseidonParser.Eof, 0); }
		public ComputationalUnitContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_computationalUnit; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterComputationalUnit(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitComputationalUnit(this);
		}
	}

	[RuleVersion(0)]
	public ComputationalUnitContext computationalUnit() {
		ComputationalUnitContext _localctx = new ComputationalUnitContext(Context, State);
		EnterRule(_localctx, 0, RULE_computationalUnit);
		int _la;
		try {
			State = 38;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,2,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 34;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__0) | (1L << T__9) | (1L << IDENTIFIER))) != 0)) {
					{
					State = 32;
					ErrorHandler.Sync(this);
					switch (TokenStream.LA(1)) {
					case T__0:
						{
						State = 30;
						classDefinition();
						}
						break;
					case T__9:
					case IDENTIFIER:
						{
						State = 31;
						statement();
						}
						break;
					default:
						throw new NoViableAltException(this);
					}
					}
					State = 36;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 37;
				Match(Eof);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ClassDefinitionContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public IdentifierContext identifier() {
			return GetRuleContext<IdentifierContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ClassExtensionContext classExtension() {
			return GetRuleContext<ClassExtensionContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ClassBlockContext classBlock() {
			return GetRuleContext<ClassBlockContext>(0);
		}
		public ClassDefinitionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_classDefinition; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterClassDefinition(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitClassDefinition(this);
		}
	}

	[RuleVersion(0)]
	public ClassDefinitionContext classDefinition() {
		ClassDefinitionContext _localctx = new ClassDefinitionContext(Context, State);
		EnterRule(_localctx, 2, RULE_classDefinition);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 40;
			Match(T__0);
			State = 41;
			identifier();
			State = 43;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==T__4) {
				{
				State = 42;
				classExtension();
				}
			}

			State = 46;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==T__2) {
				{
				State = 45;
				classBlock();
				}
			}

			State = 48;
			Match(T__1);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ClassBlockContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ClassDefinitionContext[] classDefinition() {
			return GetRuleContexts<ClassDefinitionContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public ClassDefinitionContext classDefinition(int i) {
			return GetRuleContext<ClassDefinitionContext>(i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public StatementContext[] statement() {
			return GetRuleContexts<StatementContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public StatementContext statement(int i) {
			return GetRuleContext<StatementContext>(i);
		}
		public ClassBlockContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_classBlock; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterClassBlock(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitClassBlock(this);
		}
	}

	[RuleVersion(0)]
	public ClassBlockContext classBlock() {
		ClassBlockContext _localctx = new ClassBlockContext(Context, State);
		EnterRule(_localctx, 4, RULE_classBlock);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 50;
			Match(T__2);
			State = 55;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__0) | (1L << T__9) | (1L << IDENTIFIER))) != 0)) {
				{
				State = 53;
				ErrorHandler.Sync(this);
				switch (TokenStream.LA(1)) {
				case T__0:
					{
					State = 51;
					classDefinition();
					}
					break;
				case T__9:
				case IDENTIFIER:
					{
					State = 52;
					statement();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				State = 57;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			State = 58;
			Match(T__3);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ClassExtensionContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public IdentifierContext identifier() {
			return GetRuleContext<IdentifierContext>(0);
		}
		public ClassExtensionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_classExtension; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterClassExtension(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitClassExtension(this);
		}
	}

	[RuleVersion(0)]
	public ClassExtensionContext classExtension() {
		ClassExtensionContext _localctx = new ClassExtensionContext(Context, State);
		EnterRule(_localctx, 6, RULE_classExtension);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 60;
			Match(T__4);
			State = 61;
			identifier();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class StatementContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public VariableAssignmentContext variableAssignment() {
			return GetRuleContext<VariableAssignmentContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public DeleteStatementContext deleteStatement() {
			return GetRuleContext<DeleteStatementContext>(0);
		}
		public StatementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_statement; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterStatement(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitStatement(this);
		}
	}

	[RuleVersion(0)]
	public StatementContext statement() {
		StatementContext _localctx = new StatementContext(Context, State);
		EnterRule(_localctx, 8, RULE_statement);
		try {
			State = 65;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case IDENTIFIER:
				EnterOuterAlt(_localctx, 1);
				{
				State = 63;
				variableAssignment();
				}
				break;
			case T__9:
				EnterOuterAlt(_localctx, 2);
				{
				State = 64;
				deleteStatement();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class VariableInitializerContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ArrayInitializerContext arrayInitializer() {
			return GetRuleContext<ArrayInitializerContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public LiteralContext literal() {
			return GetRuleContext<LiteralContext>(0);
		}
		public VariableInitializerContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_variableInitializer; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterVariableInitializer(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitVariableInitializer(this);
		}
	}

	[RuleVersion(0)]
	public VariableInitializerContext variableInitializer() {
		VariableInitializerContext _localctx = new VariableInitializerContext(Context, State);
		EnterRule(_localctx, 10, RULE_variableInitializer);
		try {
			State = 69;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case T__2:
				EnterOuterAlt(_localctx, 1);
				{
				State = 67;
				arrayInitializer();
				}
				break;
			case LITERAL_STRING:
			case LITERAL_INTEGER:
			case LITERAL_FLOAT:
				EnterOuterAlt(_localctx, 2);
				{
				State = 68;
				literal();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class VariableAssignmentContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public VariableDeclaratorIdContext variableDeclaratorId() {
			return GetRuleContext<VariableDeclaratorIdContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public VariableInitializerContext variableInitializer() {
			return GetRuleContext<VariableInitializerContext>(0);
		}
		public VariableAssignmentContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_variableAssignment; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterVariableAssignment(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitVariableAssignment(this);
		}
	}

	[RuleVersion(0)]
	public VariableAssignmentContext variableAssignment() {
		VariableAssignmentContext _localctx = new VariableAssignmentContext(Context, State);
		EnterRule(_localctx, 12, RULE_variableAssignment);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 71;
			variableDeclaratorId();
			State = 72;
			Match(T__5);
			State = 73;
			variableInitializer();
			State = 74;
			Match(T__1);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class VariableDeclaratorIdContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public IdentifierContext identifier() {
			return GetRuleContext<IdentifierContext>(0);
		}
		public VariableDeclaratorIdContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_variableDeclaratorId; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterVariableDeclaratorId(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitVariableDeclaratorId(this);
		}
	}

	[RuleVersion(0)]
	public VariableDeclaratorIdContext variableDeclaratorId() {
		VariableDeclaratorIdContext _localctx = new VariableDeclaratorIdContext(Context, State);
		EnterRule(_localctx, 14, RULE_variableDeclaratorId);
		int _la;
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 76;
			identifier();
			State = 87;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while (_la==T__6) {
				{
				{
				State = 77;
				Match(T__6);
				State = 81;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,9,Context);
				while ( _alt!=1 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1+1 ) {
						{
						{
						State = 78;
						MatchWildcard();
						}
						} 
					}
					State = 83;
					ErrorHandler.Sync(this);
					_alt = Interpreter.AdaptivePredict(TokenStream,9,Context);
				}
				State = 84;
				Match(T__7);
				}
				}
				State = 89;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ArrayInitializerContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public VariableInitializerContext[] variableInitializer() {
			return GetRuleContexts<VariableInitializerContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public VariableInitializerContext variableInitializer(int i) {
			return GetRuleContext<VariableInitializerContext>(i);
		}
		public ArrayInitializerContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_arrayInitializer; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterArrayInitializer(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitArrayInitializer(this);
		}
	}

	[RuleVersion(0)]
	public ArrayInitializerContext arrayInitializer() {
		ArrayInitializerContext _localctx = new ArrayInitializerContext(Context, State);
		EnterRule(_localctx, 16, RULE_arrayInitializer);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 90;
			Match(T__2);
			State = 99;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__2) | (1L << LITERAL_STRING) | (1L << LITERAL_INTEGER) | (1L << LITERAL_FLOAT))) != 0)) {
				{
				State = 91;
				variableInitializer();
				State = 96;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==T__8) {
					{
					{
					State = 92;
					Match(T__8);
					State = 93;
					variableInitializer();
					}
					}
					State = 98;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				}
			}

			State = 101;
			Match(T__3);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class LiteralContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public LiteralIntegerContext literalInteger() {
			return GetRuleContext<LiteralIntegerContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public LiteralFloatContext literalFloat() {
			return GetRuleContext<LiteralFloatContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public LiteralStringContext literalString() {
			return GetRuleContext<LiteralStringContext>(0);
		}
		public LiteralContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_literal; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterLiteral(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitLiteral(this);
		}
	}

	[RuleVersion(0)]
	public LiteralContext literal() {
		LiteralContext _localctx = new LiteralContext(Context, State);
		EnterRule(_localctx, 18, RULE_literal);
		try {
			State = 106;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case LITERAL_INTEGER:
				EnterOuterAlt(_localctx, 1);
				{
				State = 103;
				literalInteger();
				}
				break;
			case LITERAL_FLOAT:
				EnterOuterAlt(_localctx, 2);
				{
				State = 104;
				literalFloat();
				}
				break;
			case LITERAL_STRING:
				EnterOuterAlt(_localctx, 3);
				{
				State = 105;
				literalString();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class LiteralStringContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode LITERAL_STRING() { return GetToken(PoseidonParser.LITERAL_STRING, 0); }
		public LiteralStringContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_literalString; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterLiteralString(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitLiteralString(this);
		}
	}

	[RuleVersion(0)]
	public LiteralStringContext literalString() {
		LiteralStringContext _localctx = new LiteralStringContext(Context, State);
		EnterRule(_localctx, 20, RULE_literalString);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 108;
			Match(LITERAL_STRING);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class LiteralFloatContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode LITERAL_FLOAT() { return GetToken(PoseidonParser.LITERAL_FLOAT, 0); }
		public LiteralFloatContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_literalFloat; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterLiteralFloat(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitLiteralFloat(this);
		}
	}

	[RuleVersion(0)]
	public LiteralFloatContext literalFloat() {
		LiteralFloatContext _localctx = new LiteralFloatContext(Context, State);
		EnterRule(_localctx, 22, RULE_literalFloat);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 110;
			Match(LITERAL_FLOAT);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class LiteralIntegerContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode LITERAL_INTEGER() { return GetToken(PoseidonParser.LITERAL_INTEGER, 0); }
		public LiteralIntegerContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_literalInteger; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterLiteralInteger(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitLiteralInteger(this);
		}
	}

	[RuleVersion(0)]
	public LiteralIntegerContext literalInteger() {
		LiteralIntegerContext _localctx = new LiteralIntegerContext(Context, State);
		EnterRule(_localctx, 24, RULE_literalInteger);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 112;
			Match(LITERAL_INTEGER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class DeleteStatementContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public IdentifierContext identifier() {
			return GetRuleContext<IdentifierContext>(0);
		}
		public DeleteStatementContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_deleteStatement; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterDeleteStatement(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitDeleteStatement(this);
		}
	}

	[RuleVersion(0)]
	public DeleteStatementContext deleteStatement() {
		DeleteStatementContext _localctx = new DeleteStatementContext(Context, State);
		EnterRule(_localctx, 26, RULE_deleteStatement);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 114;
			Match(T__9);
			State = 115;
			identifier();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class IdentifierContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode IDENTIFIER() { return GetToken(PoseidonParser.IDENTIFIER, 0); }
		public IdentifierContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_identifier; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.EnterIdentifier(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IPoseidonListener typedListener = listener as IPoseidonListener;
			if (typedListener != null) typedListener.ExitIdentifier(this);
		}
	}

	[RuleVersion(0)]
	public IdentifierContext identifier() {
		IdentifierContext _localctx = new IdentifierContext(Context, State);
		EnterRule(_localctx, 28, RULE_identifier);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 117;
			Match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static int[] _serializedATN = {
		4,1,20,120,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,6,2,7,
		7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,7,14,
		1,0,1,0,5,0,33,8,0,10,0,12,0,36,9,0,1,0,3,0,39,8,0,1,1,1,1,1,1,3,1,44,
		8,1,1,1,3,1,47,8,1,1,1,1,1,1,2,1,2,1,2,5,2,54,8,2,10,2,12,2,57,9,2,1,2,
		1,2,1,3,1,3,1,3,1,4,1,4,3,4,66,8,4,1,5,1,5,3,5,70,8,5,1,6,1,6,1,6,1,6,
		1,6,1,7,1,7,1,7,5,7,80,8,7,10,7,12,7,83,9,7,1,7,5,7,86,8,7,10,7,12,7,89,
		9,7,1,8,1,8,1,8,1,8,5,8,95,8,8,10,8,12,8,98,9,8,3,8,100,8,8,1,8,1,8,1,
		9,1,9,1,9,3,9,107,8,9,1,10,1,10,1,11,1,11,1,12,1,12,1,13,1,13,1,13,1,14,
		1,14,1,14,1,81,0,15,0,2,4,6,8,10,12,14,16,18,20,22,24,26,28,0,0,119,0,
		38,1,0,0,0,2,40,1,0,0,0,4,50,1,0,0,0,6,60,1,0,0,0,8,65,1,0,0,0,10,69,1,
		0,0,0,12,71,1,0,0,0,14,76,1,0,0,0,16,90,1,0,0,0,18,106,1,0,0,0,20,108,
		1,0,0,0,22,110,1,0,0,0,24,112,1,0,0,0,26,114,1,0,0,0,28,117,1,0,0,0,30,
		33,3,2,1,0,31,33,3,8,4,0,32,30,1,0,0,0,32,31,1,0,0,0,33,36,1,0,0,0,34,
		32,1,0,0,0,34,35,1,0,0,0,35,39,1,0,0,0,36,34,1,0,0,0,37,39,5,0,0,1,38,
		34,1,0,0,0,38,37,1,0,0,0,39,1,1,0,0,0,40,41,5,1,0,0,41,43,3,28,14,0,42,
		44,3,6,3,0,43,42,1,0,0,0,43,44,1,0,0,0,44,46,1,0,0,0,45,47,3,4,2,0,46,
		45,1,0,0,0,46,47,1,0,0,0,47,48,1,0,0,0,48,49,5,2,0,0,49,3,1,0,0,0,50,55,
		5,3,0,0,51,54,3,2,1,0,52,54,3,8,4,0,53,51,1,0,0,0,53,52,1,0,0,0,54,57,
		1,0,0,0,55,53,1,0,0,0,55,56,1,0,0,0,56,58,1,0,0,0,57,55,1,0,0,0,58,59,
		5,4,0,0,59,5,1,0,0,0,60,61,5,5,0,0,61,62,3,28,14,0,62,7,1,0,0,0,63,66,
		3,12,6,0,64,66,3,26,13,0,65,63,1,0,0,0,65,64,1,0,0,0,66,9,1,0,0,0,67,70,
		3,16,8,0,68,70,3,18,9,0,69,67,1,0,0,0,69,68,1,0,0,0,70,11,1,0,0,0,71,72,
		3,14,7,0,72,73,5,6,0,0,73,74,3,10,5,0,74,75,5,2,0,0,75,13,1,0,0,0,76,87,
		3,28,14,0,77,81,5,7,0,0,78,80,9,0,0,0,79,78,1,0,0,0,80,83,1,0,0,0,81,82,
		1,0,0,0,81,79,1,0,0,0,82,84,1,0,0,0,83,81,1,0,0,0,84,86,5,8,0,0,85,77,
		1,0,0,0,86,89,1,0,0,0,87,85,1,0,0,0,87,88,1,0,0,0,88,15,1,0,0,0,89,87,
		1,0,0,0,90,99,5,3,0,0,91,96,3,10,5,0,92,93,5,9,0,0,93,95,3,10,5,0,94,92,
		1,0,0,0,95,98,1,0,0,0,96,94,1,0,0,0,96,97,1,0,0,0,97,100,1,0,0,0,98,96,
		1,0,0,0,99,91,1,0,0,0,99,100,1,0,0,0,100,101,1,0,0,0,101,102,5,4,0,0,102,
		17,1,0,0,0,103,107,3,24,12,0,104,107,3,22,11,0,105,107,3,20,10,0,106,103,
		1,0,0,0,106,104,1,0,0,0,106,105,1,0,0,0,107,19,1,0,0,0,108,109,5,16,0,
		0,109,21,1,0,0,0,110,111,5,18,0,0,111,23,1,0,0,0,112,113,5,17,0,0,113,
		25,1,0,0,0,114,115,5,10,0,0,115,116,3,28,14,0,116,27,1,0,0,0,117,118,5,
		19,0,0,118,29,1,0,0,0,14,32,34,38,43,46,53,55,65,69,81,87,96,99,106
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
