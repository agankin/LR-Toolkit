using System.Collections.Immutable;
using LRToolkit.Lexing;
using LRToolkit.Utilities;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing;

public sealed record Symbol<TSymbol> where TSymbol : notnull
{
    public required SymbolType Type { get; init; }

    public Option<TSymbol> Value { get; private init; } = Option.None<TSymbol>();

    public Option<Lexem<TSymbol>> Lexem { get; private init; } = Option.None<Lexem<TSymbol>>();

    public IReadOnlyCollection<Symbol<TSymbol>> ReducedFromSymbols { get; private init; } = ImmutableList<Symbol<TSymbol>>.Empty;

    public static Symbol<TSymbol> Create(TSymbol value) => new Symbol<TSymbol>
    {
        Type = SymbolType.Symbol,
        Value = value.Some()
    };

    public static Symbol<TSymbol> Create(Lexem<TSymbol> lexem) => new Symbol<TSymbol>
    {
        Type = SymbolType.Symbol,
        Value = lexem.Symbol.Some(),
        Lexem = lexem.Some()
    };

    public static Symbol<TSymbol> CreateLookahead(TSymbol value) => new Symbol<TSymbol>
    {
        Type = SymbolType.Lookahead,
        Value = value.Some()
    };

    public static Symbol<TSymbol> CreateReduced(TSymbol value, IReadOnlyCollection<Symbol<TSymbol>> reducedFromSymbols) => new Symbol<TSymbol>
    {
        Type = SymbolType.Symbol,
        Value = value.Some(),
        ReducedFromSymbols = reducedFromSymbols
    };

    public static Symbol<TSymbol> End() => new Symbol<TSymbol>
    {
        Type = SymbolType.End
    };

    public bool Equals(Symbol<TSymbol>? other) => other is not null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string? ToString()
    {
        return Type switch
        {
            SymbolType.Symbol => Value.Map(value => value.ToString()).ValueOr("NONE"),
            SymbolType.Lookahead => Value.ValueOrFailure().ToString(),
            SymbolType.End => "$",
            _ => throw new UnsupportedEnumValueException<SymbolType>(Type)
        };
    }
}