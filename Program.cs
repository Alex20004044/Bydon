using System;
using Hime.Redist; // the namespace for the Hime Runtime
using Bydon; // default namespace for the parser is the grammar's name
using System.IO;
namespace TestHime
{
    class Program
    {
        public static void Main(string[] args)
        {
          
                string path = @"D:\Bydon\Programs\ParserDebug_1.bd";
                StreamReader streamReader = new StreamReader(path);
                // Creates the lexer and parser 

                BydonLexer lexer = new BydonLexer(streamReader);
                BydonParser parser = new BydonParser(lexer);
                // Executes the parsing
                ParseResult result = parser.Parse();
                streamReader.Close();
                // Prints the produced syntax tree
                Print(result.Root, new bool[] { });

                BydonInterpreter bydonInterpreter = new BydonInterpreter();
                bydonInterpreter.Interpret(result);
        }

        private static void Print(ASTNode node, bool[] crossings)
        {
            for (int i = 0; i < crossings.Length - 1; i++)
                Console.Write(crossings[i] ? "|   " : "    ");
            if (crossings.Length > 0)
                Console.Write("+-> ");
            Console.WriteLine(node.ToString());// + " " + node.Symbol + " " + node.SymbolType + " " + node.Span + " " + node.Value + " " + node.Context.Content);
            for (int i = 0; i != node.Children.Count; i++)
            {
                bool[] childCrossings = new bool[crossings.Length + 1];
                Array.Copy(crossings, childCrossings, crossings.Length);
                childCrossings[childCrossings.Length - 1] = (i < node.Children.Count - 1);
                Print(node.Children[i], childCrossings);
            }
        }
        

    }
}