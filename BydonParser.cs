/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 3.4.0.0
 */
using System.Collections.Generic;
using Hime.Redist;
using Hime.Redist.Parsers;

namespace Bydon
{
	/// <summary>
	/// Represents a parser
	/// </summary>
	internal class BydonParser : LRkParser
	{
		/// <summary>
		/// The automaton for this parser
		/// </summary>
		private static readonly LRkAutomaton commonAutomaton = LRkAutomaton.Find(typeof(BydonParser), "BydonParser.bin");
		/// <summary>
		/// Contains the constant IDs for the variables and virtuals in this parser
		/// </summary>
		public class ID
		{
			/// <summary>
			/// The unique identifier for variable function_call
			/// </summary>
			public const int VariableFunctionCall = 0x0015;
			/// <summary>
			/// The unique identifier for variable exp_atom
			/// </summary>
			public const int VariableExpAtom = 0x0016;
			/// <summary>
			/// The unique identifier for variable exp_resatom
			/// </summary>
			public const int VariableExpResatom = 0x0017;
			/// <summary>
			/// The unique identifier for variable exp_factor
			/// </summary>
			public const int VariableExpFactor = 0x0018;
			/// <summary>
			/// The unique identifier for variable exp_term
			/// </summary>
			public const int VariableExpTerm = 0x0019;
			/// <summary>
			/// The unique identifier for variable exp_compare
			/// </summary>
			public const int VariableExpCompare = 0x001A;
			/// <summary>
			/// The unique identifier for variable exp
			/// </summary>
			public const int VariableExp = 0x001B;
			/// <summary>
			/// The unique identifier for variable exp_bracket
			/// </summary>
			public const int VariableExpBracket = 0x001C;
			/// <summary>
			/// The unique identifier for variable variable_assign_right
			/// </summary>
			public const int VariableVariableAssignRight = 0x001D;
			/// <summary>
			/// The unique identifier for variable variable_assign_left
			/// </summary>
			public const int VariableVariableAssignLeft = 0x001E;
			/// <summary>
			/// The unique identifier for variable variable
			/// </summary>
			public const int VariableVariable = 0x001F;
			/// <summary>
			/// The unique identifier for variable function_call_args
			/// </summary>
			public const int VariableFunctionCallArgs = 0x0020;
			/// <summary>
			/// The unique identifier for variable variable_type
			/// </summary>
			public const int VariableVariableType = 0x0021;
			/// <summary>
			/// The unique identifier for variable variable_init
			/// </summary>
			public const int VariableVariableInit = 0x0022;
			/// <summary>
			/// The unique identifier for variable statement
			/// </summary>
			public const int VariableStatement = 0x0023;
			/// <summary>
			/// The unique identifier for variable statement_list
			/// </summary>
			public const int VariableStatementList = 0x0024;
			/// <summary>
			/// The unique identifier for variable parametre
			/// </summary>
			public const int VariableParametre = 0x0025;
			/// <summary>
			/// The unique identifier for variable function_parametres
			/// </summary>
			public const int VariableFunctionParametres = 0x0026;
			/// <summary>
			/// The unique identifier for variable function_define
			/// </summary>
			public const int VariableFunctionDefine = 0x0027;
			/// <summary>
			/// The unique identifier for variable program
			/// </summary>
			public const int VariableProgram = 0x0028;
		}
		/// <summary>
		/// The collection of variables matched by this parser
		/// </summary>
		/// <remarks>
		/// The variables are in an order consistent with the automaton,
		/// so that variable indices in the automaton can be used to retrieve the variables in this table
		/// </remarks>
		private static readonly Symbol[] variables = {
			new Symbol(0x0015, "function_call"), 
			new Symbol(0x0016, "exp_atom"), 
			new Symbol(0x0017, "exp_resatom"), 
			new Symbol(0x0018, "exp_factor"), 
			new Symbol(0x0019, "exp_term"), 
			new Symbol(0x001A, "exp_compare"), 
			new Symbol(0x001B, "exp"), 
			new Symbol(0x001C, "exp_bracket"), 
			new Symbol(0x001D, "variable_assign_right"), 
			new Symbol(0x001E, "variable_assign_left"), 
			new Symbol(0x001F, "variable"), 
			new Symbol(0x0020, "function_call_args"), 
			new Symbol(0x0021, "variable_type"), 
			new Symbol(0x0022, "variable_init"), 
			new Symbol(0x0023, "statement"), 
			new Symbol(0x0024, "statement_list"), 
			new Symbol(0x0025, "parametre"), 
			new Symbol(0x0026, "function_parametres"), 
			new Symbol(0x0027, "function_define"), 
			new Symbol(0x0028, "program"), 
			new Symbol(0x0037, "__V55"), 
			new Symbol(0x0038, "__V56"), 
			new Symbol(0x003A, "__V58"), 
			new Symbol(0x003C, "__V60"), 
			new Symbol(0x003D, "__V61"), 
			new Symbol(0x003E, "__VAxiom") };
		/// <summary>
		/// The collection of virtuals matched by this parser
		/// </summary>
		/// <remarks>
		/// The virtuals are in an order consistent with the automaton,
		/// so that virtual indices in the automaton can be used to retrieve the virtuals in this table
		/// </remarks>
		private static readonly Symbol[] virtuals = {
 };
		/// <summary>
		/// Initializes a new instance of the parser
		/// </summary>
		/// <param name="lexer">The input lexer</param>
		public BydonParser(BydonLexer lexer) : base (commonAutomaton, variables, virtuals, null, lexer) { }

