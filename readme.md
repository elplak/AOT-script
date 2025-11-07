# AOT Script Engine

AOT Script Engine is a small C# project that builds a very simple scripting language from scratch.
It has two modes: it can **interpret** code directly, or **compile** it on the fly into real C# and run it using Roslyn.

The goal is to show how a basic language can move from text to executable code in a few clear steps.



## How it works

The engine takes lines like:

```
x = 5
y = x * 2
print y + 3
```

and either runs them right away or turns them into C# code, compiles that, and executes it.
You can switch between the interpreter and compiler mode while it’s running.



## Structure

* **Lexer.cs**: Reads characters and turns them into tokens such as identifiers, numbers, and symbols.
* **Parser.cs**: Builds a small abstract syntax tree (AST) from those tokens.
* **Node.cs**: Defines what expressions and statements look like in that tree.
* **Evaluator.cs**: Walks the AST and executes it directly.
* **CodeGenerator.cs**: Converts the AST into valid C# code and compiles it dynamically.
* **Program.cs**: A simple REPL loop that connects everything and lets you type code interactively.



## Technical details

* Written in **C#**, runs on **.NET 8+**
* Uses **Microsoft.CodeAnalysis.CSharp** for compilation
* Supports integers, variables, arithmetic (`+: * /`), parentheses, and `print`
* Error messages include line and column numbers
* Compiled programs run in memory through a static method called `ScriptProgram.Run()`



## The reason behind it

This project was built as a small personal experiment to understand how interpreters and compilers can be combined.