namespace AOTInterpreter;

public class Parser(List<Token> tokens)
{
    private int _pos;

    private Token Current => _pos < tokens.Count ? tokens[_pos] : tokens[^1];

    private Token Eat(TokenType type)
    {
        if (Current.Type != type)
        {
            throw new Exception($"Expected {type}, got {Current.Type}");
        }
        return tokens[_pos++];
    }

    public BlockStmt Parse()
    {
        var statements = new List<Stmt>();
        while (Current.Type != TokenType.EOF)
            statements.Add(ParseStatement());
        return new BlockStmt(statements);
    }

    private Stmt ParseStatement()
    {
        if (Current.Type == TokenType.Print)
        {
            Eat(TokenType.Print);
            var expr = ParseExpression();
            return new PrintStmt(expr);
        }

        if (Current.Type == TokenType.Identifier &&
            Peek(1)?.Type == TokenType.Equal)
        {
            var name = Eat(TokenType.Identifier).Text;
            Eat(TokenType.Equal);
            var expr = ParseExpression();
            return new AssignStmt(name, expr);
        }

        throw new Exception($"Unexpected token {Current.Type}");
    }

    private Token? Peek(int offset)
    {
        var index = _pos + offset;
        return index < tokens.Count ? tokens[index] : null;
    }

    private Expr ParseExpression() => ParseTermTail(ParseTerm());

    private Expr ParseTerm()
    {
        var expr = ParseFactor();
        while (Current.Type is TokenType.Plus or TokenType.Minus)
        {
            var op = Current.Type;
            Eat(op);
            var right = ParseFactor();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr ParseTermTail(Expr left)
    {
        while (Current.Type is TokenType.Plus or TokenType.Minus)
        {
            var op = Current.Type;
            Eat(op);
            var right = ParseFactor();
            left = new BinaryExpr(left, op, right);
        }

        return left;
    }

    private Expr ParseFactor()
    {
        var expr = ParsePrimary();
        while (Current.Type is TokenType.Star or TokenType.Slash)
        {
            var op = Current.Type;
            Eat(op);
            var right = ParsePrimary();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr ParsePrimary()
    {
        if (Current.Type == TokenType.Number)
        {
            var value = int.Parse(Eat(TokenType.Number).Text);
            return new NumberExpr(value);
        }

        if (Current.Type == TokenType.Identifier)
        {
            var name = Eat(TokenType.Identifier).Text;
            return new VariableExpr(name);
        }

        if (Current.Type == TokenType.LParen)
        {
            Eat(TokenType.LParen);
            var expr = ParseExpression();
            Eat(TokenType.RParen);
            return expr;
        }

        throw new Exception($"Unexpected token {Current.Type}");
    }
}