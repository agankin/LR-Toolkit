using Optional;

namespace LRToolkit.Lexing;

public delegate Option<Lexem<TToken>> ParsingChain<TToken>(TextInput input);

public static class ParsingChainBuilder
{
    public static ParsingChain<TToken> BuildChain<TToken>(this IEnumerable<LexemParser<TToken>> parsers)
    {
        var chain = new ParsingChain<TToken>(_ => Option.None<Lexem<TToken>>());

        return parsers.Reverse().Aggregate(chain, AddParser);
    }

    private static ParsingChain<TToken> AddParser<TToken>(ParsingChain<TToken> chain, LexemParser<TToken> parser) =>
        input =>
        {
            var lexem = parser.Invoke(input);

            return lexem.Map(Option.Some).ValueOr(() => chain(input));
        };
}