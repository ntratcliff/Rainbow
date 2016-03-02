# Rainbow
Rainbow is an esoteric programming language that runs on a virtual machine which takes instructions in the form of 3 byte hexadecimal strings from the RGB values of pixels in a bitmap image.

# The Language
Rainbow is a language in which 6 character, 3 byte hex strings are encoded into pixel data of an image, providing instructions for the Rainbow VM. 

Each hex string is referred to as a statement, and each statement is made up of 4 parts. The first part of a statement is the instruction to be executed on the Rainbow VM, and is defined by the first character of each statement. The second part of a statement is one of two 1 byte, 2 character parameters passed to the Rainbow VM with the instruction and is always a memory address. The third part of a statement is a single character switch indicating whether the last part of the statement is a value or memory address. This switch always has the value of either `1` or `0`. The last two characters of a statement make up the second 1 byte parameter, a value or memory address depending on the state of the previous switch. If the executing instruction calls for a value as the second parameter, but a memory address is provided, the Rainbow VM will use the value of the cell at the address in execution.

For example: the statement `0xA05031` would execute the instruction at `A` with the address `0x05` and the value `0x31` as parameters. Additionally, the statement `0xA05131` would execute the instruction at `A` with the address `0x05` and the value of the cell at address `0x31` as parameters, due to the value/address switch being set to `1`.

This is an example of a simple "Hello World!" program in Rainbow:
![Hello World](https://i.imgur.com/UbOCjLl.png)

This program is interpreted by the Rainbow VM as the following set of instructions (comments excluded):
```
0x100048  ;set cell 0x00 to value 0x48 (H)
0x101045  ;set cell 0x01 to value 0x45 (E)
0x10204C  ;set cell 0x02 to value 0x4C (L)
0x10304C  ;set cell 0x03 to value 0x4C (L)
0x10404F  ;set cell 0x04 to value 0x4F (O)
0x105020  ;set cell 0x05 to value 0x20 ( )
0x106057  ;set cell 0x06 to value 0x57 (W)
0x10704F  ;set cell 0x07 to value 0x4F (O)
0x108052  ;set cell 0x08 to value 0x52 (R)
0x10904C  ;set cell 0x09 to value 0x4C (L)
0x10A044  ;set cell 0x0A to value 0x44 (D)
0x10B021  ;set cell 0x0B to value 0x21 (!)
0x20010B  ;print values from cell 0x00 to cell 0x0B 
0x000000  ;exit with status code 0x00
```

**Note:** Rainbow is read pixel by pixel, left to right, top to bottom. Image width/height have no effect on execution if the number of pixels is the same, and they are still read in the same order.

# The Rainbow VM
The Rainbow VM currently has a set of 12 instructions, with capacity for a maximum of 16 instructions. These instructions are identified by the first character of each hex string passed to the VM, and are executed on an 256-cell tape with 8-bit memory cells.

For example: `0x10204C` would result in the VM executing the `set` command, which would set the memory cell at address `0x02` to the value of `0x4C`.

### VM Instructions

|First Character|Instruction|Address|Value  |Description                |
|-------------|-----------|-------|-------|---------------------------|
|0            |exit       |`addr` |`val`  |Exit with status code `val`|
|1            |set        |`addr` |`val`  |Set cell at address `addr` to value `val`|
|2            |print      |`addr` |`addr2`|Sequentially prints the values of each cell from `addr` to `addr2` (inclusively)|
|3            |in         |`addr` |`addr2`|Takes input starting at `addr` and sets `addr2` to the address of the cell at the end of the input stream|
|4            |*undefined*|N/A    |N/A    |N/A|
|5            |label      |N/A    |`val`  |Sets a label of `val` for lookback or lookahead instructions|
|6            |lookback   |N/A    |`val`  |Searches backwards and resumes execution at the first label with value `val` (lazy)|
|7            |lookahead  |N/A    |`val`  |Searches forwards and resumes execution at the first label with the value `val` (lazy)|
|8            |*undefined*|N/A    |N/A    |N/A|
|9            |*undefined*|N/A    |N/A    |N/A|
|A            |add        |`addr` |`val`  |Adds `val` to the value of the value of the cell at `addr`|
|B            |sub        |`addr` |`val`  |Subtracts `val` from the value of the cell at `addr`|
|C            |mul        |`addr` |`val`  |Multiplies the value of the call at `addr` by `val`|
|D            |div        |`addr` |`val`  |Divides the value of the cell at `addr` by `val`|
|E            |mod        |`addr` |`val`  |Mods the value of the cell at `addr` by `val`|
|F            |*undefined*|N/A    |N/A    |N/A|

### Exit Codes

|Hex    |Name             |Description                                                    |
|-------|-----------------|---------------------------------------------------------------|
|`0x00` |OK               |Execution completed successfully                               |
|`0x01` |ProgramException |Rainbow VM encountered an `exit` instruction with value `0x01` |
|`0x02` |RainbowException |Rainbow VM encountered an erroneous statement                  |
|`0x03` |InternalException|Rainbow VM encountered an unexpected exception                 |
|`0xFF` |Unknown          |An unexpected and unknown internal exception occurred          |

# The Interpreter

This Rainbow interpreter is written in C#.NET, and accepts most popular image formats as input, although .BMP is recommended. To run a program, simply compile the RainbowInterpreter project, and execute `RainbowInterpreter.exe` from the console, with your program's path as the first argument.

This interpreter also provides varied behavior for the `print` instruction. By default, print will output the contents of the tape in ASCII. However, if the flag `-h` is present as the second argument, `print` will output the hex values of cells. Alternatively, if the flag `-d` is present as the second argument, `print` will output the decimal values of cells. This is useful for debugging and for mathematical programs such as the factorial example.

# Examples

### Hello World

![Hello World](https://i.imgur.com/UbOCjLl.png)

This simple progam outputs the text `HELLO WORLD!`

### Factorial

![Factorial](https://i.imgur.com/cWdN127.png)

This program takes an integer as input, calculates the factorial of that integer, and then prints the resulting value. 

*Note: This factorial program is limited to the Rainbow memory cell size of 256 bits and therefore will only calculate up to 5 factorial, as all greater factorials result in a number greater 255, and undefined behavior.*

### Fibonacci

![Fibonacci](https://i.imgur.com/cYKt0u8.png)

This program takes an integer as input, calculates the nth number in the Fibonacci Sequence, and then prints the resulting value.

*Note: This program is also limited by the Rainbow memory cell size, and can only calculate up to the 13th number in the Fibonacci Sequence without undefined behavior.*
