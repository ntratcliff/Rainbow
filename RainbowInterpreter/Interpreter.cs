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

                //Console.WriteLine("{0} {1} {2}", instr, addr, val);

                switch (instr)
                {
                    case Instruction.Exit: return exit(val);
                    case Instruction.Set: set(addr, val);
                        break;
                    case Instruction.Print: print(addr, val);
                        break;
                    case Instruction.In: input(addr, val);
                        break;
                    case Instruction.Label:
                        break;
                    case Instruction.Lookback:
                        break;
                    case Instruction.Lookahead:
                        break;
                    case Instruction.Add:
                        break;
                    case Instruction.Sub:
                        break;
                    case Instruction.Mul:
                        break;
                    case Instruction.Div:
                        break;
                    case Instruction.Mod:
                        break;
                }                  
            }

            return ExitStatus.OK;
        }

        private ExitStatus exit(ValuePart val) //attempt to return known exit status by value, else return unknown
        {
            ExitStatus status = ExitStatus.Unknown;
            if (Enum.IsDefined(typeof(ExitStatus), val.Value))
                status = (ExitStatus)val.Value;

            return status;
        }

        private void set(int addr, ValuePart val) //set cell at address on tape to value
        {
            tape[addr] = (byte)val.Value;
        }

        private void print(int addr, ValuePart val) //print each cell from address to address
        {
            for(int i = addr; i <= val.Address; i++)
            {
                Console.Write((char)tape[i]);
            }
        }

        private void input(int addr, ValuePart val) //get text input starting at addr, setting val cell to input end address
        {
            string input = Console.ReadLine();
            int i;
            for (i = 0; i < input.Length; i++)
            {
                tape[i + addr] = (byte)input[i];
            }

            tape[val.Address] = (byte)(i + addr);
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
        Label = 5,
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
        InterpreterException = 3,
        Unknown = 16
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
