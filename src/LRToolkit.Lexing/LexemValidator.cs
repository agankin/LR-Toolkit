using Optional;

namespace LRToolkit.Lexing;

public static class LexemValidator<TToken>
{
    public static Option<Lexem<TToken>, string> NotEmpty(Lexem<TToken> lexem) =>
        lexem.GetLength() > 0
            ? lexem.Some<Lexem<TToken>, string>()
            : Option.None<Lexem<TToken>, string>("Lexem length cannot be zero.");

    public static Option<Lexem<TToken>, string> GetUnknownError(TextInput input) =>
        Option.None<Lexem<TToken>, string>($"Unknown lexem at {input.Position} character.");
}