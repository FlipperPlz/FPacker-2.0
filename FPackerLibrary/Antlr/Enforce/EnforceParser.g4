parser grammar EnforceParser;

@header { namespace FPackerLibrary.Antlr.Enforce; }

options { tokenVocab=EnforceLexer; }

computationalUnit: (globalDeclaration | typeDeclaration)* EOF;

variableModifier:
    PRIVATE |
    PROTECTED |
    STATIC |
    AUTOPTR |
    PROTO |
    REF |
    REFERENCE |
    CONST |
    TYPEDEF |
    OWNED;

methodModifier:
    PRIVATE |
    PROTECTED |
    STATIC |
    OVERRIDE |
    PROTO |
    NATIVE |
    REF |
    VOLATILE |
    EXTERNAL |
    EVENT |
    OWNED;

parameterModifier:
    OWNED |
    OUT |
    INOUT |
    NOTNULL |
    REF |
    PRIVATE |
    PROTECTED |
    STATIC |
    AUTOPTR |
    PROTO |
    CONST |
    TYPEDEF |
    LOCAL;

globalDeclaration: (fieldDeclaration | methodDeclaration);

classDeclaration: annotation? CLASS identifier typeParameters? (classOrEnumExtension)? classBody;

classOrEnumExtension: ('extends'|':') typeType;

typeParameters: '<' typeParameter (',' typeParameter)* '>';
typeParameter: typeTypeOrVoid identifier;

enumDeclaration: annotation? ENUM identifier (classOrEnumExtension)? enumBody ;
enumBody: '{' (enumValue ((','|';') enumValue)* (','|';')?)? '}';
enumValue: identifier ('=' expression)?;

annotation: '[' expression ']';

classBody: '{' (';' | globalDeclaration)* '}';

typeDeclaration: MODDED? (classDeclaration | enumDeclaration) ';'* | ';';

methodDeclaration: annotation? methodModifier* typeTypeOrVoid '~'? identifier formalParameters methodBody? ';'*;

methodBody: block;

typeTypeOrVoid: typeType | VOID;

fieldDeclaration: annotation? variableModifier* typeType variableDeclarators ';'*;

variableDeclarators: variableDeclarator (',' variableDeclarator)*;
variableDeclarator: variableDeclaratorId ('=' variableInitializer)?;
variableDeclaratorId: identifier ('[' variableInitializer? ']')*;


formalParameters: '(' formalParameterList? ')';
formalParameterList: (formalParameter) (',' formalParameter)*;

formalParameter: (formalParameterDefined|formalParameterUndefined);

formalParameterUndefined: parameterModifier* typeTypeOrVoid variableDeclaratorId;
formalParameterDefined: parameterModifier* typeTypeOrVoid variableDeclarator;

variableInitializer : arrayInitializer | expression;

arrayInitializer: '{' (variableInitializer (',' variableInitializer)* )? ','* '}';

literal: literalNumeric | literalVector | literalBoolean | literalString | literalNull;
literalBoolean: LITERAL_BOOLEAN;
literalString: LITERAL_STRING;
literalNull: LITERAL_NULL;


literalNumeric: literalInteger | literalFloat;

literalFloat: LITERAL_FLOAT;
literalInteger: LITERAL_INTEGER;
literalVector:
    '(' literalFloat ',' literalFloat ',' literalFloat ')' |
    '"' literalFloat SPACE literalFloat SPACE literalFloat '"';
classType: identifier typeArguments? ('.' identifier ( typeArguments)?)* ;

//STATEMENTS & BLOCKS

block: '{' blockStatement* '}';

blockStatement:
    localVariableDeclaration ';' |
    statement ;

statement:
    deleteStatment |
    ifStatement |
    forStatement |
    foreachStatement |
    whileStatement |
    switchStatement |
    blockLabel=block |
    RETURN expression? ';'* |
    BREAK ';'* |
    CONTINUE ';'* |
    statementExpression=expression ';'*|
    identifierLabel=identifier ':' statement |
    ';';

deleteStatment: DELETE expression ';'*;

//0.o
ifStatement: IF parExpression (blockStatement|block) (elseStatement)? ';'*;
    elseStatement: ELSE (blockStatement|block);
forStatement: FOR '(' forControl ')' (blockStatement|block);
foreachStatement: FOREACH '(' foreachControl ')' (blockStatement|block);
whileStatement: WHILE parExpression (blockStatement|block);
switchStatement: SWITCH parExpression '{' switchBlockStatementGroup* switchLabel* '}';


