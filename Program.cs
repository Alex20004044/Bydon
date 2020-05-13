using System;
using Hime.Redist; // the namespace for the Hime Runtime
using Bydon; // default namespace for the parser is the grammar's name
using System.IO;
namespace TestHime
{
    class Program
    {
        static public int isAutoTest = 0;
        
        static public int testCount = 9;
        public static void Main(string[] args)
        {
            if(isAutoTest == 1)
            {
                string basePath = @"D:\Bydon\Programs\ParserTest_";
                for (int i = 0; i < testCount; i++)
                {
                    StreamReader streamReader_test = new StreamReader(basePath + i + "_IN" +".bd");

                    BydonLexer lexer_test = new BydonLexer(streamReader_test);
                    BydonParser parser_test = new BydonParser(lexer_test);
                    ParseResult result_test = parser_test.Parse();

                    streamReader_test.Close();

                    StreamWriter streamWriter = File.CreateText(basePath + i + "_OUT" + ".txt");

                    BydonInterpreter bydonInterpreter_test = new BydonInterpreter();
                    bydonInterpreter_test.Interpret(result_test, streamWriter);
                    streamWriter.Close();
                    Console.WriteLine("File " + i + " was interpretated");
                }

                int numPassedTest = 0;
                for(int i = 0; i < testCount; i++)
                {              
                    
                    StreamReader streamReader_test = new StreamReader(basePath + i + "_TEST" + ".txt");

                    string str_out_name = basePath + i + "_OUT" + ".txt";
                    StreamReader streamReader_out = new StreamReader(str_out_name);

                    int cur_str_ind = 1;
                    bool isAllCorrect = true;

                    Console.WriteLine(i + "." + " ParserTest_" + i);
                    Console.WriteLine("\t   out  test");

                    while(!(streamReader_test.EndOfStream && streamReader_out.EndOfStream))
                    {
                        string str_out = streamReader_out.ReadLine();
                        string str_test = streamReader_test.ReadLine();
                        if (str_out != str_test)
                        {
                            if (isAllCorrect)
                            {
                                isAllCorrect = false;
                            }
                            Console.Write("\t" + cur_str_ind + ") " + str_out + " != " + str_test);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" - Error");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write("\t" + cur_str_ind + ") " + str_out + " == " + str_test);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(" - Correct");
                            Console.ResetColor();
                        }
                        cur_str_ind++;
                    }
                    if(isAllCorrect)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("FILE Correct");
                        Console.ResetColor();
                        numPassedTest++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("FILE Error");
                        Console.ResetColor();
                        
                    }
                    Console.WriteLine("---------");

                    streamReader_test.Close();
                    streamReader_out.Close();
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Test Results: " + numPassedTest + "/" + testCount);
                Console.ResetColor();
                Console.WriteLine("---------");
            }

            Console.WriteLine("\n\n\n");

            string path = @"D:\Bydon\Programs\ParserDebug_1.bd";
            StreamReader streamReader = new StreamReader(path);
            // Creates the lexer and parser 

            BydonLexer lexer = new BydonLexer(streamReader);
            BydonParser parser = new BydonParser(lexer);
            // Executes the parsing
            ParseResult result = parser.Parse();
            streamReader.Close();


            BydonInterpreter bydonInterpreter = new BydonInterpreter();
            BydonInterpreter.RobotMaze robotMaze = new BydonInterpreter.RobotMaze();

            robotMaze.Initialize(2);
            //bydonInterpreter.Interpret(result, null, robotMaze, true);
            bydonInterpreter.Interpret(result, null, null, true);
        }

        
    }
}