using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hime.Redist;
using Bydon;
namespace TestHime
{
    public class BydonInterpreter
    {
        Dictionary<string, VariableType> globalVarTable = new Dictionary<string, VariableType>();

        public void Interpret(ParseResult parseResult)
        {
            int result = Process(parseResult.Root);
            Console.WriteLine(result);
        }
        int Process(ASTNode node)
        {
            if(node.Symbol.ID == BydonLexer.ID.TerminalNumber)
            {
                return Convert32To10(node.Value);
            }
            else if(node.Symbol.ID == BydonParser.ID.VariableVariableInit)
            {
                VariableType variableType;
                variableType.typename = node.Children[0].Value;
                variableType.value = Process(node.Children[2]);
                string varName = node.Children[1].Value;

                globalVarTable.Add(varName, variableType);

                return variableType.value;
            }

            switch (node.Symbol.Name)
            {
                case "*":
                    {
                        return Process(node.Children[0]) * Process(node.Children[1]);
                        break;
                    }
                case "/":
                    {
                        return Process(node.Children[0]) / Process(node.Children[1]);
                        break;
                    }
                case "+":
                    {
                        if (node.Children.Count == 1)
                        {
                            return +Process(node.Children[0]);
                        }
                        else
                        {
                            return Process(node.Children[0]) + Process(node.Children[1]);
                        }
                    }
                case "-":
                    {
                        if (node.Children.Count == 1)
                        {
                            return -Process(node.Children[0]);
                        }
                        else
                        {
                            return Process(node.Children[0]) - Process(node.Children[1]);
                        }
                        break;
                    }
                case "<":
                    {
                        if (Process(node.Children[0]) < Process(node.Children[1]))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case ">":
                    {
                        if (Process(node.Children[0]) > Process(node.Children[1]))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case "<=":
                    {
                        if (Process(node.Children[0]) <= Process(node.Children[1]))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case ">=":
                    {
                        if (Process(node.Children[0]) >= Process(node.Children[1]))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case "=":
                    {
                        if (Process(node.Children[0]) == Process(node.Children[1]))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case "<>":
                    {
                        if (Process(node.Children[0]) != Process(node.Children[1]))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
            }
            Console.WriteLine("\n\nUnknown character");
            Console.WriteLine(node.ToString() + " " + node.Symbol + " " + node.SymbolType + " " + node.Span + " " + node.Value + " " + node.Context.Content);
            throw new Exception("Unknown character");
            return int.MinValue;
        }


        public static int Convert32To10(string value)
        {
            int result = 0;
            int add = 0;
            int i;
            if (value[0] == '-' || value[0]=='+')
            {
                i = 1;
            }
            else
            {
                i = 0;
            }
            for(; i < value.Length; i++)
            {
                if(value[i] >= '0' && value[i] <= '9')
                {
                    add = int.Parse(value[i].ToString());
                }
                else if(value[i] >= 'A' && value[i] <= 'V')
                {
                    add = value[i] - 'A' + 10;
                }
                else
                {
                    throw new Exception("Unknown character");
                }
                result = result * 32 + add;
            }

            if (value[0] == '-')
            {
                return -result;
            }
            else
            {
                return result;
            }
        }

        public struct VariableType
        {
            public string typename;
            public int value;
        }

    }
}
