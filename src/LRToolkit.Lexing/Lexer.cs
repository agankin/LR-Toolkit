using Optional;

namespace LRToolkit.Lexing;

public class Lexer<TToken>
{
    private readonly ParsingChain<TToken> _matchingChain;

    public Lexer(ParsingChain<TToken> matchingChain) => _matchingChain = matchingChain;

    public static Lexer<TToken> CreateFrom(params LexemParser<TToken>[] parsers)
    {
        var parsingChain = parsers.BuildChain();

        return new(parsingChain);
    }

    public IEnumerable<Option<Lexem<TToken>, string>> GetLexems(TextInput input)
    {
        Option<Lexem<TToken>, string> ToSomeResult(Lexem<TToken> lexem) => lexem.Some<Lexem<TToken>, string>();
        Option<Lexem<TToken>, string> UnknownError() => LexemValidator<TToken>.GetUnknownError(input);

        bool isError = false;
        while (!isError && !input.ReachedEnd())
        {
            var nextLexem = _matchingChain(input).Map(ToSomeResult).ValueOr(UnknownError)
                .FlatMap(LexemValidator<TToken>.NotEmpty);

            (input, isError) = nextLexem.Map(next => (input.Step(next.GetLength()), false))
                .ValueOr(_ => (input, true));

            yield return nextLexem;
        }
    }
}