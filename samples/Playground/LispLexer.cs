using LRToolkit.Lexing;

namespace Playground;

using static RegexParser;
using static LispSymbol;

public static class LispLexer
{
    public static Lexer<LispSymbol> Create()
    {
        var lexer = Lexer<LispSymbol>.CreateFrom(
            Regex(@"\G\(").Is(OPEN),
            Regex(@"\G\)").Is(CLOSE),

            Regex(@"\G\+").Is(ATOM),
            Regex(@"\G-").Is(ATOM),
            Regex(@"\G\*").Is(ATOM),
            Regex(@"\G\/").Is(ATOM),

            Regex(@"\G\d+").Is(ATOM),

            Regex(@"\G\s+").Is(WHITESPACE)
        );

        return lexer;
    }
}