namespace AOTInterpreter;

public abstract class Node
{
}

public abstract class Expr : Node
{
}

public class NumberExpr(int value) : Expr
{
    public int Value { get; } = value;
}

public class VariableExpr(string name) : Expr
{
    public string Name { get; } = name;
}

public class BinaryExpr(Expr left, TokenType op, Expr right) : Expr
{
    public Expr Left { get; } = left;
    public TokenType Op { get; } = op;
    public Expr Right { get; } = right;
}

public abstract class Stmt : Node
{
}

public class PrintStmt(Expr expr) : Stmt
{
    public Expr Expression { get; } = expr;
}

public class AssignStmt(string name, Expr value) : Stmt
{
    public string Name { get; } = name;
    public Expr Value { get; } = value;
}

public class BlockStmt(List<Stmt> stmts) : Stmt
{
    public List<Stmt> Statements { get; } = stmts;
}