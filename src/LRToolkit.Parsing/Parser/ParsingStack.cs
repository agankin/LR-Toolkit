using System.Collections;
using System.Collections.Immutable;
using LRToolkit.Utilities;

namespace LRToolkit.Parsing;

public readonly record struct ParsingStack<TSymbol> : IEnumerable<Symbol<TSymbol>> where TSymbol : notnull
{
    private ImmutableStack<Symbol<TSymbol>> _nodesStack { get; init; } = ImmutableStack<Symbol<TSymbol>>.Empty;

    public ParsingStack() { }

    public ParsingStack<TSymbol> Shift(Symbol<TSymbol> symbol) => this with { _nodesStack = _nodesStack.Push(symbol) };

    public ParsingStack<TSymbol> Reduce(
        Item<TSymbol> reducedItem,
        out Symbol<TSymbol> reducedToSymbol,
        out IReadOnlyCollection<Symbol<TSymbol>> poppedLookaheads)
    {        
        var reductionCount = reducedItem.ProductionCount;
        var lookaheadCount = reducedItem.Lookahead.Count - 1;
        
        var @this = this;
        var result = _nodesStack
            .Pop(lookaheadCount, out poppedLookaheads)
            .Pop(reductionCount, out var symbolsToReduce)
            .Pipe(stack => @this with { _nodesStack = stack });

        reducedToSymbol = Symbol<TSymbol>.CreateReduced(reducedItem.ForSymbol, symbolsToReduce);

        return result;
    }

    public Symbol<TSymbol> Peek() => _nodesStack.Peek();

    public IEnumerator<Symbol<TSymbol>> GetEnumerator() => _nodesStack.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}