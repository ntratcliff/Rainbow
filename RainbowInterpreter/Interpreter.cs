using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowInterpreter
{
    class Interpreter
    {
        private string[] statements;
        private int currentStatement;
        public Interpreter(string[] ins)
        {
            statements = ins;
        }

        public ExitStatus Execute()
        {
            for (currentStatement = 0; currentStatement < statements.Length; currentStatement++) //basic interpreter loop
            {
                string statement = statements[currentStatement];
                Instruction inst = (Instruction)Convert.ToInt32(statement.Substring(0,1), 16);
                Console.WriteLine(inst);
           
                
            }

            return ExitStatus.OK;
        }


    }

    enum Instruction
    {
        Exit = 0,
        Set = 1,
        Print = 2,
        In = 3,
        Flag = 5,
        Lookback = 6,
        Lookahead = 7,
        Add = 10,
        Sub = 11,
        Mul = 12,
        Div = 13,
        Mod = 14
    }

    enum ExitStatus
    {
        OK = 0,
        ProgramException = 1,
        RainbowException = 2,
        InterpreterException = 3
    }
}
