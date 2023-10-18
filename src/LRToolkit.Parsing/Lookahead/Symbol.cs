using LRToolkit.Lexing;
using LRToolkit.Utilities;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing;

public readonly record struct Symbol<TSymbol> where TSymbol : notnull
{
    private Symbol(SymbolType type, Option<TSymbol> value)
    {
        Type = type;
        Value = value;
        Lexem = Option.None<Lexem<TSymbol>>();
    }

    private Symbol(SymbolType type, Lexem<TSymbol> lexem)
    {
        Type = type;
        Value = lexem.Symbol.Some();
        Lexem = lexem.Some();
    }

    public SymbolType Type { get; }

    public Option<TSymbol> Value { get; }

    public Option<Lexem<TSymbol>> Lexem { get; }

    public static Symbol<TSymbol> Create(TSymbol value) => new Symbol<TSymbol>(SymbolType.Symbol, value.Some());

    public static Symbol<TSymbol> Create(Lexem<TSymbol> lexem) => new Symbol<TSymbol>(SymbolType.Symbol, lexem);

    public static Symbol<TSymbol> CreateLookahead(TSymbol value) => new Symbol<TSymbol>(SymbolType.Lookahead, value.Some());

    public static Symbol<TSymbol> End() => new Symbol<TSymbol>(SymbolType.End, Option.None<TSymbol>());

    public bool Equals(Symbol<TSymbol> other) => Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string? ToString()
    {
        return Type switch
        {
            SymbolType.Symbol => Value.ValueOrFailure().ToString(),
            SymbolType.Lookahead => Value.ValueOrFailure().ToString(),
            SymbolType.End => "$",
            _ => throw new UnsupportedEnumValueException<SymbolType>(Type)
        };
    }
}