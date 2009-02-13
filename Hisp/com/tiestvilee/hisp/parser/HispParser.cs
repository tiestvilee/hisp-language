// $ANTLR 2.7.7 (20060930): "hISP.g" -> "HispParser.cs"$

    // gets inserted in the C# source file before any
    // generated namespace declarations
    // hence -- can only be using directives

namespace com.tiestvilee.hisp.parser
{
	// Generate the header common to all output files.
	using System;
	
	using TokenBuffer              = antlr.TokenBuffer;
	using TokenStreamException     = antlr.TokenStreamException;
	using TokenStreamIOException   = antlr.TokenStreamIOException;
	using ANTLRException           = antlr.ANTLRException;
	using LLkParser = antlr.LLkParser;
	using Token                    = antlr.Token;
	using IToken                   = antlr.IToken;
	using TokenStream              = antlr.TokenStream;
	using RecognitionException     = antlr.RecognitionException;
	using NoViableAltException     = antlr.NoViableAltException;
	using MismatchedTokenException = antlr.MismatchedTokenException;
	using SemanticException        = antlr.SemanticException;
	using ParserSharedInputState   = antlr.ParserSharedInputState;
	using BitSet                   = antlr.collections.impl.BitSet;
	using AST                      = antlr.collections.AST;
	using ASTPair                  = antlr.ASTPair;
	using ASTFactory               = antlr.ASTFactory;
	using ASTArray                 = antlr.collections.impl.ASTArray;
	
   // global code stuff that will be included in the source file just before the 'MyParser' class below
   //...

