# AOT Script Engine (Ahead-of-Time Interpreter Experiment)

This project is a small **experimental interpreter and runtime compiler** written in C#.
It explores a hybrid model between **interpreted execution** and **dynamic code generation**, combining a traditional AST evaluator with a Roslyn-based AOT compilation step.

The project serves as a playground for studying **lexing**, **parsing**, **AST evaluation**, and **runtime code emission** using the .NET compiler platform.

## Interpreter Core

At its heart, the AOT Script Engine defines a simple expression language that supports:

* Variable assignment
* Arithmetic (`+`, `-`, `*`, `/`)
* `print` statements
* Grouping with parentheses

Example:

```text
x = 10
y = x * 2
print y + 5
```

### Lexing

The `Lexer` converts source text into tokens such as identifiers, numbers, and operators:

```csharp
var lexer = new Lexer("print 1 + 2 * 3");
var token = lexer.NextToken(); // Token(Print, "print"), Token(Number, "1"), ...
```

It recognizes keywords (`print`), numbers, and standard arithmetic operators.

### Parsing

The `Parser` transforms the token stream into an **abstract syntax tree (AST)**.
It implements a **recursive descent parser**, producing nodes like `BinaryExpr`, `AssignStmt`, and `PrintStmt`.

```csharp
var parser = new Parser(tokens);
var block = parser.Parse();
```

Example AST node structure (simplified):

```
Block
 ├─ AssignStmt(x, BinaryExpr(Number(10), *, Number(2)))
 └─ PrintStmt(VariableExpr(x))
```

### Evaluation

The `Evaluator` executes the parsed AST directly.
It maintains an **environment dictionary** for variable bindings and computes integer results.

```csharp
var evaluator = new Evaluator();
evaluator.Execute(block);
```

Output:

```
25
```

## Ahead-of-Time Code Generation (Experimental)

The experimental `CodeGenerator` explores compiling the parsed AST into **real C# code** and executing it via Roslyn at runtime.

Example usage:

```csharp
var generator = new CodeGenerator();
var code = generator.Generate(block);
generator.CompileAndRun(code);
```

Generated code example:

```csharp
using System;
public static class ScriptProgram {
  public static void Run() {
    var x = 10;
    var y = (x * 2);
    Console.WriteLine((y + 5));
  }
}
```

### What happens internally

* The AST is translated into a valid C# syntax tree string.
* Roslyn compiles it in-memory to a dynamic assembly.
* The assembly is immediately loaded and executed via reflection.
* Compilation references are resolved from the current runtime environment.

This allows the same script to run **interpreted** or **ahead-of-time compiled**, depending on which path you choose.

## Runtime Interaction

`Program.cs` implements a **REPL** (Read–Eval–Print Loop).
Each line entered is lexed, parsed, and dynamically compiled into an ever-growing class body:

```text
AOT Script Engine - type 'exit' to quit.
> x = 5
> print x + 1
6
```

Internally, previously entered statements are accumulated, effectively creating a live script that persists across commands.

## Motivation

This project explores a fundamental question:

> Can we mix *interpreter simplicity* with *compiler efficiency* in a minimal C# sandbox?

It provides an approachable framework for experimenting with parsing theory, dynamic compilation, and runtime reflection, only using standard .NET libraries.

## Status

This is an **educational experiment**, not intended for production.
It can serve as a base for building:

* Small scripting languages
* REPL shells for embedded systems
* AOT-compilation experiments with Roslyn
