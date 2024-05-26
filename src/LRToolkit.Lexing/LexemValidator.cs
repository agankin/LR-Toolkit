using PureMonads;

namespace LRToolkit.Lexing;

public static class LexemValidator<TSymbol>
{
    public static Result<Lexem<TSymbol>, string> NotEmpty(Lexem<TSymbol> lexem) =>
        lexem.GetLength() > 0 ? lexem : "Lexem length cannot be zero.";

    public static Result<Lexem<TSymbol>, string> GetUnknownError(TextInput input) =>
        $"Unknown lexem at {input.Position} character.";
}