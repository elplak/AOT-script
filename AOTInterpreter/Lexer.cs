namespace AOTInterpreter;

public enum TokenType
{
    Identifier,
    Number,
    String,
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

// adds ToString override so tokens display their position when printed or logged --> especially useful for error messages TODO: unit tests should cover this
public record Token(TokenType Type, string Text, int Line, int Column)
{
    public override string ToString()
    {
        return $"{Type} ('{Text}') at line {Line}, column {Column}";
    }
}

public class Lexer(string text)
{
    private int _pos;
    private int _line = 1;
    private int _col = 1;

    public Token NextToken()
    {
        while (_pos < text.Length)
        {
            var c = text[_pos];

            // skip whitespaces for now
            if (char.IsWhiteSpace(c))
            {
                if (c == '\n')
                {
                    _line++;
                    _col = 1;
                }
                else
                {
                    _col++;
                }

                _pos++;
                continue;
            }

            if (char.IsLetter(c))
            {
                return ReadIdentifier();
            }

            if (char.IsDigit(c))
            {
                return ReadNumber();
            }

            if (c == '"')
            {
                return ReadString();
            }

            var startCol = _col;
            _pos++;
            _col++;
            return c switch
            {
                '+' => new Token(TokenType.Plus, "+", _line, startCol),
                '-' => new Token(TokenType.Minus, "-", _line, startCol),
                '*' => new Token(TokenType.Star, "*", _line, startCol),
                '/' => new Token(TokenType.Slash, "/", _line, startCol),
                '=' => new Token(TokenType.Equal, "=", _line, startCol),
                '(' => new Token(TokenType.LParen, "(", _line, startCol),
                ')' => new Token(TokenType.RParen, ")", _line, startCol),
                _ => throw new Exception($"Unexpected character '{c}' at line {_line}, column {startCol}"),
            };
        }

        return new Token(TokenType.EOF, "", _line, _col);
    }

    private Token ReadIdentifier()
    {
        var start = _pos;
        var startCol = _col;
        while (_pos < text.Length && char.IsLetter(text[_pos]))
        {
            _pos++;
            _col++;
        }

        var id = text[start.._pos];
        if (id == "print")
        {
            return new Token(TokenType.Print, id, _line, startCol);
        }
        return new Token(TokenType.Identifier, id, _line, startCol);
    }

    private Token ReadNumber()
    {
        var start = _pos;
        var startCol = _col;
        while (_pos < text.Length && char.IsDigit(text[_pos]))
        {
            _pos++;
            _col++;
        }

        return new Token(TokenType.Number, text[start.._pos], _line, startCol);
    }

    private Token ReadString()
    {
        var startCol = _col;
        _pos++; // skip opening "
        _col++;

        var start = _pos;
        while (_pos < text.Length && text[_pos] != '"')
        {
            if (text[_pos] == '\n')
            {
                throw new Exception($"Unterminated string starting at line {_line}, column {startCol}");
            }
            _pos++;
            _col++;
        }

        if (_pos >= text.Length)
        {
            throw new Exception($"Unterminated string starting at line {_line}, column {startCol}");
        }

        var value = text[start.._pos];
        _pos++; // skip closing "
        _col++;

        return new Token(TokenType.String, value, _line, startCol);
    }
}
