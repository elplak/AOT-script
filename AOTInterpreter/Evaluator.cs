namespace AOTInterpreter;

public class Evaluator
{
    private readonly Dictionary<string, int> _env = new();

    public void Execute(BlockStmt block)
    {
        foreach (var stmt in block.Statements)
            ExecuteStatement(stmt);
    }

    private void ExecuteStatement(Stmt stmt)
    {
        switch (stmt)
        {
            case PrintStmt p:
                Console.WriteLine(Evaluate(p.Expression));
                break;

            case AssignStmt a:
                _env[a.Name] = Evaluate(a.Value);
                break;

            default:
                throw new Exception($"Unknown statement type: {stmt.GetType()}");
        }
    }

    private int Evaluate(Expr expr) => expr switch
    {
        NumberExpr n => n.Value,
        VariableExpr v => _env.TryGetValue(v.Name, out var val)
            ? val
            : throw new Exception($"Undefined variable {v.Name}"),
        BinaryExpr b => EvaluateBinary(b),
        _ => throw new Exception($"Unknown expression type: {expr.GetType()}")
    };

    private int EvaluateBinary(BinaryExpr b)
    {
        var left = Evaluate(b.Left);
        var right = Evaluate(b.Right);
        return b.Op switch
        {
            TokenType.Plus => left + right,
            TokenType.Minus => left - right,
            TokenType.Star => left * right,
            TokenType.Slash => left / right,
            _ => throw new Exception($"Unknown operator {b.Op}")
        };
    }
}