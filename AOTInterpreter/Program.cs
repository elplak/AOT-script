using AOTInterpreter;

Console.WriteLine("AOT Script Engine - type 'exit' to quit.");

var generator = new CodeGenerator();
var accumulatedCode = new List<string>();

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (input == null || input.Trim().Equals("exit", StringComparison.CurrentCultureIgnoreCase))
    {
        break;
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
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
    }
}