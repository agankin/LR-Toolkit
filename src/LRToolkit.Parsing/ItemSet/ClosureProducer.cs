using LRToolkit.GrammarDefinition;
using LRToolkit.Utilities;
using Optional;

namespace LRToolkit.Parsing;

internal class ClosureProducer<TSymbol> where TSymbol : notnull
{
    private readonly Grammar<TSymbol> _grammar;
    private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;

    public ClosureProducer(Grammar<TSymbol> grammar, ILookaheadFactory<TSymbol> lookaheadFactory)
    {
        _grammar = grammar;
        _lookaheadFactory = lookaheadFactory;
    }

    public IReadOnlySet<Item<TSymbol>> Produce(Item<TSymbol> kernelItem)
    {
        var symbolAheadOption = GetAhead(kernelItem);
        var closures = symbolAheadOption.Map(symbolAhead => ProduceClosures(symbolAhead).ToHashSet())
            .ValueOr(new HashSet<Item<TSymbol>>());

        return closures;
    }

    private Option<SymbolAhead> GetAhead(Item<TSymbol> item)
    {
        var symbolOption = item.GetSymbolAhead().FlatMap(symbolAhead => symbolAhead.Value);
        var lookaheadOption = _lookaheadFactory.GetAhead(item);

        return symbolOption.FlatMap(symbol => lookaheadOption.Map(lookahead => new SymbolAhead(symbol, lookahead)));
    }

    private IEnumerable<Item<TSymbol>> ProduceClosures(SymbolAhead symbolAhead) =>
        ProduceClosures(symbolAhead, new HashSet<SymbolAhead>());

    private IEnumerable<Item<TSymbol>> ProduceClosures(SymbolAhead symbolAhead, ISet<SymbolAhead> processedAheads)
    {
        var (symbol, lookahead) = symbolAhead;
        processedAheads.Add(symbolAhead);
        
        var symbolProductions = _grammar[symbol];
        var producedLookaheads = _lookaheadFactory.Produce(lookahead).ToList();
        
        var closures = symbolProductions
            .SelectMany(rule => producedLookaheads.Select(lookahead => Item<TSymbol>.FromRule(rule, lookahead)));

        var notProcessedAheads = closures.OnlySome(GetSymbolAhead)
            .Where(symbolAhead => !processedAheads.Contains(symbolAhead))
            .Distinct();

        var producedClosures = notProcessedAheads.SelectMany(symbolAhead => ProduceClosures(symbolAhead, processedAheads));

        return closures.Concat(producedClosures);
    }

    private Option<SymbolAhead> GetSymbolAhead(Item<TSymbol> item) =>
        _lookaheadFactory.GetAhead(item).Map(lookahead => new SymbolAhead(item.Production.First, lookahead));

    private readonly record struct SymbolAhead(
        TSymbol Symbol,
        ILookahead<TSymbol> Lookahead
    );
}