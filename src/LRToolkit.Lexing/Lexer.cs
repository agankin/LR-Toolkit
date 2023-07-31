using Optional;

namespace LRToolkit.Lexing;

public class Lexer<TToken>
{
    private readonly ParsingChain<TToken> _matchingChain;

    public Lexer(ParsingChain<TToken> matchingChain)
        => _matchingChain = matchingChain;

    public IEnumerable<Option<Lexem<TToken>, string>> GetLexems(TextInput input)
    {
        bool isError = false;
        while (!isError && !input.ReachedEnd())
        {
            var nextLexem = _matchingChain(input)
                .Match(
                    Option.Some<Lexem<TToken>, string>,
                    LexemValidationUtils<TToken>.UnmatchedResult(input))
                .FlatMap(LexemValidationUtils<TToken>.CheckNotEmpty);

            (input, isError) = nextLexem.Match(
                next => (input.Step(next.GetLength()), false),
                _ => (input, true));

            yield return nextLexem;
        }
    }
}