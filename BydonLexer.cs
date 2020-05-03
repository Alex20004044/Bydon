/*
 * WARNING: this file has been generated by
 * Hime Parser Generator 3.4.0.0
 */
using System.Collections.Generic;
using System.IO;
using Hime.Redist;
using Hime.Redist.Lexer;

namespace Bydon
{
	/// <summary>
	/// Represents a lexer
	/// </summary>
	internal class BydonLexer : ContextFreeLexer
	{
		/// <summary>
		/// The automaton for this lexer
		/// </summary>
		private static readonly Automaton commonAutomaton = Automaton.Find(typeof(BydonLexer), "BydonLexer.bin");
		/// <summary>
		/// Contains the constant IDs for the terminals for this lexer
		/// </summary>
		public class ID
		{
			/// <summary>
			/// The unique identifier for terminal WHITE_SPACE
			/// </summary>
			public const int TerminalWhiteSpace = 0x0003;
			/// <summary>
			/// The unique identifier for terminal SEPARATOR
			/// </summary>
			public const int TerminalSeparator = 0x0004;
			/// <summary>
			/// The unique identifier for terminal NUMBER
			/// </summary>
			public const int TerminalNumber = 0x0005;
			/// <summary>
			/// The unique identifier for terminal VARIABLE
			/// </summary>
			public const int TerminalVariable = 0x0006;
			/// <summary>
			/// The unique identifier for terminal TINY
			/// </summary>
			public const int TerminalTiny = 0x0007;
			/// <summary>
			/// The unique identifier for terminal SMALL
			/// </summary>
			public const int TerminalSmall = 0x0008;
			/// <summary>
			/// The unique identifier for terminal NORMAL
			/// </summary>
			public const int TerminalNormal = 0x0009;
			/// <summary>
			/// The unique identifier for terminal BIG
			/// </summary>
			public const int TerminalBig = 0x000A;
			/// <summary>
			/// The unique identifier for terminal FIELD
			/// </summary>
			public const int TerminalField = 0x000B;
			/// <summary>
			/// The unique identifier for terminal DO
			/// </summary>
			public const int TerminalDo = 0x000C;
			/// <summary>
			/// The unique identifier for terminal UNTIL
			/// </summary>
			public const int TerminalUntil = 0x000D;
			/// <summary>
			/// The unique identifier for terminal CHECK
			/// </summary>
			public const int TerminalCheck = 0x000E;
			/// <summary>
			/// The unique identifier for terminal GO
			/// </summary>
			public const int TerminalGo = 0x000F;
			/// <summary>
			/// The unique identifier for terminal RL
			/// </summary>
			public const int TerminalRl = 0x0010;
			/// <summary>
			/// The unique identifier for terminal RR
			/// </summary>
			public const int TerminalRr = 0x0011;
			/// <summary>
			/// The unique identifier for terminal SONAR
			/// </summary>
			public const int TerminalSonar = 0x0012;
			/// <summary>
			/// The unique identifier for terminal RETURN
			/// </summary>
			public const int TerminalReturn = 0x0013;
		}
		/// <summary>
		/// Contains the constant IDs for the contexts for this lexer
		/// </summary>
		public class Context
		{
			/// <summary>
			/// The unique identifier for the default context
			/// </summary>
			public const int Default = 0;
		}
		/// <summary>
		/// The collection of terminals matched by this lexer
		/// </summary>
		/// <remarks>
		/// The terminals are in an order consistent with the automaton,
		/// so that terminal indices in the automaton can be used to retrieve the terminals in this table
		/// </remarks>
		private static readonly Symbol[] terminals = {
			new Symbol(0x0001, "ε"),
			new Symbol(0x0002, "$"),
			new Symbol(0x0003, "WHITE_SPACE"),
			new Symbol(0x0004, "SEPARATOR"),
			new Symbol(0x0005, "NUMBER"),
			new Symbol(0x0006, "VARIABLE"),
			new Symbol(0x0007, "TINY"),
			new Symbol(0x0008, "SMALL"),
			new Symbol(0x0009, "NORMAL"),
			new Symbol(0x000A, "BIG"),
			new Symbol(0x000B, "FIELD"),
			new Symbol(0x000C, "DO"),
			new Symbol(0x000D, "UNTIL"),
			new Symbol(0x000E, "CHECK"),
			new Symbol(0x000F, "GO"),
			new Symbol(0x0010, "RL"),
			new Symbol(0x0011, "RR"),
			new Symbol(0x0012, "SONAR"),
			new Symbol(0x0013, "RETURN"),
			new Symbol(0x001D, "("),
			new Symbol(0x001E, ")"),
			new Symbol(0x001F, "*"),
			new Symbol(0x0020, "/"),
			new Symbol(0x0021, "-"),
			new Symbol(0x0022, "+"),
			new Symbol(0x0023, "<"),
			new Symbol(0x0024, ">"),
			new Symbol(0x0025, "<="),
			new Symbol(0x0026, ">="),
			new Symbol(0x0027, "="),
			new Symbol(0x0028, "<>"),
			new Symbol(0x0029, "<<") };
		/// <summary>
		/// Initializes a new instance of the lexer
		/// </summary>
		/// <param name="input">The lexer's input</param>
		public BydonLexer(string input) : base(commonAutomaton, terminals, 0x0004, input) {}
		/// <summary>
		/// Initializes a new instance of the lexer
		/// </summary>
		/// <param name="input">The lexer's input</param>
		public BydonLexer(TextReader input) : base(commonAutomaton, terminals, 0x0004, input) {}
	}
}