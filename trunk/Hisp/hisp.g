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

{
   // global code stuff that will be included in the source file just before the 'MyParser' class below
   //...
}
class HispParser extends Parser;
options {
   exportVocab=Hisp;
   buildAST = true;
}
{
   // additional methods and members for the generated 'MyParser' class
   //...
}

//... generated RULES go here ...

expression
  :! LPAREN IDENTIFIER s:sub_expression RPAREN
    {#expression = #(#IDENTIFIER, s);}
  | IDENTIFIER
  |! i:CLASS IDENTIFIER
    {#expression = #(#[CLASS, "."], #IDENTIFIER);}
  |! h:HASH IDENTIFIER
    {#expression = #(#[HASH, "#"], #IDENTIFIER);}
  |! a:ATTRIBUTE key:IDENTIFIER EQUALS value:attribute_value
    {#expression = #(#[ATTRIBUTE, "@"], key, value);}
  | STRING
  ;
  
sub_expression
  : (expression)*
  ;
  
attribute_value
  : IDENTIFIER
  | STRING
  ;


class HispLexer extends Lexer;

// one-or-more letters followed by a newline
IDENTIFIER:   ( 'a'..'z'|'A'..'Z'|'0'..'9'|'-'|'_' )+;
STRING: '"' (~('"'))* '"';

LPAREN: '<';
RPAREN: '>';
CLASS: '.';
HASH: '#';
ATTRIBUTE: '@';
EQUALS: '=';

WHITESPACE: (' ' | '\r' | '\t' | '\u000C' | '\n') { _ttype = Token.SKIP; };












/*

startRule
    :   n:NAME
        {System.out.println("Hi there, "+n.getText());}
    ;

class L extends Lexer;

// one-or-more letters followed by a newline
NAME:   ( 'a'..'z'|'A'..'Z' )+ NEWLINE
    ;

NEWLINE
    :   '\r' '\n'   // DOS
    |   '\n'        // UNIX
    ;

	
*/
	
	
	
	
	
	
	
	
	
	
	
	
	

/*


{
   // global code stuff that will be included in the source file just before the 'MyLexer' class below
   //...
}


class MyLexer extends Lexer;
options {
   exportVocab=My;
}
{
   // additional methods and members for the generated 'MyParser' class
   //...
}


//... generated RULES go here ...

{
   // global code stuff that will be included in the source file just before the 'MyTreeParser' class below
   //...
}
class MyTreeParser extends TreeParser;
options {
   exportVocab=My;
}
{
   // additional methods and members for the generated 'MyParser' class
   //...
}

*/

//... generated RULES go here ...
