﻿using System;
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
        private OutputMode outputMode;

        public Interpreter(string[] statements, OutputMode m)
        {
            this.statements = statements;
            outputMode = m;
            tape = new byte[256]; //create a tape with 256 8-bit memory cells
        }

        public ExitStatus Execute()
        {
            for (currentStatement = 0; currentStatement < statements.Length; currentStatement++) //basic interpreter loop
            {
                string statement = statements[currentStatement];

                int iinstr = Convert.ToInt32(statement.Substring(0, 1), 16);

                //check if Instruction enum has statement instruction
                if (!Enum.IsDefined(typeof(Instruction), iinstr))
                {
                    throw new ExitException("Invalid instruction in statement " + statement, ExitStatus.RainbowException);
                }

                Instruction instr = (Instruction)iinstr;
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
                    case Instruction.Lookback: lookback(val);
                        break;
                    case Instruction.Lookahead: lookahead(val);
                        break;
                    case Instruction.Add: add(addr, val);
                        break;
                    case Instruction.Sub: sub(addr, val);
                        break;
                    case Instruction.Mul: mul(addr, val);
                        break;
                    case Instruction.Div: div(addr, val);
                        break;
                    case Instruction.Mod: mod(addr, val);
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
            switch (outputMode)
            {
                case OutputMode.ASCII:
                    for (int i = addr; i <= val.Address; i++)
                    {
                        Console.Write((char)tape[i]);
                    }
                    break;
                case OutputMode.Decimal:
                    for (int i = addr; i <= val.Address; i++)
                    {
                        if (i > addr)
                            Console.Write("-");
                        Console.Write(tape[i]);
                    }
                    break;
                case OutputMode.Hex:
                    for (int i = addr; i <= val.Address; i++)
                    {
                        if (i > addr)
                            Console.Write("-");
                        Console.Write(tape[i].ToString("X2"));
                    }
                    break;
            }
            
        }

        private void input(int addr, ValuePart val) //get text input starting at addr, setting val cell to input end address
        {
            string input = Console.ReadLine();

            int num;
            if (int.TryParse(input, out num) && num < 256) //if input is just a number less than 256, set addr to that number
            {
                tape[addr] = (byte)num;
                tape[val.Address] = 0x01;
                return;
            }

            int i;
            for (i = 0; i < input.Length; i++)
            {
                tape[i + addr] = (byte)input[i];
            }

            tape[val.Address] = (byte)(i + addr);
        }

        private void lookback(ValuePart val) //looks back and begins execution at a flag with the same value (greedy)
        {
            for (; currentStatement > 0; currentStatement--)
            {
                if ((Instruction)Convert.ToInt32(statements[currentStatement].Substring(0, 1), 16) == Instruction.Label
                    && getValuePart(statements[currentStatement]).Value == val.Value) //if statement is a label and value part is equal to lookback value
                    return;
            }
        }

        private void lookahead(ValuePart val) //looks forward and begins execution at a flag with the same value (greedy)
        {
            for (; currentStatement < statements.Length; currentStatement++)
            {
                if ((Instruction)Convert.ToInt32(statements[currentStatement].Substring(0, 1), 16) == Instruction.Label
                    && getValuePart(statements[currentStatement]).Value == val.Value) //if statement is a label and value part is equal to lookahead value
                    return;
            }
        }

        private void add(int addr, ValuePart val) //adds val to the value at given address on the tape
        {
            tape[addr] += (byte)val.Value;
        }

        private void sub(int addr, ValuePart val) //subtracts val from the value at given address on the tape
        {
            tape[addr] -= (byte)val.Value;
        }

        private void mul(int addr, ValuePart val) //multiplies the value at the given address on the tape by val
        {
            tape[addr] *= (byte)val.Value;
        }

        private void div(int addr, ValuePart val) //divides the value at the given address on the tape by val
        {
            tape[addr] /= (byte)val.Value;
        }

        private void mod(int addr, ValuePart val) //sets the value at the given address on the tape to the mod of that value and val
        {
            tape[addr] %= (byte)val.Value;
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

    public enum ExitStatus
    {
        OK = 0,
        ProgramException = 1,
        RainbowException = 2,
        InternalException = 3,
        Unknown = 16
    }

    public enum OutputMode
    {
        ASCII,
        Hex,
        Decimal
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

    [Serializable]
    public class ExitException : Exception
    {
        public ExitStatus ExitStatus;
        public ExitException()
        {
            ExitStatus = ExitStatus.Unknown;
        }
        public ExitException(string message)
            : base(message)
        {
            ExitStatus = ExitStatus.Unknown;
        }

        public ExitException(string message, ExitStatus status)
            : base(message)
        {
            ExitStatus = status;
        }
        public ExitException(string message, ExitStatus status, Exception inner)
            : base(message, inner)
        {
            ExitStatus = status;
        }
        protected ExitException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
