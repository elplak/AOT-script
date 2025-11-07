using AOTInterpreter;

Console.WriteLine("AOT Script Engine - type 'exit' to quit. Type ':mode interpret' or ':mode compile' to switch mode.");

var generator = new CodeGenerator();
var evaluator = new Evaluator();
var accumulatedCode = new List<string>();
var interpretMode = true;

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (input == null || input.Trim().Equals("exit", StringComparison.CurrentCultureIgnoreCase))
    {
        break;
    }

    if (input.StartsWith(":mode", StringComparison.OrdinalIgnoreCase))
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1)
        {
            interpretMode = parts[1].Equals("interpret", StringComparison.OrdinalIgnoreCase);
            Console.WriteLine($"Mode switched to {(interpretMode ? "Interpreter" : "Compiler")} mode.");
        }
        continue;
    }

    try
    {
        var lexer = new Lexer(input);
        var tokens = new List<Token>();
        Token token;
        do
        {
            token = lexer.NextToken();
            tokens.Add(token);
        } while (token.Type != TokenType.EOF);

        var parser = new Parser(tokens);
        var block = parser.Parse();

        // true = interpret directly with Evaluator, false = compile and run via codeGenerator
        if (interpretMode)
        {
            evaluator.Execute(block);
        }
        else
        {
            var newCode = generator.Generate(block);
            accumulatedCode.AddRange(newCode.Split('\n').Skip(3).SkipLast(3));

            var fullCode =
                "using System;\n" +
                "public static class ScriptProgram{\n" +
                "  public static void Run(){\n" +
                string.Join('\n', accumulatedCode) +
                "\n  }\n}";

            generator.CompileAndRun(fullCode);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
    }
}