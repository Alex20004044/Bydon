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
            VariableType result = Process(parseResult.Root);
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
                Console.WriteLine("Result " + result.GetValue());
            }
        }
        VariableType Process(ASTNode node)
        {
            if(node.Symbol.ID == BydonLexer.ID.TerminalNumber)
            {
                return VariableType.CreateLiteral(node.Value);
            }
            else if(node.Symbol.ID == BydonLexer.ID.TerminalVariable)
            {
                VariableType variableType;
                if(functionStack.Peek().TryGetValue(node.Value, out variableType))
                {
                    return variableType;
                }
                else
                {
                    //Error
                    PrintError("Variable " + node.Value + " is undefined");
                    return null;
                }
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalReturn)
            {
                VariableType variableType;
                functionStack.Peek().TryGetValue("return", out variableType);
                variableType = Process(node.Children[0]);
                isReturn = true;
                return null;
            }
            else if(node.Symbol.ID == BydonLexer.ID.TerminalPrint)
            {
                PrintFunction(Process(node.Children[0]).GetValue().ToString());
                return null;
            }
            
            else if(node.Symbol.ID == BydonParser.ID.VariableVariableAssignRight)
            {
                VariableType variableType;
                if (functionStack.Peek().TryGetValue(node.Children[1].Value, out variableType))
                {
                    variableType.SetValue(Process(node.Children[0]).GetValue());
                    return variableType;
                }
                else
                {
                    PrintError("Variable " + node.Children[1].Value + " is undefined");
                    return null;
                }
            }
            else if (node.Symbol.ID == BydonParser.ID.VariableVariableAssignLeft)
            {
                VariableType variableType;
                if (functionStack.Peek().TryGetValue(node.Children[0].Value, out variableType))
                {
                    variableType.SetValue(Process(node.Children[1]).GetValue());
                    return variableType;
                }
                else
                {
                    PrintError("Variable " + node.Children[0].Value + " is undefined");
                    return null;
                }
            }

            //Variable construction
            else if(node.Symbol.ID == BydonParser.ID.VariableVariableInit)
            {
                
                VariableType exp = Process(node.Children[node.Children.Count - 1]);
                for (int i = 1; i < node.Children.Count - 1; i++)
                {
                    VariableType variableType = VariableType.CreateVariable(DefineVariableTypeName(node.Children[0]), exp);
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
                    return null;
                }
            }
            //Create new stack and add default variables
            else if(node.Symbol.ID == BydonParser.ID.VariableFunctionCallArgs)
            {
                Dictionary<string, VariableType> varDict = new Dictionary<string, VariableType>();
                for (int i = 0; i < node.Children.Count; i++)
                {
                    varDict.Add(i.ToString(), Process(node.Children[i]));
                }
                functionStack.Push(varDict);
                return null;
            }
            //Function define
            else if(node.Symbol.ID == BydonParser.ID.VariableFunctionDefine)
            {
                //attention
                VariableType returnVariable = VariableType.CreateVariable(DefineVariableTypeName(node.Children[0]), Process(node.Children[3]));
                functionStack.Peek().Add("return", returnVariable);

                Process(node.Children[2]);
                Process(node.Children[4]);

                return functionStack.Pop()["return"];
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
                        variableType.SetValue(stackVar.GetValue());
                        varTable.Remove((i).ToString());
                    }

                }
                return null;
            }


            //Statement list process 
            else if (node.Symbol.ID == BydonParser.ID.VariableStatementList)
            {
                for(int i = 0; i < node.Children.Count; i++)
                {
                    VariableType value = Process(node.Children[i]);
                    if (isReturn)
                    {
                        isReturn = false;
                        return value;
                    }
                }
                return null;
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
                    VariableType t =  Process(funcNode);
                    return t;
                }
                else
                {
                    PrintError("Main function is not found");
                    return null;
                }
            }


            switch (node.Symbol.Name)
            {
                case "*":
                    {
                        return VariableType.OpMult(Process(node.Children[0]), Process(node.Children[1]));
                        break;
                    }
                case "/":
                    {
                        return VariableType.OpDiv(Process(node.Children[0]), Process(node.Children[1]));
                        /*
                        VariableType num1 = Process(node.Children[0]);
                        VariableType num2 = Process(node.Children[1]);
                        if(num2.GetValue() == 0)
                        {
                            return new VariableType(int.MaxValue, num1.TypeName);
                        }
                        else
                        {
                            return new VariableType(num1.GetValue() / num2.GetValue());
                        }
                        break;*/
                    }
                case "+":
                    {
                        if (node.Children.Count == 1)
                        {
                            return VariableType.OpUPlus(Process(node.Children[0]));
                        }
                        else
                        {
                            return VariableType.OpSum(Process(node.Children[0]), Process(node.Children[1]));
                        }
                    }
                case "-":
                    {
                        if (node.Children.Count == 1)
                        {
                            return VariableType.OpUMinus(Process(node.Children[0]));
                        }
                        else
                        {
                            return VariableType.OpSub(Process(node.Children[0]), Process(node.Children[1]));
                        }
                        break;
                    }
                case "<":
                    {
                        if (Process(node.Children[0]).GetValue() < Process(node.Children[1]).GetValue())
                        {
                            return VariableType.CreateLiteral(1);
                        }
                        else
                        {
                            return VariableType.CreateLiteral(0);
                        }
                    }
                case ">":
                    {
                        if (Process(node.Children[0]).GetValue() > Process(node.Children[1]).GetValue())
                        {
                            return VariableType.CreateLiteral(1);
                        }
                        else
                        {
                            return VariableType.CreateLiteral(0);
                        }
                    }
                case "<=":
                    {
                        if (Process(node.Children[0]).GetValue() <= Process(node.Children[1]).GetValue())
                        {
                            return VariableType.CreateLiteral(1);
                        }
                        else
                        {
                            return VariableType.CreateLiteral(0);
                        }
                    }
                case ">=":
                    {
                        if (Process(node.Children[0]).GetValue() >= Process(node.Children[1]).GetValue())
                        {
                            return VariableType.CreateLiteral(1);
                        }
                        else
                        {
                            return VariableType.CreateLiteral(0);
                        }
                    }
                case "=":
                    {
                        if (Process(node.Children[0]).GetValue() == Process(node.Children[1]).GetValue())
                        {
                            return VariableType.CreateLiteral(1);
                        }
                        else
                        {
                            return VariableType.CreateLiteral(0);
                        }
                    }
                case "<>":
                    {
                        if (Process(node.Children[0]).GetValue() != Process(node.Children[1]).GetValue())
                        {
                            return VariableType.CreateLiteral(1);
                        }
                        else
                        {
                            return VariableType.CreateLiteral(0);
                        }
                    }
            }
            Console.WriteLine("\n\nUnknown character");
            Console.WriteLine(node.ToString() + " " + node.Symbol + " " + node.SymbolType + " " + node.Span + " " + node.Value + " " + node.Context.Content);
            throw new Exception("Unknown character");
            return null;
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
        public static int Convert32To10(string value, out bool isWasSigned)
        {
            int result = 0;
            int add = 0;
            int i;
            if (value[0] == '-' || value[0] == '+')
            {
                i = 1;
                isWasSigned = true;
            }
            else
            {
                i = 0;
                isWasSigned = false;
            }
            for (; i < value.Length; i++)
            {
                if (value[i] >= '0' && value[i] <= '9')
                {
                    add = int.Parse(value[i].ToString());
                }
                else if (value[i] >= 'A' && value[i] <= 'V')
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
        /*
        VariableType DefineVariableType(ASTNode node)
        {
            VariableType variable = new VariableType();
            variable.typename = "big";
            variable.value = Process(node);
            return variable;
        }*/
        VariableTypeName DefineVariableTypeName(ASTNode node)
        {
            if (node.Symbol.ID == BydonLexer.ID.TerminalTiny)
            {
                return VariableTypeName.tiny;
            }
            else if(node.Symbol.ID == BydonLexer.ID.TerminalSmall)
            {
                return VariableTypeName.small;
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalNormal)
            {
                return VariableTypeName.normal;
            }
            else if(node.Symbol.ID == BydonLexer.ID.TerminalBig)
            {
                return VariableTypeName.big;
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalField)
            {
                return VariableTypeName.field;
            }
            throw new Exception("Try to define unknown type");
            return VariableTypeName.literal; 
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
            public VariableTypeName TypeName { get; private set; }
            bool isSigned;
            int value;
            VariableType()
            {
                
            }
            public static VariableType CreateLiteral(string number)
            {
                VariableType variable = new VariableType();
                variable.TypeName = VariableTypeName.literal;
                int _value = Convert32To10(number, out variable.isSigned);
                variable.value = _value;
                return variable;
            }
            public static VariableType CreateLiteral(int _value)
            {
                VariableType variable = new VariableType();
                if (_value < 0)
                {
                    variable.isSigned = true;
                }
                else
                {
                    variable.isSigned = false;
                }
                variable.value = _value;
                variable.TypeName = VariableTypeName.literal;
                return variable;
            }
            public static VariableType CreateVariable(VariableTypeName typeName, VariableType number)
            {
                VariableType variable = new VariableType();
                variable.TypeName = typeName;
                variable.isSigned = number.isSigned;
                variable.SetValue(number.GetValue());
                return variable;  
            }        
            public bool GetIsSigned()
            {
                return isSigned;
            }
            public int GetValue()
            {
                return value;
            }
            public void SetValue(int _value)
            {
                switch (TypeName)
                {
                    case VariableTypeName.literal:
                        {
                            throw new Exception("Attempt to assign literal");
                            break;
                        }
                    case VariableTypeName.tiny:
                        {
                            if (_value >= 1)
                            {
                                value = 1;
                            }
                            else
                            {
                                value = 0;
                            }
                            break;
                        }
                    case VariableTypeName.small:
                        {
                            if (isSigned)
                            {
                                if (_value >= 15)
                                {
                                    value = 15;
                                }
                                else if (_value <= -16)
                                {
                                    value = -16;
                                }
                                else
                                {
                                    value = _value;
                                }
                            }
                            else
                            {
                                if (_value >= 31)
                                {
                                    value = 31;
                                }
                                else if (_value <= 0)
                                {
                                    value = 0;
                                }
                                else
                                {
                                    value = _value;
                                }

                            }
                            break;
                        }
                    case VariableTypeName.normal:
                        {
                            if (isSigned)
                            {
                                if (_value >= 511)
                                {
                                    value = 511;
                                }
                                else if (_value <= -512)
                                {
                                    value = -512;
                                }
                                else
                                {
                                    value = _value;
                                }
                            }
                            else
                            {
                                if (_value >= 1023)
                                {
                                    value = 1023;
                                }
                                else if (_value <= 0)
                                {
                                    value = 0;
                                }
                                else
                                {
                                    value = _value;
                                }

                            }
                            break;
                        }
                    case VariableTypeName.big:
                        {
                            if (isSigned)
                            {
                                if (_value >= 16383)
                                {
                                    value = 16383;
                                }
                                else if (_value <= -16384)
                                {
                                    value = -16384;
                                }
                                else
                                {
                                    value = _value;
                                }
                            }
                            else
                            {
                                if (_value >= 32767)
                                {
                                    value = 32767;
                                }
                                else if (_value <= 0)
                                {
                                    value = 0;
                                }
                                else
                                {
                                    value = _value;
                                }

                            }
                            break;
                        }
                    default:
                        {
                            throw new Exception("Unknown type");
                            break;
                        }
                }

            }

            public static VariableType OpSum(VariableType var1, VariableType var2)
            {
                VariableType variable = new VariableType();
                if(var1.isSigned || var2.isSigned)
                {
                    variable.isSigned = true;
                }
                else
                {
                    variable.isSigned = false;
                }
                variable.TypeName = VariableTypeName.literal;
                variable.value = var1.value + var2.value;
                return variable;
            }
            public static VariableType OpSub(VariableType var1, VariableType var2)
            {
                VariableType variable = new VariableType();
                if (var1.isSigned || var2.isSigned)
                {
                    variable.isSigned = true;
                }
                else
                {
                    variable.isSigned = false;
                }
                variable.TypeName = VariableTypeName.literal;
                variable.value = var1.value - var2.value;
                return variable;
            }
            public static VariableType OpMult(VariableType var1, VariableType var2)
            {
                VariableType variable = new VariableType();
                if (var1.isSigned || var2.isSigned)
                {
                    variable.isSigned = true;
                }
                else
                {
                    variable.isSigned = false;
                }
                variable.TypeName = VariableTypeName.literal;
                variable.value = var1.value * var2.value;
                return variable;
            }
            public static VariableType OpDiv(VariableType var1, VariableType var2)
            {
                VariableType variable = new VariableType();
                if (var1.isSigned || var2.isSigned)
                {
                    variable.isSigned = true;
                }
                else
                {
                    variable.isSigned = false;
                }
                variable.TypeName = VariableTypeName.literal;
                if (var2.value == 0)
                {
                    variable.value = int.MaxValue;
                }
                else
                {
                    variable.value = var1.value / var2.value;
                }
                return variable;
            }

            public static VariableType OpUPlus(VariableType var1)
            {
                VariableType variable = new VariableType();
                variable.TypeName = VariableTypeName.literal;
                variable.isSigned = false;
                if(var1.value >=0)
                {
                    variable.value = var1.value;
                }
                else
                {
                    variable.value = -var1.value;
                }
                return variable;
            }
            public static VariableType OpUMinus(VariableType var1)
            {
                VariableType variable = new VariableType();
                variable.TypeName = VariableTypeName.literal;
                variable.isSigned = true;
                variable.value = -var1.value;
                
                return variable;
            }

            public static VariableTypeName DefineType(int number, bool isSigned)
            {
                if (!isSigned)
                {
                    if (number == 0 || number == 1)
                    {
                        return VariableTypeName.tiny;
                    }
                    else if (number <= 31)
                    {
                        return VariableTypeName.small;
                    }
                    else if (number <= 1023)
                    {
                        return VariableTypeName.normal;
                    }
                    else if (number <= 32767)
                    {
                        return VariableTypeName.big;
                    }
                    else
                    {
                        return VariableTypeName.literal;
                    }
                }
                else
                {
                    if (number >= -16 || number <= 15)
                    {
                        return VariableTypeName.small;
                    }
                    else if (number >= -512 || number <= 511)
                    {
                        return VariableTypeName.normal;
                    }
                    else if (number >= -16384 || number <= 16383)
                    {
                        return VariableTypeName.big;
                    }
                    else
                    {
                        return VariableTypeName.literal;
                    }
                }
            }
            public static VariableTypeName DefineType(ASTNode node)
            {
                switch(node.Symbol.ID)
                {
                    case BydonLexer.ID.TerminalTiny:
                        {
                            return VariableTypeName.tiny;
                        }
                    case BydonLexer.ID.TerminalSmall:
                        {
                            return VariableTypeName.small;
                        }
                    case BydonLexer.ID.TerminalNormal:
                        {
                            return VariableTypeName.normal;
                        }
                    case BydonLexer.ID.TerminalBig:
                        {
                            return VariableTypeName.big;
                        }
                    case BydonLexer.ID.TerminalField:
                        {
                            return VariableTypeName.field;
                        }
                    default:
                        {

                            throw new Exception("Unknown type");
                        }
                }
            }
            /*
            public VariableType(VariableTypeName _typeName, string number, bool _isSigned)
            {
                TypeName = _typeName;
                isSigned = _isSigned;
                SetValue(Convert32To10(number));
            }
            public VariableType(VariableTypeName _typeName, int _value, bool _isSigned)
            {
                TypeName = _typeName;
                isSigned = _isSigned;
                SetValue(_value);
            }
            public VariableType(string number)
            {

                if (number[0] == '-' || number[0] == '+')
                {
                    isSigned = true;
                }
                else
                {
                    isSigned = false;
                }
                int _value = Convert32To10(number);
                TypeName = DefineType(_value);

                SetValue(_value);
            }*/
        }

        
        public enum VariableTypeName { literal, tiny, small, normal, big, field };
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
