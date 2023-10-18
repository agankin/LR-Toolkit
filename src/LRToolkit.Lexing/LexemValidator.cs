using Optional;

namespace LRToolkit.Lexing;

public static class LexemValidator<TSymbol>
{
    public static Option<Lexem<TSymbol>, string> NotEmpty(Lexem<TSymbol> lexem) =>
        lexem.GetLength() > 0
            ? lexem.Some<Lexem<TSymbol>, string>()
            : Option.None<Lexem<TSymbol>, string>("Lexem length cannot be zero.");

    public static Option<Lexem<TSymbol>, string> GetUnknownError(TextInput input) =>
        Option.None<Lexem<TSymbol>, string>($"Unknown lexem at {input.Position} character.");
}