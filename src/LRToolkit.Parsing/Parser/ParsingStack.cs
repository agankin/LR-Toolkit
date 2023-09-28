using System.Collections;
using System.Collections.Immutable;
using LRToolkit.Utilities;

namespace LRToolkit.Parsing;

public readonly record struct ParsingStack<TSymbol> : IEnumerable<ParsingTreeNode<TSymbol>> where TSymbol : notnull
{
    private ImmutableStack<ParsingTreeNode<TSymbol>> _nodesStack { get; init; } = ImmutableStack<ParsingTreeNode<TSymbol>>.Empty;

    public ParsingStack()
    {
    }

    public ParsingStack<TSymbol> Shift(Symbol<TSymbol> symbol)
    {
        var node = ParsingTreeNode<TSymbol>.ForShift(symbol);

        return this with { _nodesStack = _nodesStack.Push(node) };
    }

    public (ParsingStack<TSymbol>, ParsingTreeNode<TSymbol>) Reduce(Item<TSymbol> reducedItem)
    {
        var symbol = Symbol<TSymbol>.Create(reducedItem.ForSymbol);
        var reductionCount = reducedItem.ProductionCount;

        var (nodesStack, nodes) = _nodesStack.Pop(reductionCount);
        var node = ParsingTreeNode<TSymbol>.ForReduce(symbol, nodes);

        return (this with { _nodesStack = nodesStack }, node);
    }

    public IEnumerator<ParsingTreeNode<TSymbol>> GetEnumerator() => _nodesStack.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}