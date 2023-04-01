using Optional;

namespace LRToolkit.Lexing
{
    public delegate Option<Lexem<TToken>> ParsingChain<TToken>(TextInput input);

    public delegate Option<Lexem<TToken>> TokenParser<TToken>(TextInput input);

    public static class ParsingChainExtensions
    {
        public static ParsingChain<TToken> Chain<TToken>(this IEnumerable<TokenParser<TToken>> matchers)
        {
            var chain = new ParsingChain<TToken>(input => Option.None<Lexem<TToken>>());

            return matchers.Reverse().Aggregate(chain, AddParser);
        }

        private static ParsingChain<TToken> AddParser<TToken>(
            ParsingChain<TToken> chain,
            TokenParser<TToken> parser) =>
            input =>
            {
                var lexem = parser.Invoke(input);

                return lexem.Match(Option.Some, () => chain(input));
            };
    }
}