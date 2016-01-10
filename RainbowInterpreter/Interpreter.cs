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
        private byte[] tape;
        public Interpreter(string[] statements, int cellCount)
        {
            this.statements = statements;
            tape = new byte[cellCount]; //create a tape with 2048 8-bit memory cells
        }

        public ExitStatus Execute()
        {
            for (currentStatement = 0; currentStatement < statements.Length; currentStatement++) //basic interpreter loop
            {
                string statement = statements[currentStatement];

                Instruction instr = (Instruction)Convert.ToInt32(statement.Substring(0,1), 16);
                int addr = Convert.ToInt32(statement.Substring(1, 2), 16);
                ValuePart val = getValuePart(statement);

                Console.WriteLine("{0} {1} {2}", instr, addr, val);

            }

            return ExitStatus.OK;
        }

        private ValuePart getValuePart(string statement)
        {
            int val = Convert.ToInt32(statement.Substring(4, 2), 16);
            int addr = 0;

            if (statement[3] == '1') //if address switch is true
            {
                addr = val;
                val = tape[addr];
            }

            return new ValuePart(val, addr);
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

    struct ValuePart
    {
        public int Value;
        public int Address;

        public ValuePart(int value, int address)
        {
            Value = value;
            Address = address;
        }

        public override string ToString()
        {
            return "Value: " + Value + " Address: " + Address;
        }
    }
}
