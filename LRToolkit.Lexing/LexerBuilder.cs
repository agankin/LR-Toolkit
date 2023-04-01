using LRToolkit.Utilities;

namespace LRToolkit.Lexing
{
    public class LexerBuilder<TToken>
    {
        private readonly IList<TokenParser<TToken>> _parsers = new List<TokenParser<TToken>>();

        public LexerBuilder(params TokenParser<TToken>[] matchers)
            => matchers.ForEach(_parsers.Add);

        public LexerBuilder<TToken> AddParser(TokenParser<TToken> matcher)
        {
            _parsers.Add(matcher);

            return this;
        }

        public Lexer<TToken> Build()
        {
            var parsingChain = _parsers.Chain();

            return new Lexer<TToken>(parsingChain);
        }
    }
}