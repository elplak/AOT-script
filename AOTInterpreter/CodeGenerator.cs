using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace AOTInterpreter;

public class CodeGenerator
{
    public string Generate(BlockStmt block)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("public static class ScriptProgram {");
        sb.AppendLine("  public static void Run() {");

        foreach (var stmt in block.Statements)
        {
            sb.AppendLine(GenerateStatement(stmt));
        }

        sb.AppendLine("  }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private string GenerateStatement(Stmt stmt) => stmt switch
    {
        PrintStmt p => $"Console.WriteLine({GenerateExpression(p.Expression)});",
        AssignStmt a => $"var {a.Name} = {GenerateExpression(a.Value)};",
        _ => throw new Exception($"Unknown statement type: {stmt.GetType()}")
    };

    private string GenerateExpression(Expr expr) => expr switch
    {
        NumberExpr n => n.Value.ToString(),
        VariableExpr v => v.Name,
        BinaryExpr b => $"({GenerateExpression(b.Left)} {OpToString(b.Op)} {GenerateExpression(b.Right)})",
        _ => throw new Exception($"Unknown expression type: {expr.GetType()}")
    };

    private string OpToString(TokenType type) => type switch
    {
        TokenType.Plus => "+",
        TokenType.Minus => "-",
        TokenType.Star => "*",
        TokenType.Slash => "/",
        _ => throw new Exception($"Unknown operator {type}")
    };

    public void CompileAndRun(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var assemblyName = Path.GetRandomFileName();

        var refs = new List<MetadataReference>();

        var runtimeDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
        refs.Add(MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "mscorlib.dll")));
        refs.Add(MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Private.CoreLib.dll")));
        refs.Add(MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Runtime.dll")));
        refs.Add(MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Console.dll")));
        refs.Add(MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Collections.dll")));
        refs.Add(MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Linq.dll")));
        refs.Add(MetadataReference.CreateFromFile(Path.Combine(runtimeDir, "System.Private.Uri.dll")));

        refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        refs.Add(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location));


        var compilation = CSharpCompilation.Create(
            assemblyName,
            [syntaxTree],
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );


        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                Console.WriteLine(diagnostic);
            }

            return;
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var type = assembly.GetType("ScriptProgram");
        var method = type?.GetMethod("Run", BindingFlags.Static | BindingFlags.Public);
        method?.Invoke(null, null);
    }
}