	public 	class HispParser : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int LPAREN = 4;
		public const int IDENTIFIER = 5;
		public const int RPAREN = 6;
		public const int CLASS = 7;
		public const int HASH = 8;
		public const int ATTRIBUTE = 9;
		public const int EQUALS = 10;
		public const int STRING = 11;
		public const int WHITESPACE = 12;
		
		
   // additional methods and members for the generated 'MyParser' class
   //...
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
			initializeFactory();
		}
		
		
		protected HispParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public HispParser(TokenBuffer tokenBuf) : this(tokenBuf,1)
		{
		}
		
		protected HispParser(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public HispParser(TokenStream lexer) : this(lexer,1)
		{
		}
		
		public HispParser(ParserSharedInputState state) : base(state,1)
		{
			initialize();
		}
		
	public void expression() //throws RecognitionException, TokenStreamException
{
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		AST expression_AST = null;
		AST s_AST = null;
		IToken  i = null;
		AST i_AST = null;
		IToken  h = null;
		AST h_AST = null;
		IToken  a = null;
		AST a_AST = null;
		IToken  key = null;
		AST key_AST = null;
		AST value_AST = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case LPAREN:
			{
				AST tmp1_AST = null;
				tmp1_AST = astFactory.create(LT(1));
				match(LPAREN);
				AST tmp2_AST = null;
				tmp2_AST = astFactory.create(LT(1));
				match(IDENTIFIER);
				sub_expression();
				s_AST = (AST)returnAST;
				AST tmp3_AST = null;
				tmp3_AST = astFactory.create(LT(1));
				match(RPAREN);
				expression_AST = (AST)currentAST.root;
				expression_AST = (AST) astFactory.make(tmp2_AST, s_AST);
				currentAST.root = expression_AST;
				if ( (null != expression_AST) && (null != expression_AST.getFirstChild()) )
					currentAST.child = expression_AST.getFirstChild();
				else
					currentAST.child = expression_AST;
				currentAST.advanceChildToEnd();
				break;
			}
			case IDENTIFIER:
			{
				AST tmp4_AST = null;
				tmp4_AST = astFactory.create(LT(1));
				astFactory.addASTChild(ref currentAST, tmp4_AST);
				match(IDENTIFIER);
				expression_AST = currentAST.root;
				break;
			}
			case CLASS:
			{
				i = LT(1);
				i_AST = astFactory.create(i);
				match(CLASS);
				AST tmp5_AST = null;
				tmp5_AST = astFactory.create(LT(1));
				match(IDENTIFIER);
				expression_AST = (AST)currentAST.root;
				expression_AST = (AST) astFactory.make(astFactory.create(CLASS,"."), tmp5_AST);
				currentAST.root = expression_AST;
				if ( (null != expression_AST) && (null != expression_AST.getFirstChild()) )
					currentAST.child = expression_AST.getFirstChild();
				else
					currentAST.child = expression_AST;
				currentAST.advanceChildToEnd();
				break;
			}
			case HASH:
			{
				h = LT(1);
				h_AST = astFactory.create(h);
				match(HASH);
				AST tmp6_AST = null;
				tmp6_AST = astFactory.create(LT(1));
				match(IDENTIFIER);
				expression_AST = (AST)currentAST.root;
				expression_AST = (AST) astFactory.make(astFactory.create(HASH,"#"), tmp6_AST);
				currentAST.root = expression_AST;
				if ( (null != expression_AST) && (null != expression_AST.getFirstChild()) )
					currentAST.child = expression_AST.getFirstChild();
				else
					currentAST.child = expression_AST;
				currentAST.advanceChildToEnd();
				break;
			}
			case ATTRIBUTE:
			{
				a = LT(1);
				a_AST = astFactory.create(a);
				match(ATTRIBUTE);
				key = LT(1);
				key_AST = astFactory.create(key);
				match(IDENTIFIER);
				AST tmp7_AST = null;
				tmp7_AST = astFactory.create(LT(1));
				match(EQUALS);
				attribute_value();
				value_AST = (AST)returnAST;
				expression_AST = (AST)currentAST.root;
				expression_AST = (AST) astFactory.make(astFactory.create(ATTRIBUTE,"@"), key_AST, value_AST);
				currentAST.root = expression_AST;
				if ( (null != expression_AST) && (null != expression_AST.getFirstChild()) )
					currentAST.child = expression_AST.getFirstChild();
				else
					currentAST.child = expression_AST;
				currentAST.advanceChildToEnd();
				break;
			}
			case STRING:
			{
				AST tmp8_AST = null;
				tmp8_AST = astFactory.create(LT(1));
				astFactory.addASTChild(ref currentAST, tmp8_AST);
				match(STRING);
				expression_AST = currentAST.root;
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_0_);
		}
		returnAST = expression_AST;
	}
	
	public void sub_expression() //throws RecognitionException, TokenStreamException
{
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		AST sub_expression_AST = null;
		
		try {      // for error handling
			{    // ( ... )*
				for (;;)
				{
					if ((tokenSet_1_.member(LA(1))))
					{
						expression();
						astFactory.addASTChild(ref currentAST, returnAST);
					}
					else
					{
						goto _loop4_breakloop;
					}
					
				}
_loop4_breakloop:				;
			}    // ( ... )*
			sub_expression_AST = currentAST.root;
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_2_);
		}
		returnAST = sub_expression_AST;
	}
	
	public void attribute_value() //throws RecognitionException, TokenStreamException
{
		
		returnAST = null;
		ASTPair currentAST = new ASTPair();
		AST attribute_value_AST = null;
		
		try {      // for error handling
			switch ( LA(1) )
			{
			case IDENTIFIER:
			{
				AST tmp9_AST = null;
				tmp9_AST = astFactory.create(LT(1));
				astFactory.addASTChild(ref currentAST, tmp9_AST);
				match(IDENTIFIER);
				attribute_value_AST = currentAST.root;
				break;
			}
			case STRING:
			{
				AST tmp10_AST = null;
				tmp10_AST = astFactory.create(LT(1));
				astFactory.addASTChild(ref currentAST, tmp10_AST);
				match(STRING);
				attribute_value_AST = currentAST.root;
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		catch (RecognitionException ex)
		{
			reportError(ex);
			recover(ex,tokenSet_0_);
		}
		returnAST = attribute_value_AST;
	}
	
	private void initializeFactory()
	{
		if (astFactory == null)
		{
			astFactory = new ASTFactory();
		}
		initializeASTFactory( astFactory );
	}
	static public void initializeASTFactory( ASTFactory factory )
	{
		factory.setMaxNodeType(12);
	}
	
	public static readonly string[] tokenNames_ = new string[] {
		@"""<0>""",
		@"""EOF""",
		@"""<2>""",
		@"""NULL_TREE_LOOKAHEAD""",
		@"""LPAREN""",
		@"""IDENTIFIER""",
		@"""RPAREN""",
		@"""CLASS""",
		@"""HASH""",
		@"""ATTRIBUTE""",
		@"""EQUALS""",
		@"""STRING""",
		@"""WHITESPACE"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { 3056L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { 2992L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 64L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	
}
}
