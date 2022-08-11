grammar Poseidon;

@header { namespace FPackerLibrary.Antlr.Poseidon; }

computationalUnit: (classDefinition | statement)* | EOF;

classDefinition: 'class' identifier classExtension? classBlock? ';';

classBlock: '{'
    (classDefinition | statement)*
'}';

classExtension: ':' identifier;

statement:
    variableAssignment |
    deleteStatement;


variableAssignment: variableDeclaratorId '=' variableInitializer ';';
variableInitializer : arrayInitializer | literal;
variableDeclaratorId: identifier ('[' .*? ']')*;
arrayInitializer: '{' (variableInitializer (',' variableInitializer)* )? '}';

literal: literalInteger | literalFloat | literalString;
literalString: LITERAL_STRING;
literalFloat: LITERAL_FLOAT;
literalInteger: LITERAL_INTEGER;

deleteStatement: 'delete' identifier;

identifier: IDENTIFIER;

SINGLE_LINE_COMMENT: '//' ~[\r\n]*           -> skip;
EMPTY_DELIMITED_COMMENT: ('/*/' | '/**/')    -> skip;
DELIMITED_COMMENT: '/*' .*? '*/'             -> skip;
WHITESPACES: [\r\n \t]                       -> channel(HIDDEN);
PREPROCESS: '#' ~[\r\n]*                      -> channel(HIDDEN);

LITERAL_STRING: '"' (EnforceEscapeSequence | .)*? '"';
LITERAL_INTEGER: '-'? Digit+;
LITERAL_FLOAT: FloatingPoint | FloatingPointScientific | LITERAL_INTEGER ;
IDENTIFIER: [a-zA-Z0-9_]+;

fragment EnforceEscapeSequence:
    '\\\\' |
    '\\"' |
    '\\\'';

fragment Digit: [0-9];

fragment FloatingPoint: '-'? Digit* '.' Digit*;
fragment FloatingPointScientific: FloatingPoint SCIENTIFIC Digit*;


SCIENTIFIC : ('E'|'e') [+-];