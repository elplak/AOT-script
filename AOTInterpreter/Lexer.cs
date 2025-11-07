namespace AOTInterpreter;

public enum TokenType
{
    Identifier,
    Number,
    Plus,
    Minus,
    Star,
    Slash,
    Equal,
    LParen,
    RParen,
    Print,
    EOF
}

public record Token(TokenType Type, string Text);

public class Lexer(string text)
{
    private int _pos;

    public Token NextToken()
    {
        while (_pos < text.Length)
        {
            char c = text[_pos];
            if (char.IsWhiteSpace(c))
            {
                _pos++;
                continue;
            }

            if (char.IsLetter(c)) return ReadIdentifier();
            if (char.IsDigit(c)) return ReadNumber();

            _pos++;
            return c switch
            {
                '+' => new(TokenType.Plus, "+"),
                '-' => new(TokenType.Minus, "-"),
                '*' => new(TokenType.Star, "*"),
                '/' => new(TokenType.Slash, "/"),
                '=' => new(TokenType.Equal, "="),
                '(' => new(TokenType.LParen, "("),
                ')' => new(TokenType.RParen, ")"),
                _ => throw new Exception($"Unexpected character: {c}")
            };
        }

        return new Token(TokenType.EOF, "");
    }

    private Token ReadIdentifier()
    {
        var start = _pos;
        while (_pos < text.Length && char.IsLetter(text[_pos]))
        {
            _pos++;
        }
        var id = text[start.._pos];
        return id == "print" ? new Token(TokenType.Print, id) : new Token(TokenType.Identifier, id);
    }

    private Token ReadNumber()
    {
        var start = _pos;
        while (_pos < text.Length && char.IsDigit(text[_pos])) _pos++;
        return new Token(TokenType.Number, text[start.._pos]);
    }
}