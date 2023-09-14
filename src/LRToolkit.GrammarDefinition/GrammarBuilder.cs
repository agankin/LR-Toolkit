using Optional.Collections;

namespace LRToolkit.GrammarDefinition;

public class GrammarBuilder<TSymbol> where TSymbol : notnull
{
    private readonly Dictionary<TSymbol, ProductionRuleCollection<TSymbol>> _productions = new();

    public GrammarBuilder(TSymbol start) => Start = start;

    public TSymbol Start { get; }

    public Production<TSymbol> this[TSymbol symbol]
    {
        set
        {
            var productions = _productions.GetValueOrNone(symbol)
                .ValueOr(() => _productions[symbol] = new(symbol));
            
            var production = new ProductionRule<TSymbol>(symbol, value);
            productions.Add(production);
        }
    }

    public Grammar<TSymbol> Build()
    {
        var productions = _productions.ToDictionary(entry => entry.Key, entry => entry.Value);

        return new Grammar<TSymbol>(Start, productions);
    }
}