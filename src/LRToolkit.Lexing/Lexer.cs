using Optional;

namespace LRToolkit.Lexing;

public class Lexer<TSymbol>
{
    private readonly ParsingChain<TSymbol> _matchingChain;

    public Lexer(ParsingChain<TSymbol> matchingChain) => _matchingChain = matchingChain;

    public static Lexer<TSymbol> CreateFrom(params LexemParser<TSymbol>[] parsers)
    {
        var parsingChain = parsers.BuildChain();

        return new(parsingChain);
    }

    public IEnumerable<Option<Lexem<TSymbol>, string>> GetLexems(TextInput input)
    {
        Option<Lexem<TSymbol>, string> ToSomeResult(Lexem<TSymbol> lexem) => lexem.Some<Lexem<TSymbol>, string>();
        Option<Lexem<TSymbol>, string> UnknownError() => LexemValidator<TSymbol>.GetUnknownError(input);

        bool isError = false;
        while (!isError && !input.ReachedEnd())
        {
            var nextLexem = _matchingChain(input).Map(ToSomeResult).ValueOr(UnknownError)
                .FlatMap(LexemValidator<TSymbol>.NotEmpty);

            (input, isError) = nextLexem.Map(next => (input.Step(next.GetLength()), false))
                .ValueOr(_ => (input, true));

            yield return nextLexem;
        }
    }
}