using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Hime.Redist;
using Bydon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestHime
{
    public class BydonInterpreter
    {
        Dictionary<string, ASTNode> gloabalFuncTable = new Dictionary<string, ASTNode>();
        Stack<Dictionary<string, VariableType>> functionStack = new Stack<Dictionary<string, VariableType>>();
        string statemenListCallCountName = "StatementListCallCount";
        bool isReturn = false;

        int errorCounter = 1;
        TextWriter printOutput;
        RobotMaze robotMaze;

        const string robotReachExit = "ROBOT_REACH_EXIT";

        public void Interpret(ParseResult parseResult, TextWriter _printOutput = null, RobotMaze _robotMaze = null, bool isDisplayTree = false)
        {
            if (parseResult.Errors.Count == 0)
            {
                if (isDisplayTree)
                {
                    Print(parseResult.Root, new bool[] { });
                }
                printOutput = _printOutput;

                if (_robotMaze == null)
                {
                    robotMaze = new RobotMaze();
                }
                else
                {
                    robotMaze = _robotMaze;
                }

                VariableType result = null;

                try
                {
                    result = Process(parseResult.Root);
                }
                catch (Exception e)
                {
                    switch (e.Message)
                    {
                        case robotReachExit:
                            {
                                Console.WriteLine("Robot has reached exit");
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Error. Can't interpet program further. " + e.Message);
                                break;
                            }
                    }

                }
                if (errorCounter == 1)
                {
                    Console.WriteLine("Succesful compilation");
                }
                else
                {
                    Console.WriteLine("Error compilation");
                }
                if (result == null)
                {
                    Console.WriteLine("Result is undefined");
                }
                else
                {
                    Console.WriteLine("Result " + result.GetValue());
                }
            }
            else
            {
                Console.WriteLine("Error compilation on parsing step.");
                Console.WriteLine("---");
                int index = 1;
                foreach (var x in parseResult.Errors)
                {
                    Console.Write(index + ")" + x.Type + " | " + x.Message + " | (position: " + x.Position + ")\n");
                    index++;
                }
            }
            Console.WriteLine("\n\n\n");
        }
        VariableType Process(ASTNode node)
        {
            if (node.Symbol.ID == BydonLexer.ID.TerminalNumber)
            {
                return VariableType.CreateLiteral(node.Value);
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalVariable)
            {
                VariableType variableType;
                if (functionStack.Peek().TryGetValue(node.Value, out variableType))
                {
                    return variableType;
                }
                else
                {
                    //Error
                    PrintError("Variable " + node.Value + " is undefined", node);
                    return VariableType.DefaultVariableTypeValue();
                }
            }
            else if (node.Symbol.ID == BydonParser.ID.VariableIndexator)
            {
                VariableType variableType = Process(node.Children[0]);

                if (variableType.TypeName == VariableTypeName.field)
                {
                    FieldType fieldType = variableType as FieldType;
                    VariableType variable = fieldType.GetArrayVariable(Process(node.Children[1]).GetValue(), Process(node.Children[2]).GetValue());
                    if (variable != null)
                    {
                        return variable;
                    }
                    else
                    {
                        PrintError("Indexes is not correct", node.Children[0]);
                        return VariableType.DefaultVariableTypeValue();
                    }
                }
                else
                {
                    PrintError("Variable " + node.Children[0].Value + " is not support indexator", node.Children[0]);
                    return VariableType.DefaultVariableTypeValue();
                }
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalReturn)
            {
                VariableType variableType;
                functionStack.Peek().TryGetValue("return", out variableType);
                variableType.CopyVariable(Process(node.Children[0]));
                isReturn = true;
                return null;
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalPrint)
            {
                VariableType exp = Process(node.Children[0]);
                if (exp != null)
                {
                    PrintFunction(exp.GetValue().ToString());
                }
                else
                {
                    PrintFunction("Expression can't be processed " + node.Position);
                }
                return null;
            }

            else if (node.Symbol.ID == BydonParser.ID.VariableExpAssignRight)
            {
                if (node.Children[1].Symbol.ID == BydonParser.ID.VariableIndexator)
                {
                    VariableType variableType;
                    if (functionStack.Peek().TryGetValue(node.Children[1].Children[0].Value, out variableType))
                    {
                        variableType = Process(node.Children[1]);
                        variableType.SetValue(Process(node.Children[0]).GetValue());
                        return variableType;
                    }
                    else
                    {
                        PrintError("Variable " + node.Children[1].Children[0].Value + " is undefined", node.Children[1].Children[0]);
                        return VariableType.DefaultVariableTypeValue();
                    }
                }
                else
                {
                    VariableType variableType;
                    if (functionStack.Peek().TryGetValue(node.Children[1].Value, out variableType))
                    {
                        variableType.CopyVariable(Process(node.Children[0]));
                        return variableType;
                    }
                    else
                    {
                        PrintError("Variable " + node.Children[1].Value + " is undefined", node.Children[1]);
                        return VariableType.DefaultVariableTypeValue();
                    }
                }
            }
            else if (node.Symbol.ID == BydonParser.ID.VariableExpAssignLeft)
            {
                if (node.Children[0].Symbol.ID == BydonParser.ID.VariableIndexator)
                {
                    VariableType variableType;
                    if (functionStack.Peek().TryGetValue(node.Children[0].Children[0].Value, out variableType))
                    {
                        variableType = Process(node.Children[0]);
                        variableType.SetValue(Process(node.Children[1]).GetValue());
                        return variableType;
                    }
                    else
                    {
                        PrintError("Variable " + node.Children[0].Children[0].Value + " is undefined", node.Children[0].Children[0]);
                        return VariableType.DefaultVariableTypeValue();
                    }
                }
                else
                {
                    VariableType variableType;
                    if (functionStack.Peek().TryGetValue(node.Children[0].Value, out variableType))
                    {
                        variableType.CopyVariable(Process(node.Children[1]));
                        return variableType;
                    }
                    else
                    {
                        PrintError("Variable " + node.Children[0].Value + " is undefined", node.Children[0]);
                        return VariableType.DefaultVariableTypeValue();
                    }
                }
            }

            //Variable construction
            else if (node.Symbol.ID == BydonParser.ID.VariableVariableInit)
            {
                VariableType exp = Process(node.Children[node.Children.Count - 1]);
                for (int i = 1; i < node.Children.Count - 1; i++)
                {
                    VariableType variableType = VariableType.CreateVariable(DefineVariableTypeName(node.Children[0]), exp);
                    string varName = node.Children[i].Value;

                    if (functionStack.Peek().ContainsKey(varName))
                    {
                        //Error
                        PrintError(varName + " variable already exists. Second initialization is canceled", node.Children[i]);
                    }
                    else
                    {
                        functionStack.Peek().Add(varName, variableType);
                    }
                }
                return exp;
            }
            else if (node.Symbol.ID == BydonParser.ID.VariableFieldInit)
            {
                VariableType exp = Process(node.Children[node.Children.Count - 1]);
                for (int i = 2; i < node.Children.Count - 1; i++)
                {
                    FieldType fieldType = FieldType.CreateFieldVariable(DefineVariableTypeName(node.Children[0]),
                        DefineVariableTypeName(node.Children[1]), exp);
                    string varName = node.Children[i].Value;

                    if (functionStack.Peek().ContainsKey(varName))
                    {
                        //Error
                        PrintError(varName + " variable already exists. Second initialization is canceled", node.Children[i]);
                    }
                    else
                    {
                        functionStack.Peek().Add(varName, fieldType);
                    }
                }
                return exp;
            }


            //Function process
            else if (node.Symbol.ID == BydonParser.ID.VariableFunctionCall)
            {
                ASTNode func;
                if (gloabalFuncTable.TryGetValue(node.Children[0].Value, out func))
                {
                    Process(node.Children[1]);
                    return Process(func);
                }
                else
                {
                    PrintError(node.Children[0].Value + " is undefined function", node);
                    return VariableType.DefaultVariableTypeValue();
                }
            }
            //  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Create new stack and add default variables
            else if (node.Symbol.ID == BydonParser.ID.VariableFunctionCallArgs)
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
            else if (node.Symbol.ID == BydonParser.ID.VariableFunctionDefineVariable)
            {
                //attention
                
                VariableType returnVariable = VariableType.CreateVariable(DefineVariableTypeName(node.Children[0]), Process(node.Children[3]));
                functionStack.Peek().Add("return", returnVariable);
                VariableType statementListCallCount = VariableType.CreateVariable(VariableTypeName.big, VariableType.CreateLiteral("0"));
                functionStack.Peek().Add(statemenListCallCountName, statementListCallCount);

                Process(node.Children[2]);
                Process(node.Children[4]);

                return functionStack.Pop()["return"];
            }
            else if (node.Symbol.ID == BydonParser.ID.VariableFunctionDefineField)
            {
                //attention

                VariableType returnVariable = FieldType.CreateFieldVariable(DefineVariableTypeName(node.Children[0]),
                    DefineVariableTypeName(node.Children[1]), Process(node.Children[4]));
                functionStack.Peek().Add("return", returnVariable);
                VariableType statementListCallCount = VariableType.CreateVariable(VariableTypeName.big, VariableType.CreateLiteral("0"));
                functionStack.Peek().Add(statemenListCallCountName, statementListCallCount);

                Process(node.Children[3]);
                Process(node.Children[5]);

                return functionStack.Pop()["return"];
            }

            else if (node.Symbol.ID == BydonParser.ID.VariableFunctionParametres)
            {
                Dictionary<string, VariableType> varTable = functionStack.Peek();
                VariableType variableType;
                VariableType stackVar;
                for (int i = 0; i < node.Children.Count; i++)
                {
                    Process(node.Children[i]);
                    if (node.Children[i].Symbol.ID == BydonParser.ID.VariableVariableInit)
                    {
                        varTable.TryGetValue(node.Children[i].Children[1].Value, out variableType);
                    }
                    else
                    {
                        varTable.TryGetValue(node.Children[i].Children[2].Value, out variableType);
                    }

                    if (varTable.TryGetValue((i).ToString(), out stackVar))
                    {
                        variableType.CopyVariable(stackVar);
                        varTable.Remove((i).ToString());
                    }
                }
                int parametresCount = node.Children.Count;
                while (varTable.TryGetValue(parametresCount.ToString(), out variableType))
                {
                    varTable.Remove(parametresCount.ToString());
                    parametresCount++;
                    PrintError("Odd argument in function call: " + variableType.GetValue(), ((ASTNode)node.Parent).Children[1]);
                }
                return null;
            }
            else if (node.Symbol.ID == BydonParser.ID.VariableCheck)
            {
                VariableType condition = Process(node.Children[0]);
                if (condition.GetValue() != 0)
                {
                    Process(node.Children[1]);
                }
                return null;
            }
            else if (node.Symbol.ID == BydonParser.ID.VariableUntil)
            {
                while (VariableType.CreateVariable(VariableTypeName.tiny, Process(node.Children[0])).GetValue() != 0)
                {
                    Process(node.Children[1]);
                }
                return null;
            }
            //Statement list process 
            else if (node.Symbol.ID == BydonParser.ID.VariableStatementList)
            {
                VariableType statementListCallCount;
                functionStack.Peek().TryGetValue(statemenListCallCountName, out statementListCallCount);
                statementListCallCount.SetValue(statementListCallCount.GetValue() + 1);
                for (int i = 0; i < node.Children.Count; i++)
                {
                    VariableType value = Process(node.Children[i]);
                    if (isReturn)
                    {
                        statementListCallCount.SetValue(statementListCallCount.GetValue() - 1);
                        if (statementListCallCount.GetValue() == 0)
                        {
                            isReturn = false;
                        }
                        return value;
                    }
                }
                statementListCallCount.SetValue(statementListCallCount.GetValue() - 1);
                return null;
            }
            //Attention

            else if (node.Symbol.ID == BydonParser.ID.VariableProgram)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    InitFunc(node.Children[i]);
                }
                ASTNode funcNode;
                if (gloabalFuncTable.TryGetValue("main", out funcNode))
                {
                    functionStack.Push(new Dictionary<string, VariableType>());
                    VariableType t = Process(funcNode);
                    return t;
                }
                else
                {
                    PrintError("Main function is not found", node);
                    return null;
                }
            }

            else if (node.Symbol.ID == BydonLexer.ID.TerminalGo)
            {
                return robotMaze.RobotGo();
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalRr)
            {
                return robotMaze.RobotRR();
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalRl)
            {
                return robotMaze.RobotRL();
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalCompass)
            {
                return robotMaze.RobotCompass();
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalSonar)
            {
                return robotMaze.RobotSonar();
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
            if (value[0] == '-' || value[0] == '+')
            {
                i = 1;
            }
            else
            {
                i = 0;
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
            string funcName;
            if (node.Children.Count == 5)
            {
                funcName = node.Children[1].Value;
            }
            else
            {
                funcName = node.Children[2].Value;
            }
            
            if (gloabalFuncTable.ContainsKey(funcName))
            {
                PrintError(funcName + " function already exists. Second initializtion is canceled.", node);
            }
            else
            {
                gloabalFuncTable.Add(funcName, node);
            }
        }
        VariableTypeName DefineVariableTypeName(ASTNode node)
        {
            if (node.Symbol.ID == BydonLexer.ID.TerminalTiny)
            {
                return VariableTypeName.tiny;
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalSmall)
            {
                return VariableTypeName.small;
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalNormal)
            {
                return VariableTypeName.normal;
            }
            else if (node.Symbol.ID == BydonLexer.ID.TerminalBig)
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

        public class VariableType
        {
            public VariableTypeName TypeName { get; protected set; }
            bool isSigned;
            int value;
            public VariableType()
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
            public virtual int GetValue()
            {
                return value;
            }
            public virtual void CopyVariable(VariableType variableC)
            {
                SetValue(variableC.value);
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
                            throw new Exception("Undefined for this type actions ");
                            break;
                        }
                }

            }

            public static VariableType OpSum(VariableType var1, VariableType var2)
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
                if (var1.value >= 0)
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
                switch (node.Symbol.ID)
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

            public static VariableType DefaultVariableTypeValue()
            {
                return CreateLiteral(0);
            }
        }

        public class FieldType : VariableType
        {
            VariableTypeName dimensionType;
            VariableType[,] variables;
            int elementsCount;
            public VariableType GetArrayVariable(int x, int y)
            {
                if (x < elementsCount && y < elementsCount && x >= 0 && y >= 0)
                {
                    return variables[y, x];
                }
                else
                {
                    return null;
                }
            }

            public static FieldType CreateFieldVariable(VariableTypeName _typeName, VariableTypeName _dimensionType, VariableType number)
            {
                FieldType fieldType = new FieldType();
                fieldType.TypeName = VariableTypeName.field;
                fieldType.dimensionType = _dimensionType;
                fieldType.elementsCount = DefineElementsCount(_dimensionType);
                fieldType.variables = new VariableType[fieldType.elementsCount, fieldType.elementsCount];
                for (int i = 0; i < fieldType.elementsCount; i++)
                {
                    for (int j = 0; j < fieldType.elementsCount; j++)
                    {
                        VariableType variableType = VariableType.CreateVariable(_typeName, number);
                        fieldType.variables[i, j] = variableType;
                    }
                }
                return fieldType;
            }
            static int DefineElementsCount(VariableTypeName _dimensionType)
            {
                int elementsCount = 0;
                switch (_dimensionType)
                {
                    case VariableTypeName.tiny:
                        {
                            elementsCount = 2;
                            break;
                        }
                    case VariableTypeName.small:
                        {
                            elementsCount = 32;
                            break;
                        }
                    case VariableTypeName.normal:
                        {
                            elementsCount = 1024;
                            break;
                        }
                    case VariableTypeName.big:
                        {
                            elementsCount = 32768;
                            break;
                        }
                }
                return elementsCount;
            }
            public override void CopyVariable(VariableType variableC)
            {
                if (TypeName == VariableTypeName.field && variableC.TypeName == VariableTypeName.field)
                {
                    FieldType field = this as FieldType;
                    FieldType fieldC = variableC as FieldType;
                    for (int i = 0; i < field.elementsCount && i < fieldC.elementsCount; i++)
                    {
                        for (int j = 0; j < field.elementsCount && j < fieldC.elementsCount; j++)
                        {
                            field.GetArrayVariable(i, j).SetValue(fieldC.GetArrayVariable(i, j).GetValue());
                        }
                    }
                }
                else
                {
                    throw new Exception("Undefined for this type actions ");
                }
            }
            public override int GetValue()
            {
                throw new Exception("Undefined for this type actions ");
                return -1;
            }
        }
        public enum VariableTypeName { literal, tiny, small, normal, big, field };
        void PrintError(string message, ASTNode node)
        {
            Console.WriteLine(errorCounter + ")" + message + " | (Position: " + node.Position + ")");
            errorCounter++;
        }
        public class RobotMaze
        {
            public int mapNumber = 0;
            string filesPath = @"D:\Bydon\Maps\";
            string mapName = @"_map.json";
            string robotName = @"_robot.json";
            string commandsName = @"_commands.txt";
            public Dictionary<Vector2, CellType> map { get; private set; }
            public Vector2 exitCell;

            public Vector2 RobotPos
            {
                get
                {
                    return robotPos;
                }
                set
                {
                    sonarCallCount = 0;
                    previousSonarBits = 0;
                    robotPos = value;
                }
            }
            Vector2 robotPos;
            public int RobotRot
            {
                get
                {
                    return robotRot;
                }
                set
                {
                    sonarCallCount = 0;
                    previousSonarBits = 0;
                    robotRot = value;
                }
            }

            int robotRot;

            int sonarCallCount = 0;
            int previousSonarBits = 0;

            bool isInitialize = false;
            public class Cell
            {
                public int X { get; set; }
                public int Y { get; set; }
                public CellType CellType { get; set; }
            }

            public void Initialize(int _mapNumber)
            {
                mapNumber = _mapNumber;
                List<Cell> cells = JsonConvert.DeserializeObject<List<Cell>>(File.ReadAllText(filesPath + mapNumber + mapName));
                map = new Dictionary<Vector2, CellType>();
                foreach (Cell c in cells)
                {
                    if(c.CellType == CellType.exit)
                    {
                        exitCell = new Vector2(c.X, c.Y);
                    }
                    map.Add(new Vector2(c.X, c.Y), c.CellType);
                }
                JObject jObject = JObject.Parse(File.ReadAllText(filesPath + mapNumber + robotName));
                Vector2 pos = new Vector2();
                pos.x = jObject.SelectToken("x").Value<int>();
                pos.y = jObject.SelectToken("y").Value<int>();
                RobotPos = pos;
                RobotRot = jObject.SelectToken("rotation").Value<int>();

                File.CreateText(filesPath + mapNumber + commandsName).Close();
                isInitialize = true;
            }
            public BydonInterpreter.VariableType RobotRR()
            {
                if (isInitialize)
                {
                    //Do
                    RobotRot = (RobotRot + 1) % 6;
                    File.AppendAllText(filesPath + mapNumber + commandsName, "r");
                    return VariableType.CreateLiteral(1);
                }
                else
                {
                    return VariableType.CreateLiteral(0);
                }
            }
            public BydonInterpreter.VariableType RobotRL()
            {
                if (isInitialize)
                {
                    //Do
                    RobotRot = (RobotRot + 5) % 6;
                    File.AppendAllText(filesPath + mapNumber + commandsName, "l");
                    return VariableType.CreateLiteral(1);
                }
                else
                {
                    return VariableType.CreateLiteral(0);
                }
            }
            public BydonInterpreter.VariableType RobotGo()
            {
                if (isInitialize)
                {
                    //Do
                    Vector2 newPos = RobotMaze.MapDisplacement(RobotPos, RobotRot);
                    switch (GetCellTypeOnNextStep(RobotPos, RobotRot))
                    {
                        case CellType.empty:
                            {
                                RobotPos = newPos;
                                File.AppendAllText(filesPath + mapNumber + commandsName, "g");
                                return VariableType.CreateLiteral(1);

                                break;

                            }
                        case CellType.exit:
                            {
                                RobotPos = newPos;
                                File.AppendAllText(filesPath + mapNumber + commandsName, "g");
                                //ROBOT WIN!!!!!!!!!!!!!
                                throw new Exception(robotReachExit);

                                return VariableType.CreateLiteral(int.MaxValue);

                            }
                        case CellType.wall:
                            {
                                return VariableType.CreateLiteral(0);
                            }
                    }
                    return VariableType.CreateLiteral(int.MinValue);
                }
                else
                {
                    return VariableType.CreateLiteral(0);
                }
            }
            
            public BydonInterpreter.VariableType RobotSonar()
            {
                // Bitmap: forward = 0, lf = 1, rf = 2, lb = 3, rb = 4
                if (isInitialize)
                {
                    int sonarBits = 0;
                    if (previousSonarBits % 2 == 1 || GetCellTypeInDirectionStep(RobotPos, RobotRot, sonarCallCount) == CellType.wall)
                    {
                        sonarBits = 1;
                    }
                    if (((previousSonarBits >> 1) % 2 ) == 1 || GetCellTypeInDirectionStep(RobotPos, (RobotRot + 5) % 6, sonarCallCount) == CellType.wall)
                    {
                        sonarBits += 2;
                    }
                    if (((previousSonarBits >> 2) % 2) == 1 || GetCellTypeInDirectionStep(RobotPos, (RobotRot + 1) % 6, sonarCallCount) == CellType.wall)
                    {
                        sonarBits += 4;
                    }
                    if (((previousSonarBits >> 3) % 2) == 1 || GetCellTypeInDirectionStep(RobotPos, (RobotRot + 4) % 6, sonarCallCount) == CellType.wall)
                    {
                        sonarBits += 8;
                    }
                    if (((previousSonarBits >> 4) % 2) == 1 || GetCellTypeInDirectionStep(RobotPos, (RobotRot + 2) % 6, sonarCallCount) == CellType.wall)
                    {
                        sonarBits += 16;
                    }
                    previousSonarBits = sonarBits;
                    return VariableType.CreateLiteral(sonarBits);
                }
                else
                {
                    previousSonarBits = 0;
                    return VariableType.CreateLiteral(0);
                }

            }
            CellType GetCellTypeInDirectionStep(Vector2 position, int directon, int stepCount)
            {
                for(int i = 0; i <= stepCount ; i++)
                {
                    position = MapDisplacement(position, directon);
                }
                CellType cellType;
                if (map.TryGetValue(position, out cellType))
                {
                    return cellType;
                }
                else
                {
                    return CellType.wall;
                }
         
            }
            public BydonInterpreter.VariableType RobotCompass()
            {
                if (isInitialize)
                {
                    //Do
                    Vector2 dirToExit = new Vector2();
                    dirToExit.x = exitCell.x - RobotPos.x;
                    dirToExit.y = exitCell.y - RobotPos.y;

                    float xCoord = (float)((exitCell.x - RobotPos.x) * Math.Cos(30 * Math.PI * 2 / 360));
                    float yCoord = (float)((exitCell.y + exitCell.x * 0.495f) - (RobotPos.y + RobotPos.x * 0.495f));

                    xCoord = (float) (xCoord / (Math.Sqrt(xCoord * xCoord + yCoord * yCoord)));
                    yCoord = (float)(yCoord / (Math.Sqrt(xCoord * xCoord + yCoord * yCoord)));
                    float angle = 0;
                    float atg = (float) (Math.Atan2(yCoord, xCoord) * 360 / (Math.PI * 2));

                    if (xCoord == 0)
                    {
                        if (yCoord == 1)
                        {
                            angle = 90f;
                        }
                        else if (yCoord == -1)
                        {
                            angle = 270f;
                        }
                    }
                    else
                    {
                        if (yCoord > 0)
                        {
                            angle = atg;
                        }
                        else
                        {
                            angle = atg + 360f;
                        }
                    }
                    return VariableType.CreateLiteral((int)(angle * 60));
                }
                else
                {
                    return VariableType.CreateLiteral(0);
                }

            }
            [Serializable]
            public enum CellType { empty, wall, exit };
            public static Vector2 MapDisplacement(Vector2 position, int direction)
            {
                Vector2 disp = new Vector2();

                if (direction == 0)
                {
                    disp.y = position.y + 1;
                    disp.x = position.x;
                }
                else if (direction == 1)
                {
                    disp.y = position.y;
                    disp.x = position.x + 1;
                }
                else if (direction == 2)
                {
                    disp.y = position.y - 1;
                    disp.x = position.x + 1;
                }
                else if (direction == 3)
                {
                    disp.y = position.y - 1;
                    disp.x = position.x;
                }
                else if (direction == 4)
                {
                    disp.y = position.y;
                    disp.x = position.x - 1;
                }
                else if (direction == 5)
                {
                    disp.y = position.y + 1;
                    disp.x = position.x - 1;
                }
                return disp;
            }
            [Serializable]
            public struct Vector2
            {
                
                public int x;
                public int y;
                public Vector2(int _x, int _y)
                {
                    x = _x;
                    y = _y;
                }
                public override string ToString()
                {
                    return '(' + x.ToString() + ' ' + y.ToString() + ')';
                }
            }
            CellType GetCellTypeOnNextStep(Vector2 currentPos, int rot)
            {
                Vector2 pos = MapDisplacement(currentPos, rot);
                CellType cellType;
                if(map.TryGetValue(pos, out cellType))
                {
                    return cellType;
                }
                else
                {
                    return CellType.wall;
                }
            }
        }
    }
}
