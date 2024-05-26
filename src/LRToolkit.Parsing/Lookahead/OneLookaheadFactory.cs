using LRToolkit.Grammaring;
using PureMonads;

namespace LRToolkit.Parsing;

public class OneLookaheadFactory<TSymbol> : ILookaheadFactory<TSymbol> where TSymbol : notnull
{
    private const int LookaheadIdx = 1;

    private readonly Grammar<TSymbol> _grammar;

    public OneLookaheadFactory(Grammar<TSymbol> grammar) => _grammar = grammar;

    public ILookahead<TSymbol> GetForStart() => new OneLookahead<TSymbol>(Symbol<TSymbol>.End());

    public Option<ILookahead<TSymbol>> GetAhead(Item<TSymbol> item) =>
        item.GetSymbolAhead(LookaheadIdx).Map(symbol => new OneLookahead<TSymbol>(symbol));

    public IEnumerable<ILookahead<TSymbol>> Produce(ILookahead<TSymbol> lookahead)
    {
        var lookaheadSymbol = lookahead[0].ValueOrFailure();

        IEnumerable<ILookahead<TSymbol>> ProduceLookaheads(TSymbol symbol) =>
            ProduceSymbols(symbol, symbol, new HashSet<TSymbol>())
                .Distinct()
                .Select(symbol => new OneLookahead<TSymbol>(symbol))
                .ToList();

        var producedLookaheads = lookaheadSymbol.Value
            .Map(ProduceLookaheads)
            .Or(Enumerable.Empty<ILookahead<TSymbol>>);
            
        return producedLookaheads.Prepend(lookahead);
    }

    private IEnumerable<TSymbol> ProduceSymbols(TSymbol symbol, TSymbol exceptSymbol, ISet<TSymbol> processedSymbols)
    {
        if (processedSymbols.Contains(symbol))
            yield break;

        if(!symbol.Equals(exceptSymbol))
            yield return symbol;

        processedSymbols.Add(symbol);

        var productions = _grammar[symbol];
        var producedSymbols = productions.SelectMany(production => ProduceSymbols(production.Production.First, exceptSymbol, processedSymbols));

        foreach (var producedSymbol in producedSymbols)
        {
            if (!symbol.Equals(producedSymbol))
                yield return producedSymbol;
        }
    }
}