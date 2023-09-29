using LRToolkit.Lexing;

namespace Playground;

using static RegexParser;
using static LispToken;

public static class LispLexer
{
    public static Lexer<LispToken> Create()
    {
        var lexer = Lexer<LispToken>.CreateFrom(
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