switchBlockStatementGroup: switchLabel+ blockStatement+;

switchLabel:
    CASE (constantExpression=expression | enumConstantName=IDENTIFIER | typeType varName=identifier) ':' |
    DEFAULT ':';

forControl: forInit? ';' expression? ';' forUpdate=expressionList? ';'*;
forInit: localVariableDeclaration | expressionList;

foreachControl: forEachVariableList ':' expression;
forEachVariableList: forEachVariable (',' forEachVariable)*;
forEachVariable: ((typeType | AUTO) variableDeclaratorId);

localVariableDeclaration: variableModifier* (localVariableDeclarationRegular | localVariableDeclarationAssumptuative);
    localVariableDeclarationRegular: typeType variableDeclarators;
    localVariableDeclarationAssumptuative: AUTO identifier '=' expression;

identifier: IDENTIFIER | TYPE_BOOL | TYPE_INT | TYPE_FLOAT | TYPE_VECTOR | TYPE_STRING;

//EXPRESSIONS

parExpression: '(' expression ')';
expressionList: expression (',' expression)*;

methodCall:
    '!'? identifier '(' expressionList? ')' |
    THIS '.' '!'? identifier '(' expressionList? ')' |
    SUPER '.' '!'? identifier '(' expressionList? ')';

expression:
    primary |
    expression op='.'
      (
       identifier |
       methodCall |
       THIS |
       NEW variableModifier* nonWildcardTypeArguments? innerCreator |
       superSuffix |
       explicitGenericInvocation
      ) |
    expression '[' expression ']' |
    methodCall |
    NEW variableModifier* creator |
    '(' typeType ('&' typeType)* ')' expression |
    expression suffix=('++'|'--') |
    prefix=('+'|'-'|'++'|'--') expression |
    prefix=('~'|'!') expression |
    expression (IDENTIFIER|parExpression) expression |
    expression '^' expression |
    expression op=('*'|'/'|'%') expression |
    expression op=('++'|'--'|'+'|'-') expression |
    expression op=('<<' | '>>') expression |
    expression op=('<=' | '>=' | '>' | '<') expression |
    expression op=('==' | '!=') expression |
    expression op='&' expression |
    expression op='~' expression |
    expression op='|' expression |
    expression op='&&' expression |
    expression op='||' expression |
    <assoc=right> expression op=('=' | '+=' | '-=' | '*=' | '/=' | '|=' | '&=' | '<<=' | '>>=') expression;

primary:
    '(' expression ')' |
    THIS |
    arrayInitializer |
    literal |
    identifier typeArguments |
    (identifier | primitiveType) |
    nonWildcardTypeArguments (explicitGenericInvocationSuffix);

creator : nonWildcardTypeArguments createdName classCreatorRest
    | createdName (arrayCreatorRest | classCreatorRest)
    ;

createdName: identifier typeArgumentsOrDiamond? ('.' identifier typeArgumentsOrDiamond?)* |
    primitiveType;

innerCreator: identifier nonWildcardTypeArgumentsOrDiamond? classCreatorRest;

arrayCreatorRest: '[' (']' ('[' ']')* arrayInitializer | expression ']' ('[' expression ']')* ('[' ']')*)
    ;

classCreatorRest : arguments? /*classBody?*/;

explicitGenericInvocation: nonWildcardTypeArguments? explicitGenericInvocationSuffix;

typeArgumentsOrDiamond: '<' '>' |typeArguments;

nonWildcardTypeArgumentsOrDiamond: '<' '>' | nonWildcardTypeArguments;

nonWildcardTypeArguments: '<' typeList '>';

typeList: typeListChild (',' typeListChild)*;
typeListChild: typeType;
typeType: (classType | primitiveType) ('[' ']')*;

primitiveType: TYPE_INT |
               TYPE_FLOAT |
               TYPE_BOOL |
               TYPE_STRING |
               TYPE_VECTOR ;

typeArguments: LT (typeArgument (',' typeArgument)*) ('>' | '>>' )* ;
typeArgument: variableModifier* (typeType |identifier);

superSuffix: '.' methodCall;

explicitGenericInvocationSuffix: /*SUPER '.' identifier*/ |
                   identifier arguments;

arguments: '(' expressionList? ')';