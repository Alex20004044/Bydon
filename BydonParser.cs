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
			public const int VariableFunctionCall = 0x0016;
			/// <summary>
			/// The unique identifier for variable exp_atom
			/// </summary>
			public const int VariableExpAtom = 0x0017;
			/// <summary>
			/// The unique identifier for variable exp_resatom
			/// </summary>
			public const int VariableExpResatom = 0x0018;
			/// <summary>
			/// The unique identifier for variable exp_factor
			/// </summary>
			public const int VariableExpFactor = 0x0019;
			/// <summary>
			/// The unique identifier for variable exp_term
			/// </summary>
			public const int VariableExpTerm = 0x001A;
			/// <summary>
			/// The unique identifier for variable exp_compare
			/// </summary>
			public const int VariableExpCompare = 0x001B;
			/// <summary>
			/// The unique identifier for variable exp_assign_right
			/// </summary>
			public const int VariableExpAssignRight = 0x001C;
			/// <summary>
			/// The unique identifier for variable exp_assign_left
			/// </summary>
			public const int VariableExpAssignLeft = 0x001D;
			/// <summary>
			/// The unique identifier for variable exp
			/// </summary>
			public const int VariableExp = 0x001E;
			/// <summary>
			/// The unique identifier for variable exp_bracket
			/// </summary>
			public const int VariableExpBracket = 0x001F;
			/// <summary>
			/// The unique identifier for variable field_init
			/// </summary>
			public const int VariableFieldInit = 0x0020;
			/// <summary>
			/// The unique identifier for variable variable_init
			/// </summary>
			public const int VariableVariableInit = 0x0021;
			/// <summary>
			/// The unique identifier for variable indexator
			/// </summary>
			public const int VariableIndexator = 0x0022;
			/// <summary>
			/// The unique identifier for variable function_call_args
			/// </summary>
			public const int VariableFunctionCallArgs = 0x0023;
			/// <summary>
			/// The unique identifier for variable variable_type
			/// </summary>
			public const int VariableVariableType = 0x0024;
			/// <summary>
			/// The unique identifier for variable statement
			/// </summary>
			public const int VariableStatement = 0x0025;
			/// <summary>
			/// The unique identifier for variable statement_list
			/// </summary>
			public const int VariableStatementList = 0x0026;
			/// <summary>
			/// The unique identifier for variable parametre
			/// </summary>
			public const int VariableParametre = 0x0027;
			/// <summary>
			/// The unique identifier for variable function_parametres
			/// </summary>
			public const int VariableFunctionParametres = 0x0028;
			/// <summary>
			/// The unique identifier for variable function_define_variable
			/// </summary>
			public const int VariableFunctionDefineVariable = 0x0029;
			/// <summary>
			/// The unique identifier for variable function_define_field
			/// </summary>
			public const int VariableFunctionDefineField = 0x002A;
			/// <summary>
			/// The unique identifier for variable function_define
			/// </summary>
			public const int VariableFunctionDefine = 0x002B;
			/// <summary>
			/// The unique identifier for variable check
			/// </summary>
			public const int VariableCheck = 0x002C;
			/// <summary>
			/// The unique identifier for variable until
			/// </summary>
			public const int VariableUntil = 0x002D;
			/// <summary>
			/// The unique identifier for variable program
			/// </summary>
			public const int VariableProgram = 0x002E;
		}
		/// <summary>
		/// The collection of variables matched by this parser
		/// </summary>
		/// <remarks>
		/// The variables are in an order consistent with the automaton,
		/// so that variable indices in the automaton can be used to retrieve the variables in this table
		/// </remarks>
		private static readonly Symbol[] variables = {
			new Symbol(0x0016, "function_call"), 
			new Symbol(0x0017, "exp_atom"), 
			new Symbol(0x0018, "exp_resatom"), 
			new Symbol(0x0019, "exp_factor"), 
			new Symbol(0x001A, "exp_term"), 
			new Symbol(0x001B, "exp_compare"), 
			new Symbol(0x001C, "exp_assign_right"), 
			new Symbol(0x001D, "exp_assign_left"), 
			new Symbol(0x001E, "exp"), 
			new Symbol(0x001F, "exp_bracket"), 
			new Symbol(0x0020, "field_init"), 
			new Symbol(0x0021, "variable_init"), 
			new Symbol(0x0022, "indexator"), 
			new Symbol(0x0023, "function_call_args"), 
			new Symbol(0x0024, "variable_type"), 
			new Symbol(0x0025, "statement"), 
			new Symbol(0x0026, "statement_list"), 
			new Symbol(0x0027, "parametre"), 
			new Symbol(0x0028, "function_parametres"), 
			new Symbol(0x0029, "function_define_variable"), 
			new Symbol(0x002A, "function_define_field"), 
			new Symbol(0x002B, "function_define"), 
			new Symbol(0x002C, "check"), 
			new Symbol(0x002D, "until"), 
			new Symbol(0x002E, "program"), 
			new Symbol(0x003D, "__V61"), 
			new Symbol(0x003E, "__V62"), 
			new Symbol(0x0041, "__V65"), 
			new Symbol(0x0043, "__V67"), 
			new Symbol(0x0045, "__V69"), 
			new Symbol(0x0046, "__V70"), 
			new Symbol(0x0047, "__VAxiom") };
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
			public virtual void OnTerminalCompass(ASTNode node) {}
			public virtual void OnTerminalPrint(ASTNode node) {}
			public virtual void OnTerminalReturn(ASTNode node) {}
			public virtual void OnVariableFunctionCall(ASTNode node) {}
			public virtual void OnVariableExpAtom(ASTNode node) {}
			public virtual void OnVariableExpResatom(ASTNode node) {}
			public virtual void OnVariableExpFactor(ASTNode node) {}
			public virtual void OnVariableExpTerm(ASTNode node) {}
			public virtual void OnVariableExpCompare(ASTNode node) {}
			public virtual void OnVariableExpAssignRight(ASTNode node) {}
			public virtual void OnVariableExpAssignLeft(ASTNode node) {}
			public virtual void OnVariableExp(ASTNode node) {}
			public virtual void OnVariableExpBracket(ASTNode node) {}
			public virtual void OnVariableFieldInit(ASTNode node) {}
			public virtual void OnVariableVariableInit(ASTNode node) {}
			public virtual void OnVariableIndexator(ASTNode node) {}
			public virtual void OnVariableFunctionCallArgs(ASTNode node) {}
			public virtual void OnVariableVariableType(ASTNode node) {}
			public virtual void OnVariableStatement(ASTNode node) {}
			public virtual void OnVariableStatementList(ASTNode node) {}
			public virtual void OnVariableParametre(ASTNode node) {}
			public virtual void OnVariableFunctionParametres(ASTNode node) {}
			public virtual void OnVariableFunctionDefineVariable(ASTNode node) {}
			public virtual void OnVariableFunctionDefineField(ASTNode node) {}
			public virtual void OnVariableFunctionDefine(ASTNode node) {}
			public virtual void OnVariableCheck(ASTNode node) {}
			public virtual void OnVariableUntil(ASTNode node) {}
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
				case 0x0013: visitor.OnTerminalCompass(node); break;
				case 0x0014: visitor.OnTerminalPrint(node); break;
				case 0x0015: visitor.OnTerminalReturn(node); break;
				case 0x0016: visitor.OnVariableFunctionCall(node); break;
				case 0x0017: visitor.OnVariableExpAtom(node); break;
				case 0x0018: visitor.OnVariableExpResatom(node); break;
				case 0x0019: visitor.OnVariableExpFactor(node); break;
				case 0x001A: visitor.OnVariableExpTerm(node); break;
				case 0x001B: visitor.OnVariableExpCompare(node); break;
				case 0x001C: visitor.OnVariableExpAssignRight(node); break;
				case 0x001D: visitor.OnVariableExpAssignLeft(node); break;
				case 0x001E: visitor.OnVariableExp(node); break;
				case 0x001F: visitor.OnVariableExpBracket(node); break;
				case 0x0020: visitor.OnVariableFieldInit(node); break;
				case 0x0021: visitor.OnVariableVariableInit(node); break;
				case 0x0022: visitor.OnVariableIndexator(node); break;
				case 0x0023: visitor.OnVariableFunctionCallArgs(node); break;
				case 0x0024: visitor.OnVariableVariableType(node); break;
				case 0x0025: visitor.OnVariableStatement(node); break;
				case 0x0026: visitor.OnVariableStatementList(node); break;
				case 0x0027: visitor.OnVariableParametre(node); break;
				case 0x0028: visitor.OnVariableFunctionParametres(node); break;
				case 0x0029: visitor.OnVariableFunctionDefineVariable(node); break;
				case 0x002A: visitor.OnVariableFunctionDefineField(node); break;
				case 0x002B: visitor.OnVariableFunctionDefine(node); break;
				case 0x002C: visitor.OnVariableCheck(node); break;
				case 0x002D: visitor.OnVariableUntil(node); break;
				case 0x002E: visitor.OnVariableProgram(node); break;
			}
		}
	}
}
