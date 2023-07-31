using Optional;

namespace LRToolkit.Lexing;

public static class LexemValidationUtils<TToken>
{
    public static Func<Option<Lexem<TToken>, string>> UnmatchedResult(TextInput input) =>
        () => Option.None<Lexem<TToken>, string>($"Unknown lexem at {input.Position} character.");

    public static Option<Lexem<TToken>, string> CheckNotEmpty(Lexem<TToken> lexem) =>
        lexem.GetLength() > 0
            ? Option.Some<Lexem<TToken>, string>(lexem)
            : Option.None<Lexem<TToken>, string>("Lexem length cannot be zero.");
}