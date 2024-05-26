using PureMonads;

namespace LRToolkit.Lexing;

public delegate Option<Lexem<TSymbol>> ParsingChain<TSymbol>(TextInput input);

public static class ParsingChainBuilder
{
    public static ParsingChain<TSymbol> BuildChain<TSymbol>(this IEnumerable<LexemParser<TSymbol>> parsers)
    {
        var chain = new ParsingChain<TSymbol>(_ => Option.None<Lexem<TSymbol>>());

        return parsers.Reverse().Aggregate(chain, AddParser);
    }

    private static ParsingChain<TSymbol> AddParser<TSymbol>(ParsingChain<TSymbol> chain, LexemParser<TSymbol> parser) =>
        input =>
        {
            var lexem = parser.Invoke(input);

            return lexem.Map(Option.Some).Or(() => chain(input));
        };
}