		/// <summary>
		/// Visitor interface
		/// </summary>
		public class Visitor
		{
			public virtual void OnTerminalWhiteSpace(ASTNode node) {}
			public virtual void OnTerminalSeparator(ASTNode node) {}
			public virtual void OnTerminalNumber(ASTNode node) {}
			public virtual void OnTerminalVariable(ASTNode node) {}
			public virtual void OnTerminalTiny(ASTNode node) {}
			public virtual void OnTerminalSmall(ASTNode node) {}
			public virtual void OnTerminalNormal(ASTNode node) {}
			public virtual void OnTerminalBig(ASTNode node) {}
			public virtual void OnTerminalField(ASTNode node) {}
			public virtual void OnTerminalDo(ASTNode node) {}
			public virtual void OnTerminalUntil(ASTNode node) {}
			public virtual void OnTerminalCheck(ASTNode node) {}
			public virtual void OnTerminalGo(ASTNode node) {}
			public virtual void OnTerminalRl(ASTNode node) {}
			public virtual void OnTerminalRr(ASTNode node) {}
			public virtual void OnTerminalSonar(ASTNode node) {}
			public virtual void OnTerminalPrint(ASTNode node) {}
			public virtual void OnTerminalReturn(ASTNode node) {}
			public virtual void OnVariableFunctionCall(ASTNode node) {}
			public virtual void OnVariableExpAtom(ASTNode node) {}
			public virtual void OnVariableExpResatom(ASTNode node) {}
			public virtual void OnVariableExpFactor(ASTNode node) {}
			public virtual void OnVariableExpTerm(ASTNode node) {}
			public virtual void OnVariableExpCompare(ASTNode node) {}
			public virtual void OnVariableExp(ASTNode node) {}
			public virtual void OnVariableExpBracket(ASTNode node) {}
			public virtual void OnVariableVariableAssignRight(ASTNode node) {}
			public virtual void OnVariableVariableAssignLeft(ASTNode node) {}
			public virtual void OnVariableVariable(ASTNode node) {}
			public virtual void OnVariableFunctionCallArgs(ASTNode node) {}
			public virtual void OnVariableVariableType(ASTNode node) {}
			public virtual void OnVariableVariableInit(ASTNode node) {}
			public virtual void OnVariableStatement(ASTNode node) {}
			public virtual void OnVariableStatementList(ASTNode node) {}
			public virtual void OnVariableParametre(ASTNode node) {}
			public virtual void OnVariableFunctionParametres(ASTNode node) {}
			public virtual void OnVariableFunctionDefine(ASTNode node) {}
			public virtual void OnVariableProgram(ASTNode node) {}
		}

		/// <summary>
		/// Walk the AST using a visitor
		/// </summary>
		public static void Visit(ParseResult result, Visitor visitor)
		{
			VisitASTNode(result.Root, visitor);
		}

		/// <summary>
		/// Walk the AST using a visitor
		/// </summary>
		public static void VisitASTNode(ASTNode node, Visitor visitor)
		{
			for (int i = 0; i < node.Children.Count; i++)
				VisitASTNode(node.Children[i], visitor);
			switch(node.Symbol.ID)
			{
				case 0x0003: visitor.OnTerminalWhiteSpace(node); break;
				case 0x0004: visitor.OnTerminalSeparator(node); break;
				case 0x0005: visitor.OnTerminalNumber(node); break;
				case 0x0006: visitor.OnTerminalVariable(node); break;
				case 0x0007: visitor.OnTerminalTiny(node); break;
				case 0x0008: visitor.OnTerminalSmall(node); break;
				case 0x0009: visitor.OnTerminalNormal(node); break;
				case 0x000A: visitor.OnTerminalBig(node); break;
				case 0x000B: visitor.OnTerminalField(node); break;
				case 0x000C: visitor.OnTerminalDo(node); break;
				case 0x000D: visitor.OnTerminalUntil(node); break;
				case 0x000E: visitor.OnTerminalCheck(node); break;
				case 0x000F: visitor.OnTerminalGo(node); break;
				case 0x0010: visitor.OnTerminalRl(node); break;
				case 0x0011: visitor.OnTerminalRr(node); break;
				case 0x0012: visitor.OnTerminalSonar(node); break;
				case 0x0013: visitor.OnTerminalPrint(node); break;
				case 0x0014: visitor.OnTerminalReturn(node); break;
				case 0x0015: visitor.OnVariableFunctionCall(node); break;
				case 0x0016: visitor.OnVariableExpAtom(node); break;
				case 0x0017: visitor.OnVariableExpResatom(node); break;
				case 0x0018: visitor.OnVariableExpFactor(node); break;
				case 0x0019: visitor.OnVariableExpTerm(node); break;
				case 0x001A: visitor.OnVariableExpCompare(node); break;
				case 0x001B: visitor.OnVariableExp(node); break;
				case 0x001C: visitor.OnVariableExpBracket(node); break;
				case 0x001D: visitor.OnVariableVariableAssignRight(node); break;
				case 0x001E: visitor.OnVariableVariableAssignLeft(node); break;
				case 0x001F: visitor.OnVariableVariable(node); break;
				case 0x0020: visitor.OnVariableFunctionCallArgs(node); break;
				case 0x0021: visitor.OnVariableVariableType(node); break;
				case 0x0022: visitor.OnVariableVariableInit(node); break;
				case 0x0023: visitor.OnVariableStatement(node); break;
				case 0x0024: visitor.OnVariableStatementList(node); break;
				case 0x0025: visitor.OnVariableParametre(node); break;
				case 0x0026: visitor.OnVariableFunctionParametres(node); break;
				case 0x0027: visitor.OnVariableFunctionDefine(node); break;
				case 0x0028: visitor.OnVariableProgram(node); break;
			}
		}
	}
}
