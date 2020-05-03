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
        int errorCounter = 1;
        public void Interpret(ParseResult parseResult)
        {
            int result = Process(parseResult.Root);
            if(errorCounter == 1)
            {
                Console.WriteLine("Succesful compilation");
            }
            else
            {
                Console.WriteLine("Error compilation");
            }
            Console.WriteLine("Result" + result);
        }
        int Process(ASTNode node)
        {
            if(node.Symbol.ID == BydonLexer.ID.TerminalNumber)
            {
                return Convert32To10(node.Value);
            }



            else if(node.Symbol.ID == BydonLexer.ID.TerminalVariable)
            {
                VariableType variableType;
                if(globalVarTable.TryGetValue(node.Value, out variableType))
                {
                    return variableType.value;
                }
                else
                {
                    //Error
                    PrintError("Variable " + node.Value + " is undefined");
                    return int.MaxValue;
                }
            }
            //Variable construction
            else if(node.Symbol.ID == BydonParser.ID.VariableVariableInit)
            {
                VariableType variableType;
                int exp = Process(node.Children[node.Children.Count - 1]);
                for (int i = 1; i < node.Children.Count - 1; i++)
                {
                    variableType.typename = node.Children[0].Value;
                    variableType.value = exp;
                    string varName = node.Children[i].Value;

                    if (globalVarTable.ContainsKey(varName))
                    {
                        //Error
                        PrintError(varName + " variable already exists. Second initializtion is canceled. ");
                    }
                    else
                    {
                        globalVarTable.Add(varName, variableType);
                    }
                }
                return exp;
            }
            //Function process
            else if(node.Symbol.ID == BydonParser.ID.VariableFunction)
            {
                Process(node.Children[0]);
                return int.MinValue;
            }
            //Statement list process 
            else if (node.Symbol.ID == BydonParser.ID.VariableStatementList)
            {
                for(int i = 0; i < node.Children.Count; i++)
                {
                    Process(node.Children[i]);
                }
                return int.MinValue;
            }
            //Statement process
            else if(node.Symbol.ID == BydonParser.ID.VariableStatement)
            {

                Process(node.Children[0]);
                
                return int.MinValue;
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
        
        void PrintError(string message)
        {
            Console.WriteLine(errorCounter + ")" + message);
            errorCounter++;
        }
    }
}
