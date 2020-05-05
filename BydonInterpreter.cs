using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Hime.Redist;
using Bydon;
namespace TestHime
{
    public class BydonInterpreter
    {
        

        Dictionary<string, ASTNode> gloabalFuncTable = new Dictionary<string, ASTNode>();
        Stack<Dictionary<string, VariableType>> functionStack = new Stack<Dictionary<string, VariableType>>();
        bool isReturn = false;

        

        int errorCounter = 1;
        TextWriter printOutput;
        public void Interpret(ParseResult parseResult, TextWriter _printOutput = null)
        {
            printOutput = _printOutput;
            int result = Process(parseResult.Root);
            if (_printOutput == null)
            {
                if (errorCounter == 1)
                {
                    Console.WriteLine("Succesful compilation");
                }
                else
                {
                    Console.WriteLine("Error compilation");
                }
                Console.WriteLine("Result " + result);
            }
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
                if(functionStack.Peek().TryGetValue(node.Value, out variableType))
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
            else if (node.Symbol.ID == BydonLexer.ID.TerminalReturn)
            {
                VariableType variableType;
                functionStack.Peek().TryGetValue("return", out variableType);
                variableType.value = Process(node.Children[0]);
                isReturn = true;
                return int.MinValue;
            }
            else if(node.Symbol.ID == BydonLexer.ID.TerminalPrint)
            {
                PrintFunction(Process(node.Children[0]).ToString());
                return int.MinValue;
            }

            //Variable construction
            else if(node.Symbol.ID == BydonParser.ID.VariableVariableInit)
            {
                VariableType variableType = new VariableType();
                int exp = Process(node.Children[node.Children.Count - 1]);
                for (int i = 1; i < node.Children.Count - 1; i++)
                {
                    variableType.typename = node.Children[0].Value;
                    variableType.value = exp;
                    string varName = node.Children[i].Value;

                    if (functionStack.Peek().ContainsKey(varName))
                    {
                        //Error
                        PrintError(varName + " variable already exists. Second initializtion is canceled. ");
                    }
                    else
                    {
                        functionStack.Peek().Add(varName, variableType);
                    }
                }
                return exp;
            }
            //Function process
            else if (node.Symbol.ID == BydonParser.ID.VariableFunctionCall)
            {
                ASTNode func;
                if(gloabalFuncTable.TryGetValue(node.Children[0].Value, out func))
                {
                    Process(node.Children[1]);
                    return Process(func);
                }
                else
                {
                    PrintError(node.Children[0].Value + " is undefined function");
                }
            }
            //Create new stacc and add default variables
            else if(node.Symbol.ID == BydonParser.ID.VariableFunctionCallArgs)
            {
                Dictionary<string, VariableType> varStack = new Dictionary<string, VariableType>();
                for (int i = 0; i < node.Children.Count; i++)
                {
                    varStack.Add(i.ToString(), DefineVariableType(node.Children[i]));
                }
                functionStack.Push(varStack);
                return int.MinValue;
            }
            //Function define
            else if(node.Symbol.ID == BydonParser.ID.VariableFunctionDefine)
            {
                VariableType returnVariable = new VariableType();
                returnVariable.typename = node.Children[0].Value;
                returnVariable.value = Process(node.Children[3]);
                functionStack.Peek().Add("return", returnVariable);

                Process(node.Children[2]);
                Process(node.Children[4]);

                return functionStack.Pop()["return"].value;
            }
            

            else if(node.Symbol.ID == BydonParser.ID.VariableFunctionParametres)
            {
                Dictionary<string, VariableType> varTable = functionStack.Peek();
                VariableType variableType;
                VariableType stackVar;
                for (int i = 0; i < node.Children.Count; i++)
                {
                    Process(node.Children[i]);
                    varTable.TryGetValue(node.Children[i].Children[1].Value, out variableType);
                    if(varTable.TryGetValue((i).ToString(), out stackVar))
                    {
                        variableType.value = stackVar.value;
                        varTable.Remove((i).ToString());
                    }

                }
                return int.MinValue;
            }


            //Statement list process 
            else if (node.Symbol.ID == BydonParser.ID.VariableStatementList)
            {
                for(int i = 0; i < node.Children.Count; i++)
                {
                    int value = Process(node.Children[i]);
                    if (isReturn)
                    {
                        isReturn = false;
                        return value;
                    }
                }
                return int.MinValue;
            }
            //Attention

            else if(node.Symbol.ID == BydonParser.ID.VariableProgram)
            {
                for(int i = 0; i < node.Children.Count; i++)
                {
                    InitFunc(node.Children[i]);
                }
                ASTNode funcNode;
                if(gloabalFuncTable.TryGetValue("main", out funcNode))
                {
                    functionStack.Push(new Dictionary<string, VariableType>());
                    int t =  Process(funcNode);
                    return t;
                }
                else
                {
                    PrintError("Main function is not found");
                    return int.MinValue;
                }
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
                        int num1 = Process(node.Children[0]);
                        int num2 = Process(node.Children[1]);
                        if(num2 == 0)
                        {
                            return int.MaxValue;
                        }
                        else
                        {
                            return num1 / num2;
                        }
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

        void InitFunc(ASTNode node)
        {
            string funcName = node.Children[1].Value;
            if (gloabalFuncTable.ContainsKey(funcName))
            {
                PrintError(funcName + " function already exists. Second initializtion is canceled. ");
            }
            else
            {
                gloabalFuncTable.Add(funcName, node);
            }
        }
        VariableType DefineVariableType(ASTNode node)
        {
            VariableType variable = new VariableType();
            variable.typename = "big";
            variable.value = Process(node);
            return variable;
        }

        void PrintFunction(string exp)
        {
            if (printOutput == null)
            {
                Console.WriteLine("PRINT: " + exp);
            }
            else
            {
                printOutput.WriteLine(exp);
            }
        }

        public class VariableType
        {
            public string typename;
            public int value;
        }
        public class FunctionType
        {
            public VariableType returnVariable;
            public Dictionary<string, VariableType> localVarTab = new Dictionary<string, VariableType>();
            public ASTNode functionNode;
            public string funcName;
        }
        
        void PrintError(string message)
        {
            Console.WriteLine(errorCounter + ")" + message);
            errorCounter++;
        }
    }
}
