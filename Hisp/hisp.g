header 
{
    // gets inserted in the C# source file before any
    // generated namespace declarations
    // hence -- can only be using directives
}

options {
    language  = "CSharp";
    namespace = "com.tiestvilee.hisp.parser";          // encapsulate code in this namespace
    // classHeaderPrefix = "protected"; // use to specify access level for generated class
}


class HispLexer extends Lexer;

// one-or-more letters followed by a newline
UNQUOTED_STRING:   ( 'a'..'z'|'A'..'Z'|'0'..'9'|'-'|'_' )+;
STRING: '"' (~('"'))* '"';

LPAREN: '<';
RPAREN: '>';
CLASS: '.';
HASH: '#';
ATTRIBUTE: '@';
EQUALS: '=';
VARIABLE: '$';

NEWLINE: ( '\r' | '\n' | '\u000C' );
WHITESPACE: (' ')+;