using LRToolkit.Lexing;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing;

public readonly record struct ParsingTreeNode<TSymbol> where TSymbol : notnull
{
    public required readonly TSymbol Symbol { get; init; }

    public required readonly Option<Lexem<TSymbol>> Lexem { get; init; }

    public required IEnumerable<ParsingTreeNode<TSymbol>> Nodes { get; init; }

    public static ParsingTreeNode<TSymbol> ForShift(Symbol<TSymbol> symbol) =>
        new ParsingTreeNode<TSymbol>
        {
            Symbol = symbol.Value.ValueOrFailure(),
            Lexem = symbol.Lexem,
            Nodes = Enumerable.Empty<ParsingTreeNode<TSymbol>>()
        };

    public static ParsingTreeNode<TSymbol> ForReduce(Symbol<TSymbol> symbol, IEnumerable<ParsingTreeNode<TSymbol>> nodes) =>
        new ParsingTreeNode<TSymbol>
        {
            Symbol = symbol.Value.ValueOrFailure(),
            Lexem = symbol.Lexem,
            Nodes = nodes
        